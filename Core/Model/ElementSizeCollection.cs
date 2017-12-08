using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaSoft.Riviera.Modulador.Core.Model
{
    /// <summary>
    /// The element size collection
    /// </summary>
    public class ElementSizeCollection
    {
        /// <summary>
        /// The Riviera code
        /// </summary>
        public string Code;
        /// <summary>
        /// The available sizes for the Riviera asigned code
        /// </summary>
        public List<RivieraMeasure> Sizes;
        /// <summary>
        /// Initializes a new instance of the <see cref="ElementSizeCollection"/> class.
        /// </summary>
        /// <param name="code">The riviera code.</param>
        public ElementSizeCollection(string code)
        {
            this.Sizes = new List<RivieraMeasure>();
            this.Code = code;
        }
        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return String.Format("Tamaño: {0}", this.Sizes.Count);
        }
    }
}
