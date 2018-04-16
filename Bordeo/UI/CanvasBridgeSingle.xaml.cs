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
    /// Interaction logic for CanvasBridgeSingle.xaml
    /// </summary>
    public partial class CanvasBridgeSingle : UserControl, ICanvasBridge
    {
        /// <summary>
        /// Occurs when [middle bridge visibility changed].
        /// </summary>
        public event DependencyPropertyChangedEventHandler MiddleBridgeVisibilityChanged;
        /// <summary>
        /// Occurs when [middle bridge visibility changed].
        /// </summary>
        public event RoutedEventHandler CanvasIsLoaded;
        /// <summary>
        /// Gets the canvas container.
        /// </summary>
        /// <value>
        /// The canvas container.
        /// </value>
        public Grid CanvasContainer => this.items;
        /// <summary>
        /// Gets the pazo luz.
        /// </summary>
        /// <value>
        /// The pazo luz.
        /// </value>
        public BordeoPazLuzItem PazoLuz => this.pazoLuz;
        /// <summary>
        /// Gets the pazo luz.
        /// </summary>
        /// <value>
        /// The pazo luz.
        /// </value>
        public BordeoPuenteHorItem HorBridge => this.bmiddle;
        /// <summary>
        /// Initializes a new instance of the <see cref="CanvasBridgeSingle"/> class.
        /// </summary>
        public CanvasBridgeSingle()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Hides the pazo luz.
        /// </summary>
        public void HidePazoLuz()
        {
            this.HideCanvasBridgePazoLuz();
        }
        /// <summary>
        /// Shows the pazo luz.
        /// </summary>
        public void ShowPazoLuz()
        {
            this.ShowCanvasBridgePazoLuz();
        }
        /// <summary>
        /// Sets the default.
        /// </summary>
        public void SetDefault()
        {
            this.SetCanvasBridgeDefault();
        }
        /// <summary>
        /// Handles the Loaded event of the UserControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.LoadCanvasBridge();
            if (CanvasIsLoaded != null)
                CanvasIsLoaded(this, e);
        }
        /// <summary>
        /// Gets the fondo.
        /// </summary>
        /// <returns>The total fondo</returns>
        public double GetFondo()
        {
            return this.b0Top.Frente;
        }
        /// <summary>
        /// Handles the IsVisibleChanged event of the bmiddle control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void bmiddle_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.MiddleBridgeVisibilityChanged != null)
                MiddleBridgeVisibilityChanged(this.bmiddle, e);
        }
    }
}