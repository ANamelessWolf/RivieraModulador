using DaSoft.Riviera.Modulador.Bordeo.Controller;
using DaSoft.Riviera.Modulador.Core.Model;
using DaSoft.Riviera.Modulador.Core.Model.DB;
using DaSoft.Riviera.Modulador.Core.Model.UI;
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
    /// Interaction logic for BridgeProperties.xaml
    /// </summary>
    public partial class BridgeProperties : UserControl
    {
        /// <summary>
        /// Occurs when [property changed].
        /// </summary>
        public event SelectionChangedEventHandler PropertyChanged;
        /// <summary>
        /// The heights
        /// </summary>
        IEnumerable<RivieraSize> _Heights;
        /// <summary>
        /// The acabados
        /// </summary>
        IEnumerable<RivieraAcabado> _Acabados;
        /// <summary>
        /// Gets the selected height.
        /// </summary>
        /// <value>
        /// The selected Height.
        /// </value>
        public RivieraSize SelectedHeight
        {
            get
            {
                if (this.bridgeHeight.SelectedIndex != -1)
                    return (this.bridgeHeight.SelectedItem as RivieraSizeItem).Size;
                else
                    return default(RivieraSize);
            }
        }
        /// <summary>
        /// Gets the selected int acabado.
        /// </summary>
        /// <value>
        /// The selected int acabado.
        /// </value>
        public RivieraAcabado SelectedIntAcabado
        {
            get
            {
                if (this.bridgeIntAcabados.SelectedIndex != -1)
                    return (RivieraAcabado)this.bridgeIntAcabados.SelectedItem;
                else
                    return default(RivieraAcabado);
            }
        }
        /// <summary>
        /// Gets the selected ext acabado.
        /// </summary>
        /// <value>
        /// The selected ext acabado.
        /// </value>
        public RivieraAcabado SelectedExtAcabado
        {
            get
            {
                if (this.bridgeExtAcabados.SelectedIndex != -1)
                    return (RivieraAcabado)this.bridgeExtAcabados.SelectedItem;
                else
                    return default(RivieraAcabado);
            }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="BridgeProperties"/> class.
        /// </summary>
        public BridgeProperties()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Initializes the specified heights.
        /// </summary>
        /// <param name="_heights">The heights.</param>
        /// <param name="_acabados">The acabados.</param>
        public void Init(IEnumerable<RivieraSize> _heights, IEnumerable<RivieraAcabado> _acabados)
        {
            this._Heights = _heights;
            this._Acabados = _acabados;
            this.bridgeHeight.ItemsSource = _Heights.Select(x => new RivieraSizeItem() { Size = x, ItemName = x.Nominal.ToString() }); ;
            this.bridgeHeight.SelectedIndex = 0;
            this.InitAcabados(this.bridgeExtAcabados, this._Acabados);
            this.InitAcabados(this.bridgeIntAcabados, this._Acabados);
        }
        /// <summary>
        /// Initializes the acabados.
        /// </summary>
        /// <param name="cbo">The cbo.</param>
        /// <param name="acabados">The acabados.</param>
        private void InitAcabados(ComboBox cbo, IEnumerable<RivieraAcabado> acabados)
        {
            cbo.ItemsSource = acabados;
            cbo.SelectedIndex = 0;
        }
        /// <summary>
        /// Handles the SelectionChanged event of the property control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs" /> instance containing the event data.</param>
        private void property_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string pName;
            Object pValue;
            if (this.bridgeExtAcabados.Name == (sender as FrameworkElement).Name)
            {
                pName = "AcabadosExterior";
                pValue = this.SelectedExtAcabado;
                if (this.SelectedExtAcabado.Acabado == null)
                    pValue = null;
            }
            else if (this.bridgeIntAcabados.Name == (sender as FrameworkElement).Name)
            {
                pName = "AcabadosInterior";
                pValue = this.SelectedIntAcabado;
                if (this.SelectedExtAcabado.Acabado == null)
                    pValue = null;
            }
            else
            {
                pName = "Altura";
                pValue = this.SelectedHeight;
            }
            if (this.PropertyChanged != null)
                this.PropertyChanged(sender, new PropertyChangedArgs(pName, pValue, e.RoutedEvent, e.RemovedItems, e.AddedItems));
        }
    }
}