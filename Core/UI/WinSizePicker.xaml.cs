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
    /// Interaction logic for WinSizePicker.xaml
    /// </summary>
    public partial class WinSizePicker : MetroWindow
    {
        /// <summary>
        /// Gets the selected start front.
        /// </summary>
        /// <value>
        /// The selected start front.
        /// </value>
        public RivieraSize SelectedSize
        {
            get
            {
                if (this.cboSizes.SelectedIndex != -1)
                    return (this.cboSizes.SelectedItem as RivieraSizeItem).Size;
                else
                    return default(RivieraSize);
            }
        }



        IEnumerable<RivieraSize> _Frentes;
        String DefaultSize;
        String TagName;
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
            this.cboSizes.ItemsSource = _Frentes.Select(x => new RivieraSizeItem()
            {
                Size = x,
                ItemName = x.Nominal.ToString()
            });
            int index = this.cboSizes.ItemsSource.OfType<RivieraSizeItem>().Select(x => x.ItemName).ToList().IndexOf(DefaultSize);
            if (index > 0)
                this.cboSizes.SelectedIndex = index;
            else
                this.cboSizes.SelectedIndex = 0;
            this.sizeTag.Text = this.TagName;
        }

        public WinSizePicker(IEnumerable<RivieraSize> frentes, String sizeTag = "Frente", String defaultSize = "24")
        {
            this._Frentes = frentes;
            this.TagName = sizeTag;
            this.DefaultSize = defaultSize;
            InitializeComponent();
        }

    }
}
