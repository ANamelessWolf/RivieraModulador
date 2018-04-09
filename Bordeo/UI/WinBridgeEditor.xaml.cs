using DaSoft.Riviera.Modulador.Bordeo.Controller;
using DaSoft.Riviera.Modulador.Bordeo.Model;
using DaSoft.Riviera.Modulador.Core.Model;
using DaSoft.Riviera.Modulador.Core.Model.DB;
using DaSoft.Riviera.Modulador.Core.UI;
using MahApps.Metro.Controls;
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
        /// The Element Riviera codes
        /// </summary>
        public RivieraCode[] Codes;
        /// <summary>
        /// The Riviera Element sizes
        /// </summary>
        public Dictionary<string, ElementSizeCollection> Sizes;

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
            this.listOfBridges.SelectedIndex = 0;
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

        }
        /// <summary>
        /// Handles the changed event of the insert_checked control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void insert_checked_changed(object sender, RoutedEventArgs e)
        {
            string btnName = ((sender as Button).Parent as Grid).Children.OfType<Label>().FirstOrDefault().Content.ToString();
            btnName = btnName.Split('_')[0];
            var item = (puentes_layout as UserControl).FindName(btnName) as UserControl;
            item.Visibility = (sender as Button).Content.ToString() == "Insertar" ? Visibility.Visible : Visibility.Hidden;
            (sender as Button).Content = (sender as Button).Content.ToString() == "Insertar" ? "Remover" : "Insertar";
            (item as IBridgeItem).SetCode((item is BordeoPazLuzItem) ? "BR2060" : (this.listOfBridges.SelectedItem as TextBlock).Text.ToString());
            if ((item is BordeoPazLuzItem))
            {
                var pazo = item as BordeoPazLuzItem;
                pazo.Frente = (puentes_layout as ICanvasBridge).HorBridge.Frente;
                pazo.Fondo = (puentes_layout as ICanvasBridge).GetFondo();
            }
        }
        /// <summary>
        /// Handles the click event of the setAcabado control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void setAcabado_click(object sender, RoutedEventArgs e)
        {
            //string btnName = ((sender as Button).Parent as Grid).Children.OfType<Label>().FirstOrDefault().Content.ToString();
            //var item = puentes_layout.FindName(btnName) as UserControl;
            //if (item.Visibility == Visibility.Visible)
            //{
            //    String code = (item as IBridgeItem).GetCode();
            //    var win = new WinAcabadoPicker(code, this.Codes.FirstOrDefault(x => x.Code == code));
            //    if (win.ShowDialog().Value)
            //        (item as IBridgeItem).SetAcabado(win.SelectedAcabado.Acabado);
            //}
        }
        /// <summary>
        /// Handles the SelectionChanged event of the listOfBridges control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void listOfBridges_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (this.btnConsole != null)
            //    foreach (Button btn in this.btnConsole.Children.OfType<FrameworkElement>().
            //        Where(x => x is Button && x.Name[0] == 'b'))
            //        btn.Content = ((TextBlock)this.listOfBridges.SelectedItem).Text;
            //p0Luz.Content = "BR2060";
            //p1Luz.Content = "BR2060";
            //p2Luz.Content = "BR2060";
        }
        /// <summary>
        /// Handles the click event of the updateSize control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void updateSize_click(object sender, RoutedEventArgs e)
        {
            //string btnName = ((sender as Button).Parent as Grid).Children.OfType<Label>().FirstOrDefault().Content.ToString();
            //var item = puentes_layout.FindName(btnName) as UserControl;
            //if (item.Visibility == Visibility.Visible)
            //{
            //    var sizes = this.Sizes[(item as IBridgeItem).GetCode()];
            //    if (sizes.Code == "BR2040" || sizes.Code == "BR2050")
            //    {
            //        var bMeasures = sizes.Sizes.Select(x => x as BridgeMeasure);
            //        var win = new WinFrontFondoHeight(bMeasures.Select(x => x.Frente).Distinct(), bMeasures.Select(x => x.Alto).Distinct(), bMeasures.Select(x => x.Fondo).Distinct());
            //        if (win.ShowDialog().Value)
            //            this.UpdateBridge(item, win);
            //    }
            //    else if (sizes.Code == "BR2060")
            //    {
            //        var pMeausres = sizes.Sizes.Select(x => x as PazoLuzMeasure);
            //        var win = new WinFrontFondo(pMeausres.Select(x => x.Frente).Distinct(), pMeausres.Select(x => x.Fondo).Distinct());
            //        if (win.ShowDialog().Value)
            //            this.UpdatePazo(item as BordeoPazLuzItem, win);
            //    }
            //}
        }
        /// <summary>
        /// Updates the pazo.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="win">The win.</param>
        private void UpdatePazo(BordeoPazLuzItem item, WinFrontFondo win)
        {
            //BordeoPazLuzItem[] pazos =
            //    new BordeoPazLuzItem[] { puentes_layout.p0Luz, puentes_layout.p1Luz, puentes_layout.p2Luz };
            //pazos.ToList().ForEach(x =>
            //{
            //    x.UpdateSize(win.SelectedFront.Measure, win.SelectedFront.Nominal);
            //    x.UpdateSize(win.SelectedFondo.Measure, win.SelectedFondo.Nominal);
            //});
            //BordeoPuenteHorItem[] puentesH;
            //if (win.SelectedFondo.Nominal <= 42)
            //{
            //    puentesH = new BordeoPuenteHorItem[] { puentes_layout.bm0Start, puentes_layout.bm0End };
            //    puentesH.ToList().ForEach(x => x.UpdateSize(win.SelectedFront.Measure, win.SelectedFondo.Nominal));
            //}
            //else
            //{
            //    puentesH = new BordeoPuenteHorItem[] { puentes_layout.bm0Start, puentes_layout.bm0End };
            //    puentesH.ToList().ForEach(x => x.UpdateSize(win.SelectedFront.Measure, this.SelectFondo(win.SelectedFondo.Nominal, false)));
            //    puentesH = new BordeoPuenteHorItem[] { puentes_layout.bm1Start, puentes_layout.bm1End };
            //    puentesH.ToList().ForEach(x => x.UpdateSize(win.SelectedFront.Measure, this.SelectFondo(win.SelectedFondo.Nominal, true)));
            //}
            //BordeoPuenteItem[] puentes = new BordeoPuenteItem[]
            //{
            //    puentes_layout.b0Top, puentes_layout.b1Top, puentes_layout.b2Top,
            //    puentes_layout.b0Low, puentes_layout.b1Low, puentes_layout.b2Low
            //};
            //puentes.ToList().ForEach(x => x.UpdateSize(win.SelectedFront.Measure, win.SelectedFront.Nominal));
        }
        /// <summary>
        /// Updates the bridge.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="win">The win.</param>
        private void UpdateBridge(UserControl item, WinFrontFondoHeight win)
        {
            //    if (item is BordeoPuenteHorItem)
            //    {
            //        BordeoPuenteHorItem[] puentesH = item.Name.Contains('1') ?
            //            new BordeoPuenteHorItem[] { puentes_layout.bm0Start, puentes_layout.bm0End } :
            //            new BordeoPuenteHorItem[] { puentes_layout.bm1Start, puentes_layout.bm1End },
            //         puentesSel = item.Name.Contains('0') ?
            //            new BordeoPuenteHorItem[] { puentes_layout.bm0Start, puentes_layout.bm0End } :
            //            new BordeoPuenteHorItem[] { puentes_layout.bm1Start, puentes_layout.bm1End };
            //        double frenteFinal = puentes_layout.p0Luz.Fondo / 10 - win.SelectedFront.Nominal;
            //        frenteFinal = frenteFinal <= 0 ? 24d : frenteFinal;
            //        //if (frenteFinal + win.SelectedFront.Nominal != puentes_layout.p0Luz.Fondo / 10)
            //        //{ MessageBox.Show("El frente seleccionado no es valido con el fondo actual" }
            //        puentesSel.ToList().ForEach(x =>
            //        {
            //            x.UpdateSize(win.SelectedFront.Measure, win.SelectedFront.Nominal);
            //            x.UpdateSize(win.SelectedHeight.Measure, win.SelectedHeight.Nominal);
            //            x.UpdateSize(win.SelectedFondo.Measure, win.SelectedFondo.Nominal);
            //        });
            //        puentesH.ToList().ForEach(x =>
            //        {
            //            x.UpdateSize(win.SelectedFront.Measure, frenteFinal);
            //            x.UpdateSize(win.SelectedHeight.Measure, win.SelectedHeight.Nominal);
            //            x.UpdateSize(win.SelectedFondo.Measure, win.SelectedFondo.Nominal);
            //        });
            //    }
            //    else
            //    {
            //        IEnumerable<BordeoPuenteItem> puentes =
            //            puentes_layout.items.Children.OfType<FrameworkElement>().Where(x => x is BordeoPuenteItem).Select(x => x as BordeoPuenteItem);
            //        puentes.ToList().ForEach(x =>
            //        {
            //            x.UpdateSize(win.SelectedHeight.Measure, win.SelectedHeight.Nominal);
            //            x.UpdateSize(win.SelectedFondo.Measure, win.SelectedFondo.Nominal);
            //        });

            //        IEnumerable<IBridgeItem> pazoLuz =
            //            puentes_layout.items.Children.OfType<FrameworkElement>().Where(x => x is BordeoPazLuzItem).Select(x => x as IBridgeItem);
            //        puentes.Where(x => x.Name.Contains(item.Name.Substring(1, 1))).Select(x => x as IBridgeItem).Union(pazoLuz).ToList().ForEach(x => x.UpdateSize(win.SelectedFront.Measure, win.SelectedFront.Nominal));
            //    }
        }
        /// <summary>
        /// Selects the fondo.
        /// </summary>
        /// <param name="nominal">The nominal.</param>
        /// <param name="endFondo">if set to <c>true</c> [end fondo].</param>
        /// <returns>The selected "Fondo"</returns>
        private double SelectFondo(double nominal, bool endFondo)
        {
            if (nominal == 48d || nominal == 60 || nominal == 72)
                return nominal / 2d;
            else if (nominal == 54d && endFondo)
                return 24d;
            else if (nominal == 54d && !endFondo)
                return 30d;
            else if (nominal == 66d && endFondo)
                return 30d;
            else if (nominal == 66d && !endFondo)
                return 36d;
            else
                return nominal;
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
            this.Close();
        }

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
    }
}