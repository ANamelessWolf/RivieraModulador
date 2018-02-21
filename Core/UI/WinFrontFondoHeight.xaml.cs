using DaSoft.Riviera.Modulador.Core.Model;
using DaSoft.Riviera.Modulador.Core.Model.UI;
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
    /// Interaction logic for WinFrontFondoHeight.xaml
    /// </summary>
    public partial class WinFrontFondoHeight : MetroWindow
    {
        /// <summary>
        /// Gets the selected start front.
        /// </summary>
        /// <value>
        /// The selected start front.
        /// </value>
        public RivieraSize SelectedFront
        {
            get
            {
                if (this.cboFronts.SelectedIndex != -1)
                    return (this.cboFronts.SelectedItem as RivieraSizeItem).Size;
                else
                    return default(RivieraSize);
            }
        }
        /// <summary>
        /// Gets the selected height.
        /// </summary>
        /// <value>
        /// The selected start height.
        /// </value>
        public RivieraSize SelectedHeight
        {
            get
            {
                if (this.cboHeight.SelectedIndex != -1)
                    return (this.cboHeight.SelectedItem as RivieraSizeItem).Size;
                else
                    return default(RivieraSize);
            }
        }
        /// <summary>
        /// Gets the selected fondo.
        /// </summary>
        /// <value>
        /// The selected start fondo.
        /// </value>
        public RivieraSize SelectedFondo
        {
            get
            {
                if (this.cboFondo.SelectedIndex != -1)
                    return (this.cboFondo.SelectedItem as RivieraSizeItem).Size;
                else
                    return default(RivieraSize);
            }
        }
        IEnumerable<RivieraSize> _Frentes, _Alturas, _Fondos;

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.cboFronts.ItemsSource = _Frentes;
            this.cboHeight.ItemsSource = _Alturas;
            this.cboFondo.ItemsSource = _Fondos;
        }

        public WinFrontFondoHeight(IEnumerable<RivieraSize> frentes, IEnumerable<RivieraSize> alturas, IEnumerable<RivieraSize> fondos)
        {
            this._Frentes = frentes;
            this._Alturas = alturas;
            this._Fondos = fondos;
            InitializeComponent();
        }
    }
}
