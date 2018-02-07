using DaSoft.Riviera.OldModulador.Model;
using DaSoft.Riviera.OldModulador.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DaSoft.Riviera.OldModulador.UI
{
    /// <summary>
    /// Interaction logic for Dialog_PanSelector.xaml
    /// </summary>
    public partial class Dialog_AcabadoSelector : Window
    {
        /// <summary>
        /// Devuelve el acabado seleccionado
        /// </summary>
        public String SelectedAcabado
        {
            get { return this.listAcabados.SelectedIndex != -1 ? this.listAcabados.SelectedItem.ToString() : String.Empty; }
        }
        /// <summary>
        /// Lista de acabados para el código seleccionado
        /// </summary>
        public RivieraAcabado Acabados;
        /// <summary>
        /// El código seleccionado
        /// </summary>
        public String Code
        {
            get { return this.code.Text; }
            set { this.code.Text = value; }
        }
        String _code;
        public List<String> Descriptions;

        public Dialog_AcabadoSelector()
        {
          
            InitializeComponent();
        }
        public Dialog_AcabadoSelector(String strCode) : this()
        {
            this._code = strCode;
            this.Descriptions = new List<String>();
            this.Acabados = App.DB.Acabados.Where(x => x.Code == this._code).FirstOrDefault();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Code = _code;
            
            if (this.Acabados != null)
            {
                foreach (Tuple<String, String> t in this.Acabados.Acabados)
                {
                    this.listAcabados.Items.Add(t.Item1);
                    this.Descriptions.Add(t.Item2);
                }
                this.listAcabados.SelectedIndex = 0;
            }
            else
                this.btnOk.Visibility = Visibility.Hidden;
        }

        private void listAcabados_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.listAcabados.SelectedIndex != -1)
                this.fieldDesc.Text = this.Descriptions[this.listAcabados.SelectedIndex];
            else
                this.fieldDesc.Text = String.Empty;
        }
    }
}
