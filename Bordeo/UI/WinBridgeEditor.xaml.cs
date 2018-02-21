using DaSoft.Riviera.Modulador.Bordeo.Controller;
using DaSoft.Riviera.Modulador.Bordeo.Model;
using DaSoft.Riviera.Modulador.Core.Model;
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
        public RivieraCode[] Codes;
        public Dictionary<string, ElementSizeCollection> Sizes;
        public WinBridgeEditor()
        {
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.listOfBridges.SelectedIndex = 0;
            this.Sizes = new Dictionary<string, ElementSizeCollection>();
        }

        private void insert_checked_changed(object sender, RoutedEventArgs e)
        {
            string btnName = ((sender as Button).Parent as Grid).Children.OfType<Label>().FirstOrDefault().Content.ToString();
            var item = puentes_layout.FindName(btnName) as UserControl;
            item.Visibility = (sender as Button).Content.ToString() == "Insertar" ? Visibility.Visible : Visibility.Collapsed;
            (sender as Button).Content = (sender as Button).Content.ToString() == "Insertar" ? "Remover" : "Insertar";
            (item as IBridgeItem).SetCode((this.listOfBridges.SelectedItem as TextBlock).Text.ToString());
        }

        private void setAcabado_click(object sender, RoutedEventArgs e)
        {

        }

        private void listOfBridges_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.btnConsole != null)
                foreach (Button btn in this.btnConsole.Children.OfType<FrameworkElement>().
                    Where(x => x is Button && x.Name[0] == 'b'))
                    btn.Content = ((TextBlock)this.listOfBridges.SelectedItem).Text;
        }

        private void updateSize_click(object sender, RoutedEventArgs e)
        {

        }
        public void InitSizes()
        {
            ElementSizeCollection pazoLuz = new ElementSizeCollection("BR2060");
            ElementSizeCollection puente90 = new ElementSizeCollection("BR2040");
            ElementSizeCollection puente135 = new ElementSizeCollection("BR2050");
            this.Codes = new RivieraCode[]
            {    new RivieraCode("01", "02", "03") { Block = "BR2040", Code = "BR2040", Description = "Puente 90°", ElementType = RivieraElementType.Bridge, Line = DesignLine.Bordeo },
                 new RivieraCode("01", "02", "03") { Block = "BR2050", Code = "BR2050", Description = "Puente 135°", ElementType = RivieraElementType.Bridge, Line = DesignLine.Bordeo },
                 new RivieraCode("01", "02", "03") { Block = "BR2060", Code = "BR2060", Description = "Pazo de Luz", ElementType = RivieraElementType.Pazo_Luz, Line = DesignLine.Bordeo }
            };
            double[] frentes = new double[] { 24d, 30d, 36d },
                     frentesPazo = new double[] { 18d, 24d, 30d, 36d },
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
    }
}
