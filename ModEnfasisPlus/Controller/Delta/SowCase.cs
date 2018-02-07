using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaSoft.Riviera.OldModulador.Controller.Delta
{
    /// <summary>
    /// Define el caso de sembrado
    /// </summary>
    public enum SowCase
    {
        /// <summary>
        /// No realizar sembrado
        /// </summary>
        None = -1,
        /// <summary>
        /// Se inserta una mampara y una articulación
        /// </summary>
        Mampara_Joint = 0,
        /// <summary>
        /// Se inserta una mampara apartir de una articulacion
        /// </summary>
        MamparaFromJoint = 1,
    }
}
