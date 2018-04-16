using DaSoft.Riviera.Modulador.Bordeo.Controller;
using DaSoft.Riviera.Modulador.Bordeo.Model;
using DaSoft.Riviera.Modulador.Core.Controller.UI;
using DaSoft.Riviera.Modulador.Core.Model;
using DaSoft.Riviera.Modulador.Core.Model.DB;
using DaSoft.Riviera.Modulador.Core.Runtime;
using DaSoft.Riviera.Modulador.Core.UI;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
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
    /// Interaction logic for WinBridgeEditor.xaml
    /// </summary>
    public partial class WinBridgeEditor : MetroWindow
    {
        /// <summary>
        /// The selected bridge result
        /// </summary>
        public BridgeSelectionResult SelectedBridge;
        /// <summary>
        /// The Element Riviera codes
        /// </summary>
        public RivieraCode[] Codes;
        /// <summary>
        /// The Riviera Element sizes
        /// </summary>
        public Dictionary<string, ElementSizeCollection> Sizes;
        /// <summary>
        /// Gets the puentes layout.
        /// </summary>
        /// <value>
        /// The puentes layout.
        /// </value>
        public ICanvasBridge puentes_layout
        {
            get
            {
                if (canvasTab.SelectedIndex == 0)
                    return (ICanvasBridge)cBridgeSingle;

                else if (canvasTab.SelectedIndex == 1)
                    return (ICanvasBridge)cBridgeDouble;
                else
                    return (ICanvasBridge)cBridgeTriple;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WinBridgeEditor"/> class.
        /// </summary>
        public WinBridgeEditor()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Handles the Loaded event of the MetroWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Sizes = new Dictionary<string, ElementSizeCollection>();
            this.InitSizes();
            String bCode = "BR2040";
            var bMeasures = this.Sizes[bCode].Sizes.Select(x => x as BridgeMeasure);
            this.oneBridgeProp.Init(bMeasures.Select(x => x.Alto).Distinct(), this.Codes[0]);
            this.onePazoProp.Init(this.Codes[2]);
            this.twoBridgeProp.Init(bMeasures.Select(x => x.Alto).Distinct(), this.Codes[0]);
            this.twoPazoProp.Init(this.Codes[2]); this.oneBridgeProp.Init(bMeasures.Select(x => x.Alto).Distinct(), this.Codes[0]);
            this.threeBridgeProp.Init(bMeasures.Select(x => x.Alto).Distinct(), this.Codes[0]);
            this.threePazoProp.Init(this.Codes[2]); this.oneBridgeProp.Init(bMeasures.Select(x => x.Alto).Distinct(), this.Codes[0]);
            this.UpdateCode();
        }
        /// <summary>
        /// Handles the changed event of the insert_checked control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void insert_checked_changed(object sender, RoutedEventArgs e)
        {
            if (puentes_layout.PazoLuz.Frente / 10 < 36)
            {
                string btnName = ((sender as Button).Parent as Grid).Children.OfType<Label>().FirstOrDefault().Content.ToString();
                btnName = btnName.Split('_')[0];
                var item = (puentes_layout as UserControl).FindName(btnName) as UserControl;
                item.Visibility = (sender as Button).Content.ToString() == "Insertar" ? Visibility.Visible : Visibility.Hidden;
                (sender as Button).Content = (sender as Button).Content.ToString() == "Insertar" ? "Remover" : "Insertar";
                (item as IBridgeItem).SetCode((item is BordeoPazLuzItem) ? "BR2060" : "BR2040");
                if ((item is BordeoPazLuzItem))
                {
                    var pazo = item as BordeoPazLuzItem;
                    pazo.Frente = (puentes_layout as ICanvasBridge).HorBridge.Frente;
                    pazo.Fondo = (puentes_layout as ICanvasBridge).GetFondo();
                }
            }
            else
            {
                puentes_layout.HorBridge.Visibility = Visibility.Visible;
                (sender as Button).Content = "Insertar";
            }
        }

        /// <summary>
        /// Handles the click event of the updateSize control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void updateSize_click(object sender, RoutedEventArgs e)
        {
            string btnName = ((sender as Button).Parent as Grid).Children.OfType<Label>().FirstOrDefault().Content.ToString();
            string cIndex = btnName.Split('_')[1];
            btnName = btnName.Replace("_" + cIndex, "");
            Window win;
            var bMeasures = this.Sizes["BR2040"].Sizes.Select(x => x as BridgeMeasure);
            IEnumerable<FrameworkElement> items;
            if (btnName == "bmiddle")
            {
                items = this.puentes_layout.CanvasContainer.Children.OfType<FrameworkElement>().Where(x => x is BordeoPuenteHorItem || x is BordeoPazLuzItem);
                win = new WinFrontFondo(bMeasures.Select(x => x.Frente).Distinct(), bMeasures.Select(x => x.Fondo).Distinct());
            }
            else if (btnName == "pazoLuz")
            {
                items = this.puentes_layout.CanvasContainer.Children.OfType<FrameworkElement>();
                win = new WinSizePicker(bMeasures.Select(x => x.Frente).Distinct(), "Fondo");
            }
            else
            {
                items = this.puentes_layout.CanvasContainer.Children.OfType<FrameworkElement>().Where(x => x is BordeoPuenteItem || x is BordeoPazLuzItem);
                win = new WinFrontFondo(bMeasures.Select(x => x.Frente).Distinct(), bMeasures.Select(x => x.Fondo).Distinct());
            }
            BordeoPuenteHorItem middle;
            BordeoPazLuzItem pazo;
            if (win.ShowDialog().Value)
            {
                if (btnName == "bmiddle")
                {
                    middle = (BordeoPuenteHorItem)items.FirstOrDefault(x => x is BordeoPuenteHorItem);
                    middle.UpdateSize("FRENTE", ((WinFrontFondo)win).SelectedFront.Nominal);
                    middle.UpdateSize("FONDO", ((WinFrontFondo)win).SelectedFondo.Nominal);
                    pazo = (BordeoPazLuzItem)items.FirstOrDefault(x => x is BordeoPazLuzItem);
                    pazo.UpdateSize("FRENTE", ((WinFrontFondo)win).SelectedFront.Nominal);
                }
                else if (btnName == "pazoLuz")
                {
                    middle = (BordeoPuenteHorItem)items.FirstOrDefault(x => x is BordeoPuenteHorItem);
                    middle.UpdateSize("FRENTE", ((WinSizePicker)win).SelectedSize.Nominal);
                    pazo = (BordeoPazLuzItem)items.FirstOrDefault(x => x is BordeoPazLuzItem);
                    pazo.UpdateSize("FRENTE", ((WinSizePicker)win).SelectedSize.Nominal);
                    if (((WinSizePicker)win).SelectedSize.Nominal == 36)
                        middle.Visibility = Visibility.Visible;
                }
                else
                {

                    List<BordeoPuenteItem> bridges = items.Where(x => x is BordeoPuenteItem).Select(y => y as BordeoPuenteItem).ToList();
                    string row = btnName.Substring(0, 2);
                    var b = bridges.Where(x => x.Name.Substring(2) == "Top" && x.Name.Substring(0, 2) != row);
                    double size = b.Sum(y => y.Frente / 10);
                    size += ((WinFrontFondo)win).SelectedFront.Nominal;
                    if (size <= 72)
                    {

                        bridges.ForEach(x =>
                        {
                            if (row == x.Name.Substring(0, 2))
                                x.UpdateSize("FRENTE", ((WinFrontFondo)win).SelectedFront.Nominal);
                        });
                        bridges.ForEach(x => x.UpdateSize("FONDO", ((WinFrontFondo)win).SelectedFondo.Nominal));
                        pazo = (BordeoPazLuzItem)items.FirstOrDefault(x => x is BordeoPazLuzItem);
                        pazo.UpdateSize("FONDO", size);
                    }
                    else
                    {
                        await this.ShowMessageAsync("Tamaño de frente no valido", "El tamaño de frente seleccionado, rompe la regla de tamaño de fondo máximo de 72\" para el pazo de Luz.", MessageDialogStyle.AffirmativeAndNegative);
                    }
                }
            }
            this.UpdateCode();
        }
        /// <summary>
        /// Initializes the sizes.
        /// </summary>
        public void InitSizes()
        {
            ElementSizeCollection pazoLuz = new ElementSizeCollection("BR2060");
            ElementSizeCollection puente90 = new ElementSizeCollection("BR2040");
            ElementSizeCollection puente135 = new ElementSizeCollection("BR2050");
            this.Codes = new RivieraCode[]
            {
                new RivieraCode("01", "02", "03") { Block = "BR2040", Code = "BR2040", Description = "Puente 90°", ElementType = RivieraElementType.Bridge, Line = DesignLine.Bordeo },
                new RivieraCode("01", "02", "03") { Block = "BR2050", Code = "BR2050", Description = "Puente 135°", ElementType = RivieraElementType.Bridge, Line = DesignLine.Bordeo },
                new RivieraCode("01", "02", "03") { Block = "BR2060", Code = "BR2060", Description = "Pazo de Luz", ElementType = RivieraElementType.Pazo_Luz, Line = DesignLine.Bordeo }
            };
            double[] frentes = new double[] { 24d, 30d, 36d },
                     frentesPazo = new double[] { 24d, 30d, 36d },
                     heights = new double[] { 24d, 36d },
                     fondos135 = new double[] { 18d, 24d, 30d },
                     fondos90 = new double[] { 18d, 24d, 30d, 36d },
                     fondosPazo = new double[] { 24d, 30d, 36d, 42d, 48d, 54d, 60d, 66d, 72d };
            this.Combine(ref puente90, frentes, heights, fondos90);
            this.Combine(ref puente135, frentes, heights, fondos135);
            this.Combine(ref pazoLuz, frentesPazo, fondosPazo);
            this.Sizes.Add(pazoLuz.Code, pazoLuz);
            this.Sizes.Add(puente90.Code, puente90);
            this.Sizes.Add(puente135.Code, puente135);
        }
        /// <summary>
        /// Combines the specified pazo de Luz sizes.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="frentes">The available fronts.</param>
        /// <param name="frentes">The available heights.</param
        /// <param name="fondos">The available fondos.</param>
        private void Combine(ref ElementSizeCollection item, double[] frentes, double[] heights, double[] fondos)
        {
            RivieraSize frente, alto, fondo;
            for (int i = 0; i < frentes.Length; i++)
                for (int j = 0; j < heights.Length; j++)
                    for (int k = 0; k < fondos.Length; k++)
                    {
                        frente = new RivieraSize() { Measure = KEY_FRONT, Nominal = frentes[i], Real = frentes[i] * 0.0254d };
                        alto = new RivieraSize() { Measure = KEY_HEIGHT, Nominal = heights[j], Real = heights[j] * 0.0254d };
                        fondo = new RivieraSize() { Measure = KEY_DEPHT, Nominal = fondos[k], Real = fondos[k] * 0.0254d };
                        item.Sizes.Add(new BridgeMeasure(frente, alto, fondo));
                    }
        }
        /// <summary>
        /// Combines the specified pazo de Luz sizes.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="frentes">The available fronts.</param>
        /// <param name="fondos">The available fondos.</param>
        private void Combine(ref ElementSizeCollection item, double[] frentes, double[] fondos)
        {
            RivieraSize frente, fondo;
            for (int i = 0; i < frentes.Length; i++)
                for (int j = 0; j < fondos.Length; j++)
                {
                    frente = new RivieraSize() { Measure = KEY_FRONT, Nominal = frentes[i], Real = frentes[i] * 0.0254d };
                    fondo = new RivieraSize() { Measure = KEY_DEPHT, Nominal = fondos[j], Real = fondos[j] * 0.0254d };
                    item.Sizes.Add(new PazoLuzMeasure(frente, fondo));
                }
        }
        /// <summary>
        /// Handles the Click event of the Cancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;

            this.Close();
        }
        /// <summary>
        /// Handles the Click event of the Ok control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            BridgeProperties bridgeProp = (this.puentes_layout is CanvasBridgeSingle) ? oneBridgeProp :
                            (this.puentes_layout is CanvasBridgeDouble) ? twoBridgeProp : threeBridgeProp;
            PazoLuzProperties pazoProp = (this.puentes_layout is CanvasBridgeSingle) ? onePazoProp :
                        (this.puentes_layout is CanvasBridgeDouble) ? twoPazoProp : threePazoProp;
            RivieraAcabado codeBridge = bridgeProp.SelectedExtAcabado.Acabado.CompareTo(bridgeProp.SelectedIntAcabado.Acabado) > 0 ?
                bridgeProp.SelectedExtAcabado : bridgeProp.SelectedIntAcabado,
                codePazo = pazoProp.SelectedExtAcabado.Acabado.CompareTo(pazoProp.SelectedIntAcabado.Acabado) > 0 ?
                bridgeProp.SelectedExtAcabado : pazoProp.SelectedIntAcabado;

            this.SelectedBridge = new BridgeSelectionResult()
            {
                SelectedCode = this.bridgeGroupCode.Text,
                AcabadoPazo = codePazo,
                AcabadoPuentes = codeBridge
            };
            this.Close();
        }
        /// <summary>
        /// Handles the SelectionChanged event of the TabControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tab = (TabControl)sender;
            if (tab.SelectedIndex == 0)
            {
                cBridgeSingle.Visibility = Visibility.Visible;
                cBridgeDouble.Visibility = Visibility.Collapsed;
                cBridgeTriple.Visibility = Visibility.Collapsed;
            }
            else if (tab.SelectedIndex == 1)
            {
                cBridgeSingle.Visibility = Visibility.Collapsed;
                cBridgeDouble.Visibility = Visibility.Visible;
                cBridgeTriple.Visibility = Visibility.Collapsed;
            }
            else if (tab.SelectedIndex == 2)
            {
                cBridgeSingle.Visibility = Visibility.Collapsed;
                cBridgeDouble.Visibility = Visibility.Collapsed;
                cBridgeTriple.Visibility = Visibility.Visible;
            }
            this.UpdateCode();
        }
        /// <summary>
        /// Handles the PropertyChanged event of the threeBridgeProp control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void oneBridgeProp_PropertyChanged(object sender, SelectionChangedEventArgs e)
        {
            PropertyChangedArgs args = (PropertyChangedArgs)e;
            var bridges = (this.cBridgeSingle as ICanvasBridge).CanvasContainer.Children.OfType<FrameworkElement>().
                Where(x => x is BordeoPuenteItem || x is BordeoPuenteHorItem).ToList();
            this.UpdateBridgeProperties(args, this.oneBridgeProp, bridges);
            this.UpdateCode();
        }
        /// <summary>
        /// Handles the PropertyChanged event of the onePazoProp control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void onePazoProp_PropertyChanged(object sender, SelectionChangedEventArgs e)
        {
            PropertyChangedArgs args = (PropertyChangedArgs)e;
            var pazo = (this.cBridgeSingle as ICanvasBridge).CanvasContainer.Children.OfType<FrameworkElement>().
                  FirstOrDefault(x => x is BordeoPazLuzItem) as BordeoPazLuzItem;
            this.UpdatePazo(args, this.onePazoProp, pazo);
            this.UpdateCode();
        }
        /// <summary>
        /// Handles the PropertyChanged event of the threeBridgeProp control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void twoBridgeProp_PropertyChanged(object sender, SelectionChangedEventArgs e)
        {
            PropertyChangedArgs args = (PropertyChangedArgs)e;
            var bridges = (this.cBridgeDouble as ICanvasBridge).CanvasContainer.Children.OfType<FrameworkElement>().
                Where(x => x is BordeoPuenteItem || x is BordeoPuenteHorItem).ToList();
            this.UpdateBridgeProperties(args, this.twoBridgeProp, bridges);
        }
        /// <summary>
        /// Handles the PropertyChanged event of the twoPazoProp control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void twoPazoProp_PropertyChanged(object sender, SelectionChangedEventArgs e)
        {
            PropertyChangedArgs args = (PropertyChangedArgs)e;
            var pazo = (this.cBridgeDouble as ICanvasBridge).CanvasContainer.Children.OfType<FrameworkElement>().
                FirstOrDefault(x => x is BordeoPazLuzItem) as BordeoPazLuzItem;
            this.UpdatePazo(args, this.twoPazoProp, pazo);
        }
        /// <summary>
        /// Handles the PropertyChanged event of the threeBridgeProp control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void threeBridgeProp_PropertyChanged(object sender, SelectionChangedEventArgs e)
        {
            PropertyChangedArgs args = (PropertyChangedArgs)e;
            var bridges = (this.cBridgeTriple as ICanvasBridge).CanvasContainer.Children.OfType<FrameworkElement>().
                Where(x => x is BordeoPuenteItem || x is BordeoPuenteHorItem).ToList();
            this.UpdateBridgeProperties(args, this.threeBridgeProp, bridges);
        }
        /// <summary>
        /// Handles the PropertyChanged event of the threePazoProp control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void threePazoProp_PropertyChanged(object sender, SelectionChangedEventArgs e)
        {
            PropertyChangedArgs args = (PropertyChangedArgs)e;
            var pazo = (this.cBridgeTriple as ICanvasBridge).CanvasContainer.Children.OfType<FrameworkElement>().
                FirstOrDefault(x => x is BordeoPazLuzItem) as BordeoPazLuzItem;
            this.UpdatePazo(args, this.threePazoProp, pazo);
        }
        /// <summary>
        /// Updates the bridge properties.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="bridgeProp">The bridge property.</param>
        /// <param name="bridges">The bridges.</param>
        private void UpdateBridgeProperties(PropertyChangedArgs args, BridgeProperties bridgeProp, List<FrameworkElement> bridges)
        {
            RivieraAcabado acabado;
            if (args.PropertyName == "AcabadosExterior" && args.PropertyValue != null)
            {
                acabado = bridgeProp.SelectedExtAcabado.Acabado.CompareTo(bridgeProp.SelectedIntAcabado.Acabado) > 0 ?
                    bridgeProp.SelectedExtAcabado : bridgeProp.SelectedIntAcabado;
                bridges.ForEach(x => (x as IBridgeItem).SetAcabado(acabado.Acabado));
            }
            else if (args.PropertyName == "AcabadosInterior" && args.PropertyValue != null)
            {
                acabado = bridgeProp.SelectedExtAcabado.Acabado.CompareTo(bridgeProp.SelectedIntAcabado.Acabado) > 0 ?
                    bridgeProp.SelectedExtAcabado : bridgeProp.SelectedIntAcabado;
                bridges.ForEach(x => (x as IBridgeItem).SetAcabado(acabado.Acabado));
            }
            else if (args.PropertyName == "Altura")
                bridges.ForEach(x => (x as IBridgeItem).UpdateSize("ALTO", bridgeProp.SelectedHeight.Nominal));
        }
        /// <summary>
        /// Updates the pazo.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="pazoProp">The pazo property.</param>
        /// <param name="pazo">The pazo.</param>
        private void UpdatePazo(PropertyChangedArgs args, PazoLuzProperties pazoProp, BordeoPazLuzItem pazo)
        {
            RivieraAcabado acabado;
            if (args.PropertyName == "AcabadosExterior" && args.PropertyValue != null)
            {
                acabado = pazoProp.SelectedExtAcabado.Acabado.CompareTo(pazoProp.SelectedIntAcabado.Acabado) > 0 ?
                    pazoProp.SelectedExtAcabado : pazoProp.SelectedIntAcabado;
                pazo.SetAcabado(acabado.Acabado);
            }
            else if (args.PropertyName == "AcabadosInterior" && args.PropertyValue != null)
            {
                acabado = pazoProp.SelectedExtAcabado.Acabado.CompareTo(pazoProp.SelectedIntAcabado.Acabado) > 0 ?
                    pazoProp.SelectedExtAcabado : pazoProp.SelectedIntAcabado;
                pazo.SetAcabado(acabado.Acabado);
            }
        }
        /// <summary>
        /// Middles the bridge visibility changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void middleBridgeVisibilityChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            BordeoPuenteHorItem bmiddle = (BordeoPuenteHorItem)sender;
            this.UpdateCode();
        }
        /// <summary>
        /// Updates the code.
        /// </summary>
        private void UpdateCode()
        {
            //  BR9|2|24|24|24|30|30|H24|P30
            int bridgeIndex = this.puentes_layout is CanvasBridgeSingle ? 1 : this.puentes_layout is CanvasBridgeDouble ? 2 : 3;
            double[] codeParams = new double[] {
                            (this.puentes_layout is CanvasBridgeSingle) ? (this.puentes_layout as CanvasBridgeSingle).b0Top.Frente :
                            (this.puentes_layout is CanvasBridgeDouble) ? (this.puentes_layout as CanvasBridgeDouble).b0Top.Frente:
                            (this.puentes_layout as CanvasBridgeTriple).b0Top.Frente, //b0Front
                            (this.puentes_layout is CanvasBridgeSingle) ? double.NaN :
                            (this.puentes_layout is CanvasBridgeDouble) ? (this.puentes_layout as CanvasBridgeDouble).b1Top.Frente :
                            (this.puentes_layout as CanvasBridgeTriple).b1Top.Frente, //b1Front
                            (this.puentes_layout is CanvasBridgeSingle) ? double.NaN :
                            (this.puentes_layout is CanvasBridgeDouble) ? double.NaN :
                            (this.puentes_layout as CanvasBridgeTriple).b2Top.Frente, //b2Front
                            (this.puentes_layout is CanvasBridgeSingle) ? (this.puentes_layout as CanvasBridgeSingle).b0Top.Fondo:
                            (this.puentes_layout is CanvasBridgeDouble) ? (this.puentes_layout as CanvasBridgeDouble).b0Top.Fondo:
                            (this.puentes_layout as CanvasBridgeTriple).b0Top.Fondo,  //bFondo
                            this.puentes_layout.PazoLuz.Frente, //Pazo Frente
                            (this.puentes_layout is CanvasBridgeSingle) ? oneBridgeProp.SelectedHeight.Nominal*10d ://bHeight
                            (this.puentes_layout is CanvasBridgeDouble) ? twoBridgeProp.SelectedHeight.Nominal*10d :
                            threeBridgeProp.SelectedHeight.Nominal*10d,
                            this.puentes_layout.HorBridge.IsVisible ? this.puentes_layout.HorBridge.Frente : double.NaN //bMiddleFront
            };
            String[] codeParamsStr = new String[codeParams.Length + 1];
            int i = 1;
            codeParamsStr[0] = bridgeIndex.ToString();
            codeParams.ToList().ForEach(x => codeParamsStr[i++] = double.IsNaN(x) ? "NA" : (x / 10d).ToString());
            this.bridgeGroupCode.Text = String.Format("BR9{0}{1}{2}{3}{4}{5}H{6}P{7}", codeParamsStr);
        }
        /// <summary>
        /// Canvases the is loaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void canvasIsLoaded(object sender, RoutedEventArgs e)
        {
            this.UpdateCode();
        }
    }
}