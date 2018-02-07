using System;
using System.Windows;
using System.Windows.Controls;

namespace DaSoft.Riviera.OldModulador.UI.Delta
{
    /// <summary>
    /// Interaction logic for Ctrl_QuantifyTable.xaml
    /// </summary>
    public partial class Ctrl_QuantifyTable : UserControl
    {
        /// <summary>
        /// Realiza el llenado de la tabla
        /// </summary>
        public event RoutedEventHandler Fill;
        /// <summary>
        /// El control lista de la tabla
        /// </summary>
        public ListView List
        {
            get { return this.list; }
        }

        /// <summary>
        /// Cambia el texto del elemento
        /// </summary>
        public String ColumnOne
        {
            get { return (String)GetValue(ColumnOneProperty); }
            set { SetValue(ColumnOneProperty, value); }
        }
        /// <summary>
        /// Cambia el texto del elemento
        /// </summary>
        public String ColumnTwo
        {
            get { return (String)GetValue(ColumnTwoProperty); }
            set { SetValue(ColumnTwoProperty, value); }
        }

        public static DependencyProperty ColumnOneProperty;
        public static DependencyProperty ColumnTwoProperty;
        /// <summary>
        /// Static constructor
        /// </summary>
        static Ctrl_QuantifyTable()
        {
            ColumnOneProperty = DependencyProperty.Register("ColumnOneValue", typeof(String), typeof(Ctrl_QuantifyTable), new FrameworkPropertyMetadata("Columna 1", FrameworkPropertyMetadataOptions.AffectsRender, ColumnOne_Changed));
            ColumnTwoProperty = DependencyProperty.Register("ColumnTwoValue", typeof(String), typeof(Ctrl_QuantifyTable), new FrameworkPropertyMetadata("Columna 2", FrameworkPropertyMetadataOptions.AffectsRender, ColumnTwo_Changed));
        }

        /// <summary>
        /// Cambia el título de la primera columna
        /// </summary>
        static void ColumnOne_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Ctrl_QuantifyTable ctrl = (Ctrl_QuantifyTable)sender;
            ctrl.fieldColumnOne.Text = e.NewValue.ToString();
        }
        /// <summary>
        /// Cambia el título de la primera columna
        /// </summary>
        static void ColumnTwo_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Ctrl_QuantifyTable ctrl = (Ctrl_QuantifyTable)sender;
            ctrl.fieldColumnTwo.Text = e.NewValue.ToString();
        }

        public Ctrl_QuantifyTable()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Agregá un elemento a la lista
        /// </summary>
        /// <param name="item1">El primer elemento de la fila</param>
        /// <param name="item2">El segundo  elemento de la fila</param>
        /// <param name="data">La información del objeto</param>
        public void AddItem(String item1, int item2, Object data)
        {
            int index = this.list.Items.Count;
            Ctrl_QuantifyItem qItem = new Ctrl_QuantifyItem() { Text = item1, Total = item2, Data = data, Index = index };
            this.list.Items.Add(qItem);
        }
        /// <summary>
        /// Limpia la información de la tabla
        /// </summary>
        public void Clear()
        {
            this.list.Items.Clear();
        }
        /// <summary>
        /// Llena la lista 
        /// </summary>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.list.Items.Clear();
            if (this.Fill != null)
                this.Fill(this, e);
        }



    }
}
