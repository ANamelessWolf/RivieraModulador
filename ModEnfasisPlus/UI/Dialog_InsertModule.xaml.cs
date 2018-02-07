using DaSoft.Riviera.OldModulador.Model;
using DaSoft.Riviera.OldModulador.Query;
using DaSoft.Riviera.OldModulador.Runtime;
using MahApps.Metro.Controls;
using NamelessOld.Libraries.DB.Mikasa.Model;
using NamelessOld.Libraries.DB.Tessa;
using NamelessOld.Libraries.DB.Tessa.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;

namespace DaSoft.Riviera.OldModulador.UI
{
    /// <summary>
    /// Interaction logic for Dialog_InsertModule.xaml
    /// </summary>
    public partial class Dialog_InsertModule : MetroWindow
    {
        /// <summary>
        /// Las claves seleccionadas
        /// </summary>
        public List<Tuple<string, int>> Claves;
        /// <summary>
        /// La acción del modulo
        /// </summary>
        public ModuleAction Action;
        /// <summary>
        /// El nombre del módulo
        /// </summary>
        public String ModuleName
        {
            get { return this.listOfModulos.Items.Count > 0 && this.listOfModulos.SelectedIndex != -1 ? this.listOfModulos.SelectedItem.ToString() : String.Empty; }
        }
        public Dialog_InsertModule()
        {
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                this.listOfModulos.Items.Clear();
                IEnumerable<String> modulos = new List<String>();
                AccessConnectionBuilder aBuilder = new AccessConnectionBuilder()
                {
                    Access_DB_File = App.Riviera.ModulosMDB.FullName,
                    OleDbProvider = new AccessProvider(OledbProviders.Microsoft_ACE_OleDb_12),
                };
                new VoidAccess_Transaction<Object>(new AccessConnectionContent(aBuilder.Data).GenerateConnectionString(),
                    delegate (Access_Connector conn, Object[] trParameters)
                    {
                        Query_BasesMDB q = new Query_BasesMDB();
                        modulos = conn.SelectTables().Where(x => x != q.TableName && x != q.TableName_DaNTeBase);
                    }).Run();
                string dwgname;
                foreach (String blockName in modulos.OrderBy(x => x))
                {
                    dwgname = Path.Combine(App.Riviera.Modules2D.FullName, String.Format("{0}.dwg", blockName));
                    if (File.Exists(dwgname))
                        this.listOfModulos.Items.Add(blockName);
                }
                if (this.listOfModulos.Items.Count > 0)
                    this.listOfModulos.SelectedIndex = 0;
            }
            catch { }
        }

        private void listOfModulos_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Claves = new List<Tuple<string, int>>();
            if (this.listOfModulos.SelectedIndex != -1)
            {
                try
                {
                    AccessConnectionBuilder aBuilder = new AccessConnectionBuilder()
                    {
                        Access_DB_File = App.Riviera.ModulosMDB.FullName,
                        OleDbProvider = new AccessProvider(OledbProviders.Microsoft_ACE_OleDb_12),
                    };
                    new VoidAccess_Transaction<Object>(new AccessConnectionContent(aBuilder.Data).GenerateConnectionString(),
                        delegate (Access_Connector conn, Object[] trParameters)
                        {
                            Query_BasesMDB q = new Query_BasesMDB();
                            List<String> rows = conn.SelectRows(q.SelectClaveCantidad(this.listOfModulos.SelectedItem.ToString()), '@');
                            String[] cells;
                            int count;
                            foreach (String row in rows)
                            {
                                cells = row.Split('@');
                                this.Claves.Add(new Tuple<string, int>(cells[0], int.TryParse(cells[1], out count) ? count : 0));
                            }
                        }).Run();
                    this.table.Clear();
                    foreach (var clave in Claves.OrderBy(x => x.Item1))
                        table.AddItem(clave.Item1, clave.Item2, clave);
                }
                catch { }
            }
            else
                this.table.Clear();
        }

        private void table_Fill(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.Claves != null)
            {
                foreach (var clave in Claves.OrderBy(x => x.Item1))
                    table.AddItem(clave.Item1, clave.Item2, clave);
            }
        }

        private void DialogAction_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string name = (sender as Button).Name;
            if (name == this.button_Ok.Name && this.ModuleName != String.Empty)
            {
                this.Action = ModuleAction.Ok;
                this.Hide();
            }
            else if (name == this.button_Ok.Name && this.ModuleName == String.Empty)
                Dialog_MessageBox.Show("No existen módulos disponibles, revise las rutas de archivos de la aplicación.", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            else
            {
                this.Action = ModuleAction.Cancel;
                this.Close();
            }
        }
    }
}
