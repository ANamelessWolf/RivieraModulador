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
using DaSoft.Riviera.Modulador.Bordeo.Controller;
using DaSoft.Riviera.Modulador.Bordeo.Model;
using DaSoft.Riviera.Modulador.Core.UI.Items;
using DaSoft.Riviera.Modulador.Bordeo.Model.UI;

namespace DaSoft.Riviera.Modulador.Bordeo.UI
{
    /// <summary>
    /// Interaction logic for PanelStackView.xaml
    /// </summary>
    public partial class PanelStackView : UserControl
    {
        /// <summary>
        /// La colección de items de los paneles seleccionados de la línea bordeo.
        /// </summary>
        public List<PanelItem> Items => this.list.Items.OfType<PanelItem>().ToList();
        /// <summary>
        /// Devuelve la lista de elementos seleccionados.
        /// </summary>
        /// <value>
        /// Los elementos seleccionados.
        /// </value>
        public IEnumerable<PanelItem> SelectedItems => this.Items.Where(x => x.PanelStatus == PanelItemStatus.Selected);
        /// <summary>
        /// Gets the <see cref="PanelItem"/> at the specified index.
        /// </summary>
        /// <value>
        /// The <see cref="PanelItem"/>.
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public PanelItem this[int index] => Items[index];
        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        public int Count => Items.Count;
        /// <summary>
        /// Initializes a new instance of the <see cref="PanelStackView"/> class.
        /// </summary>
        public PanelStackView()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Clears this instance.
        /// </summary>
        internal void Clear()
        {
            this.list.Items.Clear();
        }
        /// <summary>
        /// Fills the specified stack.
        /// </summary>
        /// <param name="stack">The stack.</param>
        internal void Fill(IBordeoPanelStyler stack, BordeoPanelHeight defHeight, Boolean sideA = true)
        {
            this.Clear();
            BordeoPanelHeightItem item = new BordeoPanelHeightItem() { Height = defHeight };
            String pName = item.ImageName;
            String[] acabados = sideA ? stack.AcabadosLadoA.Select(x => x.Acabado).ToArray() : stack.AcabadosLadoB.Select(x => x.Acabado).ToArray();
            String code = this.GetCode(stack);
            List<PanelItem> Panels = new List<PanelItem>();
            for (int i = 0, j = 0; i < pName.Length; i += 2)
            {
                if (pName.Substring(i, 2) == "PB")
                    Panels.Add(new PanelItem() { Code = String.Format("{0}27T", code), Acabado = acabados[j], Height = 135 });
                else if (pName.Substring(i, 2) == "Ps")
                    Panels.Add(new PanelItem() { Code = String.Format("{0}15T", code), Acabado = acabados[j], Height = 75 });
                if ((pName.Substring(i, 2) == "PB" || pName.Substring(i, 2) == "Ps") && j < acabados.Length - 1)
                    j++;
            }
            for (int i = Panels.Count - 1; i >= 0; i--)
                this.list.Items.Add(Panels[i]);
        }
        /// <summary>
        /// Gets the code.
        /// </summary>
        /// <param name="stack">The stack.</param>
        /// <returns></returns>
        public string GetCode(IBordeoPanelStyler stack)
        {
            String code = String.Empty;
            var size = stack.BordeoPanelSize;
            if (size is PanelMeasure)
            {
                var nSize = ((PanelMeasure)size);
                code = String.Format("{0}{1}{2}", stack.RivieraBordeoCode, nSize.Frente.Nominal, nSize.Alto.Nominal);
            }
            else if (size is LPanelMeasure)
            {
                var lSize = ((LPanelMeasure)size);
                double start, end;
                if (lSize.FrenteStart.Nominal < lSize.FrenteEnd.Nominal)
                {
                    start = lSize.FrenteStart.Nominal;
                    end = lSize.FrenteEnd.Nominal;
                }
                else
                {
                    start = lSize.FrenteEnd.Nominal;
                    end = lSize.FrenteStart.Nominal;
                }
                code = String.Format("{0}{1}{2}{3}", stack.RivieraBordeoCode, start, end, lSize.Alto.Nominal);
            }
            return code;
        }

        private void list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            (sender as ListView).SelectedIndex = -1;
        }
    }
}
