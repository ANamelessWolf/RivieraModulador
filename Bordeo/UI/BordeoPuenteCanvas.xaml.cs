using DaSoft.Riviera.Modulador.Bordeo.Controller;
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
namespace DaSoft.Riviera.Modulador.Bordeo.UI
{
    /// <summary>
    /// Interaction logic for BordeoPuenteCanvas.xaml
    /// </summary>
    public partial class BordeoPuenteCanvas : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BordeoPuenteCanvas"/> class.
        /// </summary>
        public BordeoPuenteCanvas()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Handles the Loaded event of the UserControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.Hide();
            this.SetDefault();
        }
        /// <summary>
        /// Sets the default.
        /// </summary>
        private void SetDefault()
        {
            this.items.Children.OfType<FrameworkElement>().
                Where(x => x is BordeoPuenteItem || x is BordeoPuenteHorItem).
                Select(x => x as IBridgeItem).ToList().
                        ForEach(x =>
                        {
                            x.UpdateSize(KEY_FRONT, 24d);
                            x.UpdateSize(KEY_HEIGHT, 24d);
                            x.UpdateSize(KEY_DEPHT, 18d);
                            x.SetAcabado("01");
                        });
            this.items.Children.OfType<FrameworkElement>().
                Where(x => x is BordeoPazLuzItem).
                Select(x => x as IBridgeItem).ToList().
                ForEach(x =>
                {
                    x.UpdateSize(KEY_FRONT, 24d);
                    x.UpdateSize(KEY_DEPHT, 48d);
                    x.SetAcabado("01");
                });
        }
        /// <summary>
        /// Hides this instance.
        /// </summary>
        private void Hide()
        {
            this.b0TopSelected.Visibility = Visibility.Collapsed;
            this.b1TopSelected.Visibility = Visibility.Collapsed;
            this.b2TopSelected.Visibility = Visibility.Collapsed;
            this.b0LowSelected.Visibility = Visibility.Collapsed;
            this.b1LowSelected.Visibility = Visibility.Collapsed;
            this.b2LowSelected.Visibility = Visibility.Collapsed;
            this.b0Low.Visibility = Visibility.Collapsed;
            this.b1Low.Visibility = Visibility.Collapsed;
            this.b2Low.Visibility = Visibility.Collapsed;
            this.b0Top.Visibility = Visibility.Collapsed;
            this.b1Top.Visibility = Visibility.Collapsed;
            this.b2Top.Visibility = Visibility.Collapsed;
            this.p0Luz.Visibility = Visibility.Collapsed;
            this.p1Luz.Visibility = Visibility.Collapsed;
            this.p2Luz.Visibility = Visibility.Collapsed;
            this.bm0Start.Visibility = Visibility.Collapsed;
            this.bm0End.Visibility = Visibility.Collapsed;
            this.bm1Start.Visibility = Visibility.Collapsed;
            this.bm1End.Visibility = Visibility.Collapsed;
        }
    }
}
