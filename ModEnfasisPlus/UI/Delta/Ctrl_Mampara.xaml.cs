using DaSoft.Riviera.OldModulador.Controller;
using DaSoft.Riviera.OldModulador.Model;
using DaSoft.Riviera.OldModulador.Model.Delta;
using DaSoft.Riviera.OldModulador.Runtime;
using NamelessOld.Libraries.HoukagoTeaTime.Yui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using static DaSoft.Riviera.OldModulador.Assets.Strings;
using static DaSoft.Riviera.OldModulador.Runtime.CMD;
namespace DaSoft.Riviera.OldModulador.UI.Delta
{
    /// <summary>
    /// Interaction logic for Ctrl_Mampara.xaml
    /// </summary>
    public partial class Ctrl_Mampara : UserControl
    {
        public static Mampara MamparaTemplate;
        public static List<PanelRaw> StackPanelTemplate;
        public static PanelRaw BiomboTemplate;

        RivieraObject obj;
        public RivieraObject CurrentObject
        {
            get
            {
                return obj;
            }
            set
            {
                obj = value;
                if (value != null)
                    snoopBox.Text =
                        String.Format(
                            "Tipo: {0}\n" +
                            "Handle: {1}\n" +
                            "Código: {2}\n" +
                            "Padre: {3}\n" +
                            "Ubicación: [{4} - {5}]\n" +
                                 "{6}" +
                                 "Geometría: {7}",
                            value.GetType().Name,
                            value.Line.Handle.Value,
                            value.Code,
                            value.Parent,
                            value.Start, value.End,
                            ListSons(value),
                            value.Ids[1].ToString()
                            );
            }
        }

        private String ListSons(RivieraObject value)
        {
            StringBuilder sb = new StringBuilder();
            String[] fields = new String[]
            {   FIELD_LEFT_FRONT.ToHumanReadable(),
                FIELD_FRONT.ToHumanReadable(),
                FIELD_RIGHT_FRONT.ToHumanReadable(),
                FIELD_BACK.ToHumanReadable(),
                FIELD_LEFT_BACK.ToHumanReadable(),
                FIELD_RIGHT_BACK.ToHumanReadable()
             };
            long[] Ids = new long[]
            {
                value.Children[FIELD_LEFT_FRONT],
                value.Children[FIELD_FRONT],
                value.Children[FIELD_RIGHT_FRONT],
                value.Children[FIELD_BACK],
                value.Children[FIELD_LEFT_BACK],
                value.Children[FIELD_RIGHT_BACK],
            };
            for (int i = 0; i < fields.Length; i++)
                if (Ids[i] != 0)
                    sb.Append(String.Format("{0}: {1}\n", fields[i], Ids[i]));
            return sb.ToString();
        }

        /// <summary>
        /// El tamaño seleccionado de la mampara.
        /// </summary>
        public RivieraSize Size
        {
            get
            {
                return new RivieraSize()
                {
                    Alto = (this.listOfAltura.SelectedItem as MamparaSize).Real.Alto.ToInternalUnits(Unit_Type.mm),
                    Frente = (this.listOfAltura.SelectedItem as MamparaSize).Real.Frente.ToInternalUnits(Unit_Type.mm),
                    Ancho = new MamparaSize().Real.Ancho.ToInternalUnits(Unit_Type.mm)
                };
            }
        }
        /// <summary>
        /// Las unidades que usa la aplicación
        /// </summary>
        public DaNTeUnits Units
        {
            get
            {
                return this.unitType.SelectedIndex == 0 ? DaNTeUnits.Metric : DaNTeUnits.Imperial;
            }
            set
            {
                this.unitType.SelectedIndex = App.Riviera.Units == DaNTeUnits.Metric ? 0 : 1;
            }
        }
        /// <summary>
        /// El código de la mampara seleccionada
        /// </summary>
        public String Code
        {
            get
            {
                return (this.listOfAltura.SelectedItem as MamparaSize).CodeFormatted;
            }
        }

        public Ctrl_Mampara()
        {
            InitializeComponent();
        }

        private void Frente_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.listOfAltura.Items.Clear();
            if (this.listOfFrentes.SelectedIndex != -1)
            {
                MamparaSize frente = this.listOfFrentes.SelectedItem as MamparaSize;
                foreach (var mAlt in App.DB.Mampara_Sizes.Where(x => frente.Nominal.Frente == x.Nominal.Frente).GroupBy(x => x.Alto))
                    this.listOfAltura.Items.Add(mAlt.FirstOrDefault());
            }
            this.listOfAltura.SelectedIndex = 0;
        }

        private void Insert_Click(object sender, RoutedEventArgs e)
        {
            Selector.InvokeCMD(DELTA_INSERT);

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.listOfFrentes.Items.Clear();
            foreach (var mFre in App.DB.Mampara_Sizes.GroupBy(x => x.Frente))
                this.listOfFrentes.Items.Add(mFre.FirstOrDefault());
            this.listOfFrentes.SelectedIndex = 0;
            this.useArneses.IsChecked = App.Riviera.IsArnesEnabled;
            this.Units = App.Riviera.Units;
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {

            Selector.InvokeCMD(DELTA_REMOVE);
        }

        private void Continue_Click(object sender, RoutedEventArgs e)
        {
            Selector.InvokeCMD(DELTA_CONTINUE_INSERT);
        }

        private void ChangeElement_Click(object sender, RoutedEventArgs e)
        {
            Selector.InvokeCMD(DELTA_CHANGE_MAMPARA);
        }

        private void InspectElement_Click(object sender, RoutedEventArgs e)
        {
            Selector.InvokeCMD(DELTA_SNOOP_MAMPARA);
        }

        private void Quantify_Click(object sender, RoutedEventArgs e)
        {
            Selector.InvokeCMD(DELTA_QUANTIFY);
        }

        private void Panel_Click(object sender, RoutedEventArgs e)
        {
            Selector.InvokeCMD(DELTA_EDIT_PANELS);
        }

        private void PanelCopyPaste_Click(object sender, RoutedEventArgs e)
        {
            Selector.InvokeCMD(DELTA_COPY_PASTE_PANELS);
        }

        private void AppendStyle_Click(object sender, RoutedEventArgs e)
        {
            Selector.InvokeCMD(DELTA_CRETE_PANEL_TEMPLATE);
        }

        private void InsertT(object sender, RoutedEventArgs e)
        {
            Selector.InvokeCMD(DELTA_INSERT_I);
        }

        private void SwapView_Click(object sender, RoutedEventArgs e)
        {
            Selector.InvokeCMD(SWAP_3D_MODE);
        }

        private void Modulo_Click(object sender, RoutedEventArgs e)
        {
            Selector.InvokeCMD(MODULE_CREATOR);
        }

        private void InsertModule_Click(object sender, RoutedEventArgs e)
        {
            Selector.InvokeCMD(INSERT_MODULE);
        }

        private void arnesesChecked(object sender, RoutedEventArgs e)
        {
            App.Riviera.IsArnesEnabled = ((CheckBox)sender).IsChecked.Value;
        }

        private void UpdatesId_Click(object sender, RoutedEventArgs e)
        {
            Selector.InvokeCMD(REGEN_IDS);
        }

        private void unitType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.Units != App.Riviera.Units)
                App.Riviera.Units = this.Units;
        }

        private void CreatesReport_Click(object sender, RoutedEventArgs e)
        {
            Selector.InvokeCMD(DELTA_MAMPARA_REPORT_TIPO);
        }

        private void PanelDouble_Click(object sender, RoutedEventArgs e)
        {
            Selector.InvokeCMD(DELTA_EDIT_PANELS_DOUBLE);
        }

        private void FlipMampara54_Click(object sender, RoutedEventArgs e)
        {
            Selector.InvokeCMD(DELTA_FLIP_54_MAMPARA);
        }
    }
}
