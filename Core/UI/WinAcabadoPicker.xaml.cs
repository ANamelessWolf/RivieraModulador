using DaSoft.Riviera.Modulador.Core.Model;
using DaSoft.Riviera.Modulador.Core.Model.DB;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DaSoft.Riviera.Modulador.Core.UI
{
    /// <summary>
    /// Interaction logic for WinAcabadoPicker.xaml
    /// </summary>
    public partial class WinAcabadoPicker : MetroWindow
    {
        /// <summary>
        /// Devuelve el acabado seleccionado
        /// </summary>
        public RivieraAcabado SelectedAcabado
        {
            get { return this.listAcabados.SelectedIndex != -1 ? (RivieraAcabado)this.listAcabados.SelectedItem : default(RivieraAcabado); }
        }
        /// <summary>
        /// Lista de acabados para el código seleccionado
        /// </summary>
        public RivieraCode Acabados;
        /// <summary>
        /// El código seleccionado
        /// </summary>
        public String Code
        {
            get { return this.code.Text; }
            set { this.code.Text = value; }
        }
        String _code;
        /// <summary>
        /// The descriptions
        /// </summary>
        public List<String> Descriptions;
        /// <summary>
        /// Initializes a new instance of the <see cref="WinAcabadoPicker"/> class.
        /// </summary>
        public WinAcabadoPicker()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="WinAcabadoPicker"/> class.
        /// </summary>
        /// <param name="strCode">The string code.</param>
        /// <param name="acabados">The acabados.</param>
        public WinAcabadoPicker(String strCode, RivieraCode acabados)
            : this()
        {
            this._code = strCode;
            this.Descriptions = new List<String>();
            this.Acabados = acabados;
        }
        /// <summary>
        /// Handles the SelectionChanged event of the listAcabados control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void listAcabados_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.fieldDesc.Text = this.listAcabados.SelectedIndex != -1 ? ((RivieraAcabado)this.listAcabados.SelectedItem).Description : String.Empty;
        }
        /// <summary>
        /// Handles the Click event of the Ok control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
        /// <summary>
        /// Handles the Click event of the Cancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
        /// <summary>
        /// Handles the Loaded event of the MetroWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Code = _code;
            this.listAcabados.ItemsSource = Acabados;
            if (this.listAcabados.Items.Count == 0)
                this.listAcabados.SelectedIndex = 0;
        }
    }
}
