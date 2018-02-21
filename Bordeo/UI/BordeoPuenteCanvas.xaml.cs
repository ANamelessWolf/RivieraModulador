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
    /// Interaction logic for BordeoPuenteCanvas.xaml
    /// </summary>
    public partial class BordeoPuenteCanvas : UserControl
    {
        public BordeoPuenteCanvas()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.Hide();
         
        }
        /// <summary>
        /// Hides this instance.
        /// </summary>
        private void Hide()
        {
            this.b0TopSelected.Visibility = Visibility.Hidden;
            this.b1TopSelected.Visibility = Visibility.Hidden;
            this.b2TopSelected.Visibility = Visibility.Hidden;
            this.b0LowSelected.Visibility = Visibility.Hidden;
            this.b1LowSelected.Visibility = Visibility.Hidden;
            this.b2LowSelected.Visibility = Visibility.Hidden;
            this.b0Low.Visibility = Visibility.Hidden;
            this.b1Low.Visibility = Visibility.Hidden;
            this.b2Low.Visibility = Visibility.Hidden;
            this.b0Top.Visibility = Visibility.Hidden;
            this.b1Top.Visibility = Visibility.Hidden;
            this.b2Top.Visibility = Visibility.Hidden;
            this.p0Luz.Visibility = Visibility.Hidden;
            this.p1Luz.Visibility = Visibility.Hidden;
            this.p2Luz.Visibility = Visibility.Hidden;
            this.bm0Start.Visibility = Visibility.Hidden;
            this.bm0End.Visibility = Visibility.Hidden;
            
        }
    }
}
