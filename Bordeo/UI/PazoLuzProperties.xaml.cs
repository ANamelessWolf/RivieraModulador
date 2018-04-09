using DaSoft.Riviera.Modulador.Bordeo.Controller;
using DaSoft.Riviera.Modulador.Core.Model;
using DaSoft.Riviera.Modulador.Core.Model.DB;
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
    /// Interaction logic for PazoLuzProperties.xaml
    /// </summary>
    public partial class PazoLuzProperties : UserControl
    {
        /// <summary>
        /// Occurs when [property changed].
        /// </summary>
        public event SelectionChangedEventHandler PropertyChanged;
        /// <summary>
        /// The acabados
        /// </summary>
        IEnumerable<RivieraAcabado> _Acabados;

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
                if (this.intAcabados.SelectedIndex != -1)
                    return (RivieraAcabado)this.intAcabados.SelectedItem;
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
                if (this.extAcabados.SelectedIndex != -1)
                    return (RivieraAcabado)this.extAcabados.SelectedItem;
                else
                    return default(RivieraAcabado);
            }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="BridgeProperties"/> class.
        /// </summary>
        public PazoLuzProperties()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Initializes the specified heights.
        /// </summary>
        /// <param name="_acabados">The acabados.</param>
        public void Init(IEnumerable<RivieraAcabado> _acabados)
        {
            this._Acabados = _acabados;
            this.InitAcabados(this.extAcabados, this._Acabados);
            this.InitAcabados(this.intAcabados, this._Acabados);
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
            if (this.extAcabados.Name == (sender as FrameworkElement).Name)
            {
                pName = "AcabadosExterior";
                pValue = this.SelectedExtAcabado;
            }
            else 
            {
                pName = "AcabadosInterior";
                pValue = this.SelectedIntAcabado;
            }
            if (this.PropertyChanged != null)
                this.PropertyChanged(sender, new PropertyChangedArgs(pName, pValue, e.RoutedEvent, e.RemovedItems, e.AddedItems));
        }
    }
}