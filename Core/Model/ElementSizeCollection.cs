using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaSoft.Riviera.Modulador.Core.Model
{
    public class ElementSizeCollection
    {
        /// <summary>
        /// The Riviera code
        /// </summary>
        public string Code;
        /// <summary>
        /// The available sizes for the Riviera asigned code
        /// </summary>
        public IEnumerable<RivieraMeasure> Sizes;
    }
}
