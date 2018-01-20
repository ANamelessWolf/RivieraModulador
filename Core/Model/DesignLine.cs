using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaSoft.Riviera.Modulador.Core.Model
{
    /// <summary>
    /// The Riviera Modulador supported design lines
    /// "Líneas de diseño"
    /// </summary>
    public enum DesignLine
    {
        /// <summary>
        /// Unknown Design line
        /// </summary>
        None = -1,
        /// <summary>
        /// Any Design line abrev "TODAS"
        /// </summary>
        Any = 0,
        /// <summary>
        /// Enfasis plus design line abrev "EP"
        /// </summary>
        Enfasis_Plus = 1,
        /// <summary>
        /// Bordeo design line abrev "BD"
        /// </summary>
        Bordeo = 2,
    }
}
