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
using Nameless.Libraries.HoukagoTeaTime.Yui;

namespace DaSoft.Riviera.Modulador.Bordeo.UI
{
    /// <summary>
    /// Interaction logic for CtrlBordeoMenuTab.xaml
    /// </summary>
    public partial class TabBordeoMenu : UserControl
    {
        const String BORDEO_SOW_START = "BordeoSowStart";
        const String BORDEO_SOW_CONTINUE = "BordeoSowContinue";
        const String BORDEO_EDIT_PANELS = "BordeoPanelEditor";
        const String SWAP_3D_MODE = "Swap3DMode";
        const String BORDEO_PANEL_COPY_PASTE = "BordeoCopyPastePanel";
        /// <summary>
        /// Gets the riviera bordeo available sizes
        /// </summary>
        public Dictionary<String, ElementSizeCollection> Sizes;
        /// <summary>
        /// The Height for a panel of 27
        /// </summary>
        public RivieraSize PanelHeight27;
        /// <summary>
        /// The Height for a panel of 15
        /// </summary>
        public RivieraSize PanelHeight15;
        /// <summary>
        /// Gets the selected start front.
        /// </summary>
        /// <value>
        /// The selected start front.
        /// </value>
        public RivieraSize SelectedStartFront
        {
            get
            {
                if (this.cboStartFronts.SelectedIndex != -1)
                    return (this.cboStartFronts.SelectedItem as RivieraSizeItem).Size;
                else
                    return default(RivieraSize);
            }
        }
        /// <summary>
        /// Gets the selected end front.
        /// </summary>
        /// <value>
        /// The selected end front.
        /// </value>
        public RivieraSize SelectedEndFront
        {
            get
            {
                if (this.cboEndFronts.SelectedIndex != -1)
                    return (this.cboEndFronts.SelectedItem as RivieraSizeItem).Size;
                else
                    return default(RivieraSize);
            }
        }
        /// <summary>
        /// Gets the select heights.
        /// </summary>
        /// <value>
        /// The select heights.
        /// </value>
        public RivieraSize[] SelectHeights
        {
            get
            {
                return this.listHeights.SelectedHeight.GetHeights(this.PanelHeight27, this.PanelHeight15);
            }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="TabBordeoMenu"/> class.
        /// </summary>
        public TabBordeoMenu()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Gets the panels.
        /// </summary>
        /// <param name="code">The panel code.</param>
        /// <returns>The panel list</returns>
        public List<RivieraMeasure> GetPanels(String code)
        {
            return code.GetPanelMeasure(this.Sizes, this.SelectHeights, this.SelectedStartFront, this.SelectedEndFront);
        }
        /// <summary>
        /// Gets the linear panels.
        /// </summary>
        /// <returns>The Selected linear panels</returns>
        public IEnumerable<PanelMeasure> GetLinearPanels()
        {
            return this.GetPanels(CODE_PANEL_RECTO).Select(x => x as PanelMeasure);
        }
        /// <summary>
        /// Gets the L panels of 90°.
        /// </summary>
        /// <returns>The selected l panels</returns>
        public IEnumerable<LPanelMeasure> GetL90Panels()
        {
            return this.GetPanels(CODE_PANEL_90).Select(x => x as LPanelMeasure);
        }
        /// <summary>
        /// Gets the L panels of 135°.
        /// </summary>
        /// <returns>The selected l panels</returns>
        public IEnumerable<LPanelMeasure> GetL135Panels()
        {
            return this.GetPanels(CODE_PANEL_135).Select(x => x as LPanelMeasure);
        }


        private void btnInsertModule_Click(object sender, RoutedEventArgs e)
        {
            Selector.InvokeCMD(BORDEO_SOW_START);
        }

        private void btnContinueInsert_Click(object sender, RoutedEventArgs e)
        {
            Selector.InvokeCMD(BORDEO_SOW_CONTINUE);
        }

        private void btnInsert_Click(object sender, RoutedEventArgs e)
        {
            Selector.InvokeCMD(BORDEO_SOW_START);
        }



        private void btnSummonPanelEditor_Click(object sender, RoutedEventArgs e)
        {
            Selector.InvokeCMD(BORDEO_EDIT_PANELS);
        }

        private void btnSummonPanelClipboard_Click(object sender, RoutedEventArgs e)
        {
            Selector.InvokeCMD(BORDEO_PANEL_COPY_PASTE);
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
            Selector.InvokeCMD(SWAP_3D_MODE);
        }
        /// <summary>
        /// Handles the Loaded event of the UserControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var db = GetDatabase();
                this.Sizes = db.Sizes;
                var sizes = this.Sizes[CODE_PANEL_RECTO].Sizes.Select(x => x as PanelMeasure);
                var frentes = sizes.Select(x => new RivieraSizeItem() { Size = x.Frente, ItemName = x.Frente.Nominal.ToString() });
                frentes = frentes.Distinct();
                this.InitFrentes(this.cboStartFronts, frentes);
                this.InitFrentes(this.cboEndFronts, frentes);
                this.PanelHeight15 = sizes.FirstOrDefault(x => x.Alto.Nominal == 15d).Alto;
                this.PanelHeight27 = sizes.FirstOrDefault(x => x.Alto.Nominal == 27d).Alto;
            }
            catch (Exception exc)
            {
                App.Riviera.Log.AppendEntry(exc, null);
            }
        }
        /// <summary>
        /// Initializes the list of frentes.
        /// </summary>
        /// <param name="cbo">The combo box.</param>
        /// <param name="frentes">The list of frentes.</param>
        private void InitFrentes(ComboBox cbo, IEnumerable<RivieraSizeItem> frentes)
        {
            cbo.ItemsSource = frentes;
            if (cbo.Items.Count > 0)
                cbo.SelectedIndex = 0;
        }
    }
}
