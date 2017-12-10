using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaSoft.Riviera.Modulador.Core.Model.UI
{
    public class RivieraSizeItem
    {
        /// <summary>
        /// The item name
        /// </summary>
        public string ItemName;
        /// <summary>
        /// The size
        /// </summary>
        public RivieraSize Size;
        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.ItemName;
        }
    }
}
