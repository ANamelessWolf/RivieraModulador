using DaSoft.Riviera.Modulador.Bordeo.Controller;
using DaSoft.Riviera.Modulador.Bordeo.Model;
using DaSoft.Riviera.Modulador.Bordeo.Model.Enities;
using DaSoft.Riviera.Modulador.Core.UI;
using DaSoft.Riviera.Modulador.Core.UI.Items;
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
using System.Windows.Shapes;

namespace DaSoft.Riviera.Modulador.Bordeo.UI
{
    /// <summary>
    /// Interaction logic for WinPanelEditor.xaml
    /// </summary>
    public partial class WinPanelEditor : MetroWindow
    {
        /// <summary>
        /// Gets the selected items.
        /// </summary>
        /// <value>
        /// The selected items.
        /// </value>
        public IEnumerable<PanelItem> SelectedItems
        {
            get
            {
                return this.stackViewA.SelectedItems.Union(this.stackViewB.SelectedItems);
            }
        }
        /// <summary>
        /// Las alturas de los paneles defindos.
        /// </summary>
        /// <value>
        /// La altura de los paneles.
        /// </value>
        public BordeoPanelHeight Heights
        {
            get { return this.heights.SelectedHeight; }
        }
        /// <summary>
        /// Gets the acabados lado a.
        /// </summary>
        /// <value>
        /// The acabados lado a.
        /// </value>
        public IEnumerable<String> AcabadosLadoA => this.stackViewA.Items.Select(x => x.Acabado);
        /// <summary>
        /// Gets the acabados lado a.
        /// </summary>
        /// <value>
        /// The acabados lado a.
        /// </value>
        public IEnumerable<String> AcabadosLadoB => this.stackViewB.Items.Select(x => x.Acabado);
        /// <summary>
        /// Accede al Panel Stack Styler
        /// </summary>
        IBordeoPanelStyler Stack;
        /// <summary>
        /// Initializes a new instance of the <see cref="WinPanelEditor"/> class.
        /// </summary>
        public WinPanelEditor()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="WinPanelEditor"/> class.
        /// </summary>
        /// <param name="stack">The stack.</param>
        public WinPanelEditor(IBordeoPanelStyler stack)
        {
            this.Stack = stack;
            InitializeComponent();
        }
        /// <summary>
        /// Handles the SelectionChanged event of the CtrlBordeoHeights control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void CtrlBordeoHeights_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.heights.SelectedIndex != -1)
            {
                this.stackViewA.Fill(this.Stack, this.heights.SelectedHeight);
                this.stackViewB.Fill(this.Stack, this.heights.SelectedHeight, false);
            }
        }
        /// <summary>
        /// Handles the Click event of the SetAcabado control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void SetAcabado_Click(object sender, RoutedEventArgs e)
        {
            String code = this.codeHost.Text.Substring(0, 6);
            WinAcabadoPicker win = new WinAcabadoPicker(code, BordeoUtils.GetRivieraCode(code));
            if (win.ShowDialog().Value)
            {
                foreach (PanelItem item in this.SelectedItems)
                {
                    item.Acabado = win.SelectedAcabado.Acabado;
                    item.PanelStatus = item.LastPanelStatus;
                }
            }
        }
        /// <summary>
        /// Handles the Click event of the CopyLeft control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void CopyLeft_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < this.stackViewA.Count; i++)
                this.stackViewA[i].Acabado = this.stackViewB[i].Acabado;
        }
        /// <summary>
        /// Handles the Click event of the CopyRight control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void CopyRight_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < this.stackViewA.Count; i++)
                this.stackViewB[i].Acabado = this.stackViewA[i].Acabado;
        }
        /// <summary>
        /// Handles the Click event of the Swap control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Swap_Click(object sender, RoutedEventArgs e)
        {
            String tmp;
            for (int i = 0; i < this.stackViewA.Count; i++)
            {
                tmp = this.stackViewA[i].Acabado;
                this.stackViewA[i].Acabado = this.stackViewB[i].Acabado;
                this.stackViewB[i].Acabado = tmp;
            }
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
        /// Handles the Loaded event of the MetroWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.heights.DefaultHeight = this.Stack.Height;
            this.stackViewA.Fill(this.Stack, this.Stack.Height);
            this.stackViewB.Fill(this.Stack, this.Stack.Height, false);
            this.codeHost.Text = this.Stack.RivieraBordeoCode;
            if (this.Stack.BordeoPanelSize is PanelMeasure)
                this.frenteSize.Text = String.Format("{0}\"", ((PanelMeasure)this.Stack.BordeoPanelSize).Frente.Nominal);
            else if (this.Stack.BordeoPanelSize is LPanelMeasure)
            {
                var size = (LPanelMeasure)this.Stack.BordeoPanelSize;
                double start, end;
                if (size.FrenteStart.Nominal < size.FrenteEnd.Nominal)
                {
                    start = size.FrenteStart.Nominal;
                    end = size.FrenteEnd.Nominal;
                }
                else
                {
                    start = size.FrenteEnd.Nominal;
                    end = size.FrenteStart.Nominal;
                }
                this.frenteSize.Text = String.Format("{0}\" {1}\"", start, end);
            }
        }
    }
}
