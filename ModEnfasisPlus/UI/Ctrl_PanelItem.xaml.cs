using DaSoft.Riviera.OldModulador.Controller;
using DaSoft.Riviera.OldModulador.Model;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DaSoft.Riviera.OldModulador.UI
{
    /// <summary>
    /// Interaction logic for PanelItem.xaml
    /// </summary>
    public partial class Ctrl_PanelItem : UserControl
    {
        /// <summary>
        /// La información guardada en el panel
        /// </summary>
        public Object Data;
        /// <summary>
        /// El evento al momento de dar click en una etiqueta
        /// </summary>

        public event MouseButtonEventHandler Click;
        /// <summary>
        /// El id asignado al panel actual
        /// </summary>
        public int Id;


        /// <summary>
        /// Establece el estatus del panel actual
        /// </summary>
        public PanelStatus Status
        {
            get
            {
                return (PanelStatus)GetValue(PanelStatusProperty);
            }
            set
            {
                SetValue(PanelStatusProperty, value);
            }
        }
        /// <summary>
        /// El ancho del panel
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
        /// El alto del panel
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
        /// El texto del código que aparece en el panel
        /// </summary>
        public String Text
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


        public static DependencyProperty PanelStatusProperty;
        public static DependencyProperty CodeProperty;
        public new static DependencyProperty WidthProperty;
        public new static DependencyProperty HeightProperty;
        public string Acabado;

        public PanelSide Side;

        /// <summary>
        /// Constructor estatico
        /// </summary>
        static Ctrl_PanelItem()
        {
            PanelStatusProperty = DependencyProperty.Register("PanelStatusValue", typeof(PanelStatus), typeof(Ctrl_PanelItem),
                new FrameworkPropertyMetadata(PanelStatus.None, FrameworkPropertyMetadataOptions.AffectsRender, PanelStatus_Changed));
            CodeProperty = DependencyProperty.Register("CodeValue", typeof(String), typeof(Ctrl_PanelItem),
                new FrameworkPropertyMetadata(String.Empty, FrameworkPropertyMetadataOptions.AffectsRender, Code_Changed));
            WidthProperty = DependencyProperty.Register("WidthValue", typeof(Double), typeof(Ctrl_PanelItem),
                new FrameworkPropertyMetadata(200d, FrameworkPropertyMetadataOptions.AffectsRender, Width_Changed));
            HeightProperty = DependencyProperty.Register("HeightValue", typeof(Double), typeof(Ctrl_PanelItem),
                new FrameworkPropertyMetadata(40d, FrameworkPropertyMetadataOptions.AffectsRender, HeightStatus_Changed));
        }
        /// <summary>
        /// Ejecuta un cambio en la visibilidad del estado del panel
        /// </summary>
        private static void PanelStatus_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Ctrl_PanelItem ctr = sender as Ctrl_PanelItem;
            PanelStatus res = (PanelStatus)e.NewValue;
            Brush brush;
            if (res == PanelStatus.Ok)
                brush = new SolidColorBrush(Color.FromArgb(127, 0, 255, 0));
            else if (res == PanelStatus.Error)
                brush = new SolidColorBrush(Color.FromArgb(127, 255, 0, 0));
            else if (res == PanelStatus.Selected)
                brush = new SolidColorBrush(Color.FromArgb(127, 237, 255, 54));
            else
                brush = new SolidColorBrush(Color.FromArgb(51, 225, 237, 247));
            ctr.panelArea.Background = brush;
        }
        private static void Code_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Ctrl_PanelItem ctr = sender as Ctrl_PanelItem;
            String res = (String)e.NewValue;
            ctr.codePanel.Text = res;
        }
        private static void Width_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Ctrl_PanelItem ctr = sender as Ctrl_PanelItem;
            Double res = (Double)e.NewValue;
            ctr.panelArea.Width = res;
        }
        private static void HeightStatus_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Ctrl_PanelItem ctr = sender as Ctrl_PanelItem;
            Double res = (Double)e.NewValue;
            ctr.panelArea.Height = res;
        }
        public Ctrl_PanelItem()
        {
            InitializeComponent();
        }

        private void panelArea_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Click != null)
                this.Click(this, e);
        }

        internal void SetAcabado()
        {
            String acabado, code;
            if (this.SelectAcabado(out acabado, out code))
            {
                this.Acabado = acabado;
                this.codePanel.Text = code;
            }
        }
    }
}
