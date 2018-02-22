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

namespace DaSoft.Riviera.Modulador.Bordeo.UI
{
    /// <summary>
    /// Interaction logic for BordeoPazLuzItem.xaml
    /// </summary>
    public partial class BordeoPazLuzItem : UserControl, IBridgeItem
    {
        /// <summary>
        /// The Bridge Front
        /// </summary>
        public Double Frente
        {
            get
            {
                return (Double)GetValue(FrenteProperty);
            }
            set
            {
                SetValue(FrenteProperty, value);
            }
        }
        /// <summary>
        /// Gets the Frente.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        public static Double GetFrente(BordeoPazLuzItem target)
        {
            return target.Frente;
        }
        /// <summary>
        /// Sets the Frente.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="value">The value.</param>
        public static void SetFrente(BordeoPazLuzItem target, Double value)
        {
            target.Frente = value;
        }
        /// <summary>
        /// The Bridge Fondo
        /// </summary>
        public Double Fondo
        {
            get
            {
                return (Double)GetValue(FondoProperty);
            }
            set
            {
                SetValue(FondoProperty, value);
            }
        }
        /// <summary>
        /// Gets the Fondo.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        public static Double GetFondo(BordeoPazLuzItem target)
        {
            return target.Fondo;
        }
        /// <summary>
        /// Sets the Fondo.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="value">The value.</param>
        public static void SetFondo(BordeoPazLuzItem target, Double value)
        {
            target.Fondo = value;
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
        public static String GetCode(BordeoPazLuzItem target)
        {
            return target.Code;
        }
        /// <summary>
        /// Sets the code.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="value">The value.</param>
        public static void SetCode(BordeoPazLuzItem target, String value)
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
        public static String GetAcabado(BordeoPazLuzItem target)
        {
            return target.Acabado;
        }
        /// <summary>
        /// Sets the acabado.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="value">The value.</param>
        public static void SetAcabado(BordeoPazLuzItem target, String value)
        {
            target.Code = value;
        }
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
                return this.CodeTxt.Text;
            }
        }
        public static DependencyProperty FrenteProperty;
        public static DependencyProperty FondoProperty;
        public static DependencyProperty CodeProperty;
        public static DependencyProperty AcabadoProperty;
        /// <summary>
        /// Constructor estatico
        /// </summary>
        static BordeoPazLuzItem()
        {
            CodeProperty = DependencyProperty.Register("Code", typeof(String), typeof(BordeoPazLuzItem),
                new FrameworkPropertyMetadata(String.Empty, FrameworkPropertyMetadataOptions.AffectsRender, Code_Changed));
            AcabadoProperty = DependencyProperty.Register("Acabado", typeof(String), typeof(BordeoPazLuzItem),
                new FrameworkPropertyMetadata(String.Empty, FrameworkPropertyMetadataOptions.AffectsRender, Acabado_Changed));
            FrenteProperty = DependencyProperty.Register("Frente", typeof(Double), typeof(BordeoPazLuzItem),
                new FrameworkPropertyMetadata(default(double), FrameworkPropertyMetadataOptions.AffectsRender, Frente_Changed));
            FondoProperty = DependencyProperty.Register("Fondo", typeof(Double), typeof(BordeoPazLuzItem),
                new FrameworkPropertyMetadata(default(double), FrameworkPropertyMetadataOptions.AffectsRender, Fondo_Changed));
        }
        /// <summary>
        /// Codes the changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void Code_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            BordeoPazLuzItem ctr = sender as BordeoPazLuzItem;
            String res = (String)e.NewValue;
            string sizeFormat = "{0:00}{1:00}",
                   size = String.Format(sizeFormat, ctr.Frente / 10, ctr.Fondo / 10);
            ctr.CodeTxt.Text = res + size + 'T' + (ctr.Acabado != null ? ctr.Acabado : String.Empty) ;
        }
        /// <summary>
        /// Acabadoes the changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void Acabado_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            BordeoPazLuzItem ctr = sender as BordeoPazLuzItem;
            String res = (String)e.NewValue;
            string sizeFormat = "{0:00}{1:00}",
                   size = String.Format(sizeFormat, ctr.Frente / 10, ctr.Fondo / 10);
            ctr.CodeTxt.Text = (ctr.Code != null ? ctr.Code : String.Empty) + size + 'T' + res;
        }

        private void UpdatePosition()
        {
            this.Height = this.Fondo;
            this.Width = this.Frente;
        }

        /// <summary>
        /// Frente changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void Frente_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            BordeoPazLuzItem ctr = sender as BordeoPazLuzItem;
            Double res = (Double)e.NewValue;
            ctr.FrenteWidth.Width = new GridLength(res);
            ctr.UpdatePosition();
        }
        /// <summary>
        /// Fondo changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void Fondo_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            BordeoPazLuzItem ctr = sender as BordeoPazLuzItem;
            Double res = (Double)e.NewValue;
            ctr.FondoHeight.Height = new GridLength(res);
            ctr.UpdatePosition();
        }
        /// <summary>
        /// Sets the code.
        /// </summary>
        /// <param name="code">The code.</param>
        public void SetCode(string code)
        {
            this.Code = code;
        }
        /// <summary>
        /// Gets the code.
        /// </summary>
        /// <param name="code">The code.</param>
        public string GetCode()
        {
            return this.Code;
        }
        /// <summary>
        /// Sets the acabado.
        /// </summary>
        /// <param name="acabdo">The acabdo.</param>
        public void SetAcabado(string acabdo)
        {
            this.Acabado = acabdo;
        }
        /// <summary>
        /// Updates the size.
        /// </summary>
        /// <param name="sizeName">Name of the size.</param>
        /// <param name="sizeValue">The size value.</param>
        public void UpdateSize(string sizeName, double sizeValue)
        {
            sizeValue *= 10;
            if (sizeName == "FRENTE")
                this.Frente = sizeValue;
            else if (sizeName == "FONDO")
                this.Fondo = sizeValue;
            string sizeFormat = "{0:00}{1:00}",
                   size = String.Format(sizeFormat, this.Frente / 10, this.Fondo / 10);
            this.CodeTxt.Text = this.Code + size + 'T' + this.Acabado;
        }

        public BordeoPazLuzItem()
        {
            InitializeComponent();
        }
    }
}
