using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaSoft.Riviera.Modulador.Core.Model
{
    /// <summary>
    /// Defines a Riviera Measure asociated to Riviera element
    /// </summary>
    public abstract class RivieraMeasure
    {
        /// <summary>
        /// The Riviera element measures
        /// </summary>
        public List<RivieraSize> Sizes;
        /// <summary>
        /// Initializes a new instance of the <see cref="RivieraMeasure"/> class.
        /// </summary>
        /// <param name="sizes">The sizes that defines the elment</param>
        public RivieraMeasure(params RivieraSize[] sizes)
        {
            this.Sizes = new List<RivieraSize>();
            foreach (var size in sizes)
                this.Sizes.Add(size);
        }
    }
}
