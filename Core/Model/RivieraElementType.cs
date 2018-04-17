using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaSoft.Riviera.Modulador.Core.Model
{
    /// <summary>
    /// Defines the type of elements
    /// </summary>
    public enum RivieraElementType
    {
        /// <summary>
        /// Unknown element
        /// </summary>
        None = -1,
        /// <summary>
        /// Panel abrev "P"
        /// </summary>
        Panel = 0,
        /// <summary>
        /// Panel a piso abrev "PP"
        /// </summary>
        Panel_Piso = 1,
        /// <summary>
        /// Pichonera abrev "PI"
        /// </summary>
        Pichonera = 2,
        /// <summary>
        /// Cajonera abrev "C"
        /// </summary>
        Cajonera = 3,
        /// <summary>
        /// Cajonera abrev "B"
        /// </summary>
        Biombo = 4,
        /// <summary>
        /// Panel Stack
        /// </summary>
        PanelStack = 5,
        /// <summary>
        /// Bridge
        /// </summary>
        Bridge = 6,
        /// <summary>
        /// Pazo_Luz
        /// </summary>
        Pazo_Luz = 7,
        /// <summary>
        /// The bridge group
        /// </summary>
        Bridge_Group = 8,
    }
}
