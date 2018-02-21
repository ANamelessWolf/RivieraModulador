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
    /// Interaction logic for BordeoPuenteHorItem.xaml
    /// </summary>
    public partial class BordeoPuenteHorItem : UserControl,IBridgeItem
    {
        /// <summary>
        /// The Bridge Rotation
        /// </summary>
        public Double Rotation
        {
            get
            {
                return (Double)GetValue(RotationProperty);
            }
            set
            {
                SetValue(RotationProperty, value);
            }
        }
        /// <summary>
        /// Gets the Rotation.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        public static Double GetRotation(BordeoPuenteHorItem target)
        {
            return target.Rotation;
        }
        /// <summary>
        /// Sets the Rotation.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="value">The value.</param>
        public static void SetRotation(BordeoPuenteHorItem target, Double value)
        {
            target.Rotation = value;
        }
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
        public static Double GetFrente(BordeoPuenteHorItem target)
        {
            return target.Frente;
        }
        /// <summary>
        /// Sets the Frente.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="value">The value.</param>
        public static void SetFrente(BordeoPuenteHorItem target, Double value)
        {
            target.Frente = value;
        }
        /// <summary>
        /// The Bridge Altura
        /// </summary>
        public Double Altura
        {
            get
            {
                return (Double)GetValue(AlturaProperty);
            }
            set
            {
                SetValue(AlturaProperty, value);
            }
        }
        /// <summary>
        /// Gets the Altura.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        public static Double GetAltura(BordeoPuenteHorItem target)
        {
            return target.Altura;
        }
        /// <summary>
        /// Sets the Altura.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="value">The value.</param>
        public static void SetAltura(BordeoPuenteHorItem target, Double value)
        {
            target.Altura = value;
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
        public static Double GetFondo(BordeoPuenteHorItem target)
        {
            return target.Fondo;
        }
        /// <summary>
        /// Sets the Fondo.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="value">The value.</param>
        public static void SetFondo(BordeoPuenteHorItem target, Double value)
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
        public static String GetCode(BordeoPuenteHorItem target)
        {
            return target.Code;
        }
        /// <summary>
        /// Sets the code.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="value">The value.</param>
        public static void SetCode(BordeoPuenteHorItem target, String value)
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
        public static String GetAcabado(BordeoPuenteHorItem target)
        {
            return target.Acabado;
        }
        /// <summary>
        /// Sets the acabado.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="value">The value.</param>
        public static void SetAcabado(BordeoPuenteHorItem target, String value)
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
        public static DependencyProperty RotationProperty;
        public static DependencyProperty FrenteProperty;
        public static DependencyProperty AlturaProperty;
        public static DependencyProperty FondoProperty;
        public static DependencyProperty CodeProperty;
        public static DependencyProperty AcabadoProperty;
        /// <summary>
        /// Constructor estatico
        /// </summary>
        static BordeoPuenteHorItem()
        {
            CodeProperty = DependencyProperty.Register("Code", typeof(String), typeof(BordeoPuenteHorItem),
                new FrameworkPropertyMetadata(String.Empty, FrameworkPropertyMetadataOptions.AffectsRender, Code_Changed));
            AcabadoProperty = DependencyProperty.Register("Acabado", typeof(String), typeof(BordeoPuenteHorItem),
                new FrameworkPropertyMetadata(String.Empty, FrameworkPropertyMetadataOptions.AffectsRender, Acabado_Changed));
            RotationProperty = DependencyProperty.Register("Rotation", typeof(Double), typeof(BordeoPuenteHorItem),
                new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender, Rotation_Changed));
            FrenteProperty = DependencyProperty.Register("Frente", typeof(Double), typeof(BordeoPuenteHorItem),
                new FrameworkPropertyMetadata(default(double), FrameworkPropertyMetadataOptions.AffectsRender, Frente_Changed));
            AlturaProperty = DependencyProperty.Register("Altura", typeof(Double), typeof(BordeoPuenteHorItem),
                new FrameworkPropertyMetadata(default(double), FrameworkPropertyMetadataOptions.AffectsRender, Altura_Changed));
            FondoProperty = DependencyProperty.Register("Fondo", typeof(Double), typeof(BordeoPuenteHorItem),
                new FrameworkPropertyMetadata(default(double), FrameworkPropertyMetadataOptions.AffectsRender, Fondo_Changed));
        }
        /// <summary>
        /// Codes the changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void Code_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            BordeoPuenteHorItem ctr = sender as BordeoPuenteHorItem;
            String res = (String)e.NewValue;
            ctr.CodeTxt.Text = res + (ctr.Acabado != null ? ctr.Acabado : String.Empty);
        }
        /// <summary>
        /// Acabadoes the changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void Acabado_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            BordeoPuenteHorItem ctr = sender as BordeoPuenteHorItem;
            String res = (String)e.NewValue;
            ctr.CodeTxt.Text = (ctr.Code != null ? ctr.Code : String.Empty) + res;
        }
        /// <summary>
        /// Rotation changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void Rotation_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            BordeoPuenteHorItem ctr = sender as BordeoPuenteHorItem;
            Double res = (Double)e.NewValue;
            ctr.rotPanel.Angle = res;
            ctr.UpdatePosition();
        }

        private void UpdatePosition()
        {
            this.Height  = this.Frente;
            this.Width = this.Altura + this.Fondo + 20d;
            this.rotPanel.CenterX = this.Width / 2;
            this.rotPanel.CenterY = this.Height / 2;
            if (this.Rotation == 0)
            {
                this.rotTxt.ScaleX = 1;
                this.rotTxt.ScaleY = 1;
            }
            else
            {
                this.rotTxt.ScaleX = -1;
                this.rotTxt.ScaleY = -1;
            }
            this.rotTxt.CenterX = this.Width / 2;
        }

        /// <summary>
        /// Frente changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void Frente_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            BordeoPuenteHorItem ctr = sender as BordeoPuenteHorItem;
            Double res = (Double)e.NewValue;
            ctr.FrenteWidth.Height = new GridLength(res);
            ctr.UpdatePosition();
        }
        /// <summary>
        /// Altura changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void Altura_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            BordeoPuenteHorItem ctr = sender as BordeoPuenteHorItem;
            Double res = (Double)e.NewValue;
            ctr.AlturaHeight.Width = new GridLength(res);
            ctr.UpdatePosition();
        }
        /// <summary>
        /// Fondo changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void Fondo_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            BordeoPuenteHorItem ctr = sender as BordeoPuenteHorItem;
            Double res = (Double)e.NewValue;
            ctr.FondoHeight.Width = new GridLength(res);
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
            if (sizeName == "Frente")
                this.Frente = sizeValue;
            else if (sizeName == "Fondo")
                this.Fondo = sizeValue;
            else if (sizeName == "Altura")
                this.Altura = sizeValue;
        }

        public BordeoPuenteHorItem()
        {
            InitializeComponent();
        }
    }
}
