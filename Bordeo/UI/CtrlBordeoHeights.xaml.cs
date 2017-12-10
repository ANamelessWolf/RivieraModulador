using DaSoft.Riviera.Modulador.Bordeo.Model;
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

namespace DaSoft.Riviera.Modulador.Bordeo.UI
{
    /// <summary>
    /// Lógica de interacción para CtrlBordeoHeights.xaml
    /// </summary>
    public partial class CtrlBordeoHeights : UserControl
    {
        /// <summary>
        /// Gets the selected height for the panel.
        /// </summary>
        /// <value>
        /// The selected height panel.
        /// </value>
        public BordeoPanelHeight SelectedHeight
        {
            get
            {
                Image img = this.lvPanelHeights.SelectedIndex != -1 ? this.lvPanelHeights.SelectedItem as Image : null;
                if (img != null)
                    return (BordeoPanelHeight)int.Parse(img.Tag.ToString());
                else
                    return BordeoPanelHeight.None;
            }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="CtrlBordeoHeights"/> class.
        /// </summary>
        public CtrlBordeoHeights()
        {
            InitializeComponent();
        }
    }
}
