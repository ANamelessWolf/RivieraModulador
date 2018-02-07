using DaSoft.Riviera.OldModulador.Model;
using System;
using System.Windows;
using System.Windows.Controls;

namespace DaSoft.Riviera.OldModulador.UI
{
    /// <summary>
    /// Interaction logic for Ctrl_PanelDataItem.xaml
    /// </summary>
    public partial class Ctrl_PanelDataItem : UserControl
    {
        /// <summary>
        /// Accede a la información de panel
        /// </summary>
        public PanelData Data;
        /// <summary>
        /// El código del panel seleccionado
        /// </summary>
        public String Code
        {
            get { return this.Data.Code; }
            set
            {
                this.textCode.Text = value;
                this.textJustCode.Text = value;
            }
        }
        /// <summary>
        /// Accede a la descripción del código
        /// </summary>
        public String Description
        {
            get { return this.Description; }
            set
            {
                this.textDescription.Text = value;
            }
        }
        /// <summary>
        /// Si esta activa esta opción el control solo muestra el código
        /// </summary>
        public Boolean? JustCode
        {
            get { return (Boolean?)GetValue(JustCodeProperty); }
            set { SetValue(JustCodeProperty, value); }
        }
        public static DependencyProperty JustCodeProperty;
        /// <summary>
        /// Static constructor
        /// </summary>
        static Ctrl_PanelDataItem()
        {
            JustCodeProperty = DependencyProperty.Register("JustCode", typeof(Boolean?), typeof(Ctrl_PanelDataItem), new FrameworkPropertyMetadata(default(Boolean?), FrameworkPropertyMetadataOptions.AffectsRender, JustCode_Changed));
        }
        /// <summary>
        /// Cambia el texto ingresado
        /// </summary>
        static void JustCode_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Ctrl_PanelDataItem ctrl = (Ctrl_PanelDataItem)sender;
            if ((Boolean)e.NewValue)
            {
                ctrl.textJustCode.Visibility = Visibility.Visible;
                ctrl.textCodeDesc.Visibility = Visibility.Collapsed;
            }
            else
            {
                ctrl.textJustCode.Visibility = Visibility.Collapsed;
                ctrl.textCodeDesc.Visibility = Visibility.Visible;
            }
        }
        /// <summary>
        /// Crea un nuevo elemento de panel data a partir de un panel
        /// data existente
        /// </summary>
        /// <param name="data">El panel data existente</param>
        public Ctrl_PanelDataItem(PanelData data)
        {
            this.Data = data;
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.Code = this.Data.Code;
            this.Description = this.Data.Description;
        }
    }
}
