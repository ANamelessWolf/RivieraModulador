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
        /// Gets the <see cref="RivieraSize"/> with the specified key.
        /// </summary>
        /// <value>
        /// The <see cref="RivieraSize"/>.
        /// </value>
        /// <param name="key">The key.</param>
        /// <returns>The Riviera size associated to a key</returns>
        public RivieraSize this[String key] => Sizes.FirstOrDefault(x => x.Measure == key);
        /// <summary>
        /// Determines whether the specified size exists on the measure
        /// </summary>
        /// <param name="sizes">The sizes to check.</param>
        /// <returns>
        ///   <c>true</c> if the specified sizes exists on the measure; otherwise, <c>false</c>.
        /// </returns>
        public Boolean HasSize(params KeyValuePair<string, double>[] sizes)
        {
            Boolean flag = true;
            for (int i = 0; i < sizes.Length && flag; i++)
                flag = flag && (Sizes.FirstOrDefault(x => x.Measure == sizes[i].Key && x.Nominal == sizes[i].Value).Measure != null);
            return flag;
        }
        /// <summary>
        /// The Riviera element measures
        /// </summary>
        protected List<RivieraSize> Sizes;
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
        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var size in this.Sizes)
                sb.Append(String.Format("{0}: {1}\" || ", size.Measure, size.Nominal));
            return sb.ToString().Substring(0, sb.Length - 4);
        }
    }
}
