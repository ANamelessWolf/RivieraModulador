using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using DaSoft.Riviera.OldModulador.Controller;
using DaSoft.Riviera.OldModulador.Model;
using DaSoft.Riviera.OldModulador.Query;
using DaSoft.Riviera.OldModulador.Runtime;
using DaSoft.Riviera.OldModulador.Runtime.DaNTe;
using MahApps.Metro.Controls;
using NamelessOld.Libraries.HoukagoTeaTime.Runtime;
using NamelessOld.Libraries.HoukagoTeaTime.Yui;
using NamelessOld.Libraries.Yggdrasil.Aerith;
using NamelessOld.Libraries.Yggdrasil.Lain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using static DaSoft.Riviera.OldModulador.Assets.Strings;
using static DaSoft.Riviera.OldModulador.Runtime.CMD;

namespace DaSoft.Riviera.OldModulador.UI.Delta
{
    /// <summary>
    /// Interaction logic for Dialog_Quantification.xaml
    /// </summary>
    public partial class Dialog_Quantification : MetroWindow
    {
        /// <summary>
        /// El cuantificador de la aplicación
        /// </summary>
        public Quantifier QMan;
        public Dialog_Quantification()
        {
            QMan = new Quantifier();
            InitializeComponent();
        }

        public void tableElements_Fill(object sender, RoutedEventArgs e)
        {
            Ctrl_QuantifyTable table = (Ctrl_QuantifyTable)sender;
            table.list.Items.Clear();
            List<QuantifiableObject> grpItems = new List<QuantifiableObject>();

            foreach (QuantifiableObject item in this.QMan.ItemQuantification.Union(this.QMan.ModuleQuantification))
            {
                int index = -1;
                for (int i = 0; i < grpItems.Count && index == -1; i++)
                    if (item.Code == grpItems[i].Code)
                        index = i;
                if (index == -1)
                    grpItems.Add(new QuantifiableObject() { Code = item.Code, Count = item.Count, Visibility = true, Zone = item.Zone });
                else
                    grpItems[index].Count += item.Count;
            }
            foreach (QuantifiableObject item in grpItems)
                table.AddItem(item.Code, item.Count, item);
            try
            {
                List<Tuple<string, int, long>> items = new List<Tuple<string, int, long>>();
                //Se cargan los códigos extendidos
                foreach (var item in grpItems)
                    items.Add(new Tuple<string, int, long>(item.Code, item.Count, item.Handle));
                foreach (var item in DaNTe_Transaction.GetAsociados(items))
                    table.AddItem(item.ItemKey, item.Count, item);
            }
            catch (Exception exc)
            {
                Selector.Ed.WriteMessage("Error mostrando asociados\n{0}", exc.Message);
                App.Riviera.Log.AppendEntry("Error mostrando asociados\n" + exc.Message, Protocol.Error, "tableElements_Fill", "Dialog_Quantification");
            }
        }

        public void tableUnion_Fill(object sender, RoutedEventArgs e)
        {
            Ctrl_QuantifyTable table = (Ctrl_QuantifyTable)sender;
            table.list.Items.Clear();
            List<QuantifiableUnion> grpUnion = new List<QuantifiableUnion>();
            Boolean add;
            for (int i = 0; i < this.QMan.UnionQuantification.Count; i++)
            {
                add = true;
                if (grpUnion.Count > 0)
                {
                    for (int j = 0; add && j < grpUnion.Count; j++)
                    {
                        if (grpUnion[j].EqualtTo(this.QMan.UnionQuantification[i], false))
                        {
                            add = false;
                            grpUnion[j].Count++;
                        }
                    }
                }

                if (add)
                    grpUnion.Add(this.QMan.UnionQuantification[i].Copy());

            }
            foreach (QuantifiableUnion union in grpUnion)
                table.AddItem(union.GetCellFormat(), union.Count, union);
        }

        private void refreshTable_Click(object sender, RoutedEventArgs e)
        {

            Selector.InvokeCMD(DELTA_CLEAR_CACHE);
        }

        private void exportToCVS(object sender, RoutedEventArgs e)
        {
            this.Refresh();

            List<String> data = new List<string>();
            data.Add(string.Format("Clave,Cantidad"));
            foreach (var item in QMan.ItemQuantification)
                data.Add(String.Format("{0},{1}", item.Code, item.Count));
            String[] codes = QMan.ItemQuantification.Select<QuantifiableObject, String>(y => y.Code).ToArray();
            Dictionary<String, int> unionItems = new Dictionary<string, int>();
            foreach (var item in QMan.UnionQuantification)
                foreach (var subItem in item.Members.Where(x => !codes.Contains(x)))
                {
                    if (unionItems.ContainsKey(subItem))
                        unionItems[subItem] += item.Count;
                    else
                        unionItems.Add(subItem, item.Count);
                }
            foreach (var item in unionItems.Keys.ToList())
                data.Add(String.Format("{0},{1}", item, unionItems[item]));

            NamelessOld.Libraries.Yggdrasil.Aerith.FileSaver<Object> save =
                new NamelessOld.Libraries.Yggdrasil.Aerith.FileSaver<object>(
                    delegate (string saveName, object[] savingParameters)
                    {
                        List<String> lines = savingParameters[0] as List<String>;
                        System.IO.File.WriteAllLines(saveName, lines);
                    }, "CSV");
            save.ShowDialog("CSV File", "Exportar cuantificación", data);

        }

        private void Refresh()
        {
            this.QMan = new Quantifier();
            this.tableElements.List.Items.Clear();
            this.tableUnion.List.Items.Clear();
            this.tableElements_Fill(this.tableElements, null);
            this.tableUnion_Fill(this.tableUnion, null);
        }

        private void exportToAccess(object sender, RoutedEventArgs e)
        {
            this.Refresh();
            List<String> zones = new ZoneManager().ListZones();
            List<string> qs = new ZoneManager().ListQuantifications();
            Dialog_AccessExport aExport = new Dialog_AccessExport(qs, zones.ToArray());
            aExport.SendAction =
                () =>
                {
                    new ZoneManager().SaveListQuantifications(Dialog_AccessExport.DoneQuantifications);
                    String qName = aExport.QuantificationName;
                    String aPath = String.Empty;
                    if (aExport.Result == DaNTeExportStatus.Save)
                    {
                        if (App.Riviera.DaNTeMDB != null)
                            aPath = App.Riviera.DaNTeMDB.FullName;
                        else
                        {
                            FilePicker picker = new FilePicker(false, "MDB");
                            picker.PickPath(CAPTION_DAN, TIT_SEL_DANTE, out aPath);
                        }
                    }
                    else if (aExport.Result == DaNTeExportStatus.New)
                    {
                        FileSaver<Object> sav_Dialog = new FileSaver<object>(delegate (string saveName, object[] savingParameters)
                        {
                            File.WriteAllBytes(saveName, File.ReadAllBytes(App.Riviera.DaNTeDBTemplate.FullName));
                        }, "MDB");
                        try
                        {
                            sav_Dialog.ShowDialog(CAPTION_DAN, TIT_EXPORT_QUAN);
                            aPath = sav_Dialog.SavedFileName;
                        }
                        catch (Exception exc)
                        {
                            Dialog_MessageBox.Show(ERR_SAV_ACCESS + exc.InnerException.Message, MessageBoxButton.OK, MessageBoxImage.Error);
                            aPath = String.Empty;
                        }
                    }

                    if (qName != String.Empty && aPath != String.Empty)
                    {
                        DaNTeQuantification q = new DaNTeQuantification(aPath, qName, this.QMan);
                        q.Quantify(aExport.Zone, aExport.Comments, aExport.GroupedQuantification, aExport.MergeQuantification);
                    }
                };
            aExport.Show();
        }

        private void exportToOracle(object sender, RoutedEventArgs e)
        {
            this.Refresh();
            List<String> zones = new ZoneManager().ListZones();

            if (App.Riviera.Credentials.ProjectId != -1 &&
                Dialog_MessageBox.Show(String.Format(MSG_ORACLE_Q, App.Riviera.Credentials.ProjectId.GetProjectName(App.Riviera.Credentials)), MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
            {
                Dialog_OracleExport aExport = new Dialog_OracleExport(zones.ToArray());
                aExport.ShowDialog();
                String qName = aExport.QuantificationName;
                if (aExport.Result == DaNTeExportStatus.Oracle)
                {
                    DaNTeQuantification q = new DaNTeQuantification(App.Riviera.Credentials.ProjectId, qName, this.QMan);
                    q.Quantify(aExport.Zone, aExport.Comments, aExport.GroupedQuantification, aExport.MergeQuantification);
                }
            }

        }
        /// <summary>
        /// Agrega llave de dantes a los elementos insertados con la aplicación
        /// </summary>
        private void createDaNTeKeys(object sender, RoutedEventArgs e)
        {
            DaNTeCodeSetter set = new DaNTeCodeSetter();
            FastTransactionWrapper ft = new FastTransactionWrapper
            (delegate (Document doc, Transaction tr)
             {
                 doc.LockDocument();
                 set.AddDanteToDatabase(tr);
             });
            ft.Run();
            set.HiglightFailed();
            Dialog_MessageBox.Show("Claves de DaNTe, agregadas a la geometría de la aplicación", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}

