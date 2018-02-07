using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DaSoft.Riviera.OldModulador.UI.Delta
{
    /// <summary>
    /// Interaction logic for Ctrl_QuantifyItem.xaml
    /// </summary>
    public partial class Ctrl_QuantifyItem : UserControl
    {
        /// <summary>
        /// Cambia el indice del elemento
        /// </summary>
        public int Index
        {
            get { return (int)GetValue(IndexProperty); }
            set { SetValue(IndexProperty, value); }
        }
        /// <summary>
        /// Cambia el total del elemento
        /// </summary>
        public int Total
        {
            get { return (int)GetValue(TotalProperty); }
            set { SetValue(TotalProperty, value); }
        }
        /// <summary>
        /// Cambia el texto del elemento
        /// </summary>
        public String Text
        {
            get { return (String)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// La información guardada en la fila
        /// </summary>
        public Object Data;

        public static DependencyProperty TextProperty;
        public static DependencyProperty TotalProperty;
        public static DependencyProperty IndexProperty;
        /// <summary>
        /// Static constructor
        /// </summary>
        static Ctrl_QuantifyItem()
        {
            TextProperty = DependencyProperty.Register("TextValue", typeof(String), typeof(Ctrl_QuantifyItem), new FrameworkPropertyMetadata(String.Empty, FrameworkPropertyMetadataOptions.AffectsRender, Text_Changed));
            TotalProperty = DependencyProperty.Register("TotalValue", typeof(int), typeof(Ctrl_QuantifyItem), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsRender, Count_Changed));
            IndexProperty = DependencyProperty.Register("IndexValue", typeof(int), typeof(Ctrl_QuantifyItem), new FrameworkPropertyMetadata(-1, FrameworkPropertyMetadataOptions.AffectsRender, Index_Changed));
        }
        /// <summary>
        /// Cambia el texto ingresado
        /// </summary>
        static void Text_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Ctrl_QuantifyItem ctrl = (Ctrl_QuantifyItem)sender;
            ctrl.field_desc.Text = e.NewValue.ToString();
        }
        /// <summary>
        /// Cambia el total ingresado
        /// </summary>
        static void Count_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Ctrl_QuantifyItem ctrl = (Ctrl_QuantifyItem)sender;
            ctrl.field_count.Text = e.NewValue.ToString();
        }
        /// <summary>
        /// Cambia el indice ingresado y el color de la fila
        /// </summary>
        static void Index_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Ctrl_QuantifyItem ctrl = (Ctrl_QuantifyItem)sender;
            int index = (int)e.NewValue;
            if (index % 2 == 0)
                ctrl.itemRow.Background = new SolidColorBrush(Color.FromArgb(255, 246, 231, 210));
            else
                ctrl.itemRow.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
        }


        public Ctrl_QuantifyItem()
        {
            InitializeComponent();
        }
    }
}
