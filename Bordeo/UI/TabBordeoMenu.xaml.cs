using DaSoft.Riviera.Modulador.Bordeo.Model;
using DaSoft.Riviera.Modulador.Core.Model;
using DaSoft.Riviera.Modulador.Core.Runtime;
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
using static DaSoft.Riviera.Modulador.Bordeo.Controller.BordeoUtils;
using static DaSoft.Riviera.Modulador.Bordeo.Assets.Codes;
using DaSoft.Riviera.Modulador.Core.Model.UI;

namespace DaSoft.Riviera.Modulador.Bordeo.UI
{
    /// <summary>
    /// Interaction logic for CtrlBordeoMenuTab.xaml
    /// </summary>
    public partial class TabBordeoMenu : UserControl
    {
        public TabBordeoMenu()
        {
            InitializeComponent();
        }

        private void btnInsertModule_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnContinueInsert_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnInsert_Click(object sender, RoutedEventArgs e)
        {

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var db = GetDatabase();
                var sizes = db.Sizes[CODE_PANEL_RECTO].Sizes.Select(x=> x as PanelMeasure);
                var frentes = sizes.Select(x => new RivieraSizeItem() { Size = x.Frente, ItemName = x.Frente.Nominal.ToString() });
                frentes = frentes.Distinct();
                this.cboFronts.ItemsSource = frentes;
                if (this.cboFronts.Items.Count > 0)
                    this.cboFronts.SelectedIndex = 0;
            }
            catch (Exception exc)
            {
                App.Riviera.Log.AppendEntry(exc, null);
            }
        }

        private void btnSummonPanelEditor_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnSummonPanelClipboard_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnDeletePanel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnReportMamparaType_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnValidate_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnCreateModule_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnSwapRenderView_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
