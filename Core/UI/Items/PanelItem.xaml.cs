using DaSoft.Riviera.Modulador.Core.Model;
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

namespace DaSoft.Riviera.Modulador.Core.UI.Items
{
    /// <summary>
    /// Interaction logic for PanelItem.xaml
    /// </summary>
    /// <seealso cref="System.Windows.Controls.UserControl" />
    public partial class PanelItem : UserControl
    {
        /// <summary>
        /// The panel asociated data
        /// </summary>
        public Object Data;
        /// <summary>
        /// Occurs when the panel [click].
        /// </summary>
        public event MouseButtonEventHandler Click;
        /// <summary>
        /// Gets or sets the panel status.
        /// </summary>
        /// <value>
        /// The panel status.
        /// </value>
        public PanelItemStatus PanelStatus
        {
            get
            {
                return (PanelItemStatus)GetValue(PanelStatusProperty);
            }
            set
            {
                SetValue(PanelStatusProperty, value);
            }
        }
        /// <summary>
        /// Gets the panel status.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>The panel item current status</returns>
        public static PanelItemStatus GetPanelStatus(PanelItem target)
        {
            return target.PanelStatus;
        }
        /// <summary>
        /// Gets the panel status.
        /// </summary>
        /// <param name="target">The target.</param>
        public static void SetPanelStatus(PanelItem target, PanelItemStatus value)
        {
            target.PanelStatus = value;
        }
        /// <summary>
        /// The panel width
        /// </summary>
        public new Double Width
        {
            get
            {
                return (Double)GetValue(WidthProperty);
            }
            set
            {
                SetValue(WidthProperty, value);
            }
        }
        /// <summary>
        /// The panel height
        /// </summary>
        public new Double Height
        {
            get
            {
                return (Double)GetValue(HeightProperty);
            }
            set
            {
                SetValue(HeightProperty, value);
            }
        }
        /// <summary>
        /// Gets or sets the panel code.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        public String Code
        {
            get
            {
                return (String)GetValue(CodeProperty);
            }
            set
            {
                SetValue(CodeProperty, value);
            }
        }
        /// <summary>
        /// Gets the code.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        public static String GetCode(PanelItem target)
        {
            return target.Code;
        }
        /// <summary>
        /// Sets the code.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="value">The value.</param>
        public static void SetCode(PanelItem target, String value)
        {
            target.Code = value;
        }
        /// <summary>
        /// El texto del código asignado al acabado
        /// </summary>
        public String Acabado
        {
            get
            {
                return (String)GetValue(AcabadoProperty);
            }
            set
            {
                SetValue(AcabadoProperty, value);
            }
        }
        /// <summary>
        /// Gets the acabado.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        public static String GetAcabado(PanelItem target)
        {
            return target.Acabado;
        }
        /// <summary>
        /// Sets the acabado.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="value">The value.</param>
        public static void SetAcabado(PanelItem target, String value)
        {
            target.Code = value;
        }
        /// <summary>
        /// The side
        /// </summary>
        public PanelSide Side;
        /// <summary>
        /// Gets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        public String Text
        {
            get
            {
                return this.codePanel.Text;
            }
        }

        public static DependencyProperty PanelStatusProperty;
        public static DependencyProperty CodeProperty;
        public static DependencyProperty AcabadoProperty;
        public new static DependencyProperty WidthProperty;
        public new static DependencyProperty HeightProperty;
        /// <summary>
        /// Constructor estatico
        /// </summary>
        static PanelItem()
        {
            PanelStatusProperty = DependencyProperty.Register("PanelStatus", typeof(PanelItemStatus), typeof(PanelItem),
                new FrameworkPropertyMetadata(PanelItemStatus.Initial, FrameworkPropertyMetadataOptions.AffectsRender, PanelStatus_Changed));
            CodeProperty = DependencyProperty.Register("Code", typeof(String), typeof(PanelItem),
                new FrameworkPropertyMetadata(String.Empty, FrameworkPropertyMetadataOptions.AffectsRender, Code_Changed));
            AcabadoProperty = DependencyProperty.Register("Acabado", typeof(String), typeof(PanelItem),
                new FrameworkPropertyMetadata(String.Empty, FrameworkPropertyMetadataOptions.AffectsRender, Acabado_Changed));
            WidthProperty = DependencyProperty.Register("WidthValue", typeof(Double), typeof(PanelItem),
                new FrameworkPropertyMetadata(default(double), FrameworkPropertyMetadataOptions.AffectsRender, Width_Changed));
            HeightProperty = DependencyProperty.Register("HeightValue", typeof(Double), typeof(PanelItem),
                new FrameworkPropertyMetadata(default(double), FrameworkPropertyMetadataOptions.AffectsRender, HeightStatus_Changed));
        }
        /// <summary>
        /// Panels the status changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void PanelStatus_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            PanelItem ctr = sender as PanelItem;
            PanelItemStatus res = (PanelItemStatus)e.NewValue;
            Brush brush;
            if (res == PanelItemStatus.Ok)
                brush = new SolidColorBrush(Color.FromArgb(127, 0, 255, 0));
            else if (res == PanelItemStatus.Error)
                brush = new SolidColorBrush(Color.FromArgb(127, 255, 0, 0));
            else if (res == PanelItemStatus.Selected)
                brush = new SolidColorBrush(Color.FromArgb(127, 237, 255, 54));
            else
                brush = new SolidColorBrush(Color.FromArgb(51, 225, 237, 247));
            ctr.panelArea.Background = brush;
        }
        /// <summary>
        /// Codes the changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void Code_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            PanelItem ctr = sender as PanelItem;
            String res = (String)e.NewValue;
            ctr.codePanel.Text = res + (ctr.Acabado != null ? ctr.Acabado:String.Empty);
        }
        /// <summary>
        /// Acabadoes the changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void Acabado_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            PanelItem ctr = sender as PanelItem;
            String res = (String)e.NewValue;
            ctr.codePanel.Text = ctr.Code != null ? ctr.Code : String.Empty + res;
        }
        /// <summary>
        /// Widthes the changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void Width_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            PanelItem ctr = sender as PanelItem;
            Double res = (Double)e.NewValue;
            ctr.panelArea.Width = res;
        }
        /// <summary>
        /// Heights the status changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void HeightStatus_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            PanelItem ctr = sender as PanelItem;
            Double res = (Double)e.NewValue;
            ctr.panelArea.Height = res;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="PanelItem"/> class.
        /// </summary>
        public PanelItem()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Handles the MouseLeftButtonUp event of the panelArea control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void panelArea_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Click != null)
                this.Click(this, e);
        }
    }
}
