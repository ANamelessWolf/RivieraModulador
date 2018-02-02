using DaSoft.Riviera.Modulador.Core.Model;
using DaSoft.Riviera.Modulador.Core.Model.UI;
using DaSoft.Riviera.Modulador.Core.Runtime;
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
using static DaSoft.Riviera.Modulador.Core.Assets.CONST;
namespace DaSoft.Riviera.Modulador.Core.UI
{
    /// <summary>
    /// Lógica de interacción para WinSelectFront.xaml
    /// </summary>
    public partial class WinSelectFront : MetroWindow
    {
        /// <summary>
        /// The Riviera design database
        /// </summary>
        public RivieraDesignDatabase Database;
        /// <summary>
        /// The base code
        /// </summary>
        public String BaseCode;
        /// <summary>
        /// The base code
        /// </summary>
        private String DefaultFront;
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
        /// Initializes a new instance of the <see cref="WinSelectFront"/> class.
        /// </summary>
        /// <param name="db">The database.</param>
        /// <param name="code_base">The code base.</param>
        /// <param name="currentFront">The current front.</param>
        public WinSelectFront(RivieraDesignDatabase db, String code_base, String currentFront = "")
        {
            this.Database = db;
            this.BaseCode = code_base;
            this.DefaultFront = currentFront;
            InitializeComponent();
        }
        /// <summary>
        /// Handles the Loaded event of the MetroWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var dbSizes = this.Database.Sizes;
                var sizes = dbSizes[this.BaseCode].Sizes;
                var frentes = sizes.Select(x => new RivieraSizeItem() { Size = x[KEY_FRONT], ItemName = x[KEY_FRONT].Nominal.ToString() });
                frentes = frentes.Distinct();
                cboFronts.ItemsSource = frentes;
                if (cboFronts.Items.Count > 0 && this.DefaultFront == "")
                    cboFronts.SelectedIndex = 0;
                else if (cboFronts.Items.Count > 0 && this.DefaultFront != "")
                {
                    int index = cboFronts.ItemsSource.OfType<RivieraSizeItem>().Select(x => x.ItemName).ToList().IndexOf(this.DefaultFront);
                    cboFronts.SelectedIndex = index >= 0 ? index : 0;
                }
            }
            catch (Exception exc)
            {
                App.Riviera.Log.AppendEntry(exc, null);
            }
        }

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
    }
}
