using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaSoft.Riviera.Modulador.Core.Model
{
    /// <summary>
    /// Defines a riviera Element size
    /// </summary>
    public struct RivieraSize
    {
        /// <summary>
        /// The measure name
        /// </summary>
        public string Measure;
        /// <summary>
        /// The measure in nominal value inches
        /// </summary>
        public Double Nominal;
        /// <summary>
        /// The measure in real value milimiters
        /// </summary>
        public Double Real;
        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return String.Format("{0} Nominal: {1}in Real: {2}mm", this.Measure, this.Nominal, this.Real);
        }
    }
}
