using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaSoft.Riviera.Modulador.Bordeo.Model.Station
{
    public enum StationDirection
    {
        /// <summary>
        /// El stack es el elemento inicial de la estación
        /// </summary>
        Initial = 0,
        /// <summary>
        /// El stack tiene la misma dirección al stack anterior
        /// </summary>
        Same = 1,
        /// <summary>
        /// El stack tiene la dirección invertida al stack anterior
        /// </summary>
        Invert = 2,
        /// <summary>
        /// El stack es el último elemento de la estación
        /// </summary>
        End = 3,
    }
}
