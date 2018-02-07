using DaSoft.Riviera.OldModulador.Model.Delta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaSoft.Riviera.OldModulador.Model
{
    public class RivieraPanelBiombo : RivieraPanel
    {
        /// <summary>
        /// Iniicializa una instancia del panel 54 <see cref="RivieraPanelBiombo"/> class.
        /// </summary>
        /// <param name="mampara">La mampara seleccionada.</param>
        /// <param name="panel">El panel a insertar en la mampara.</param>
        /// <param name="upperPanel">El panel superior.</param>
        /// <param name="location">La ubicación de los paneles.</param>
        public RivieraPanelBiombo(Mampara mampara, PanelRaw panel, RivieraPanelDoubleLocation location) :
            base(mampara, panel, location.GetDoubleSize(mampara, panel), location)
        {

        }
    }
}
