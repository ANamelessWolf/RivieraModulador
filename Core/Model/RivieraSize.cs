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
        /// Implements the operator ==
        /// </summary>
        /// <param name="size1">The first size to compare.</param>
        /// <param name="size2">The second size to compare.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(RivieraSize size1, RivieraSize size2)
        {
            return size1.Nominal == size2.Nominal;
        }
        /// <summary>
        /// Implements the operator !=
        /// </summary>
        /// <param name="size1">The first size to compare.</param>
        /// <param name="size2">The second size to compare.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(RivieraSize size1, RivieraSize size2)
        {
            return size1.Nominal != size2.Nominal;
        }
        /// <summary>
        /// Implements the operator >
        /// </summary>
        /// <param name="size1">The first size to compare.</param>
        /// <param name="size2">The second size to compare.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator >(RivieraSize size1, RivieraSize size2)
        {
            return size1.Nominal > size2.Nominal;
        }
        /// <summary>
        /// Implements the operator <
        /// </summary>
        /// <param name="size1">The first size to compare.</param>
        /// <param name="size2">The second size to compare.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator <(RivieraSize size1, RivieraSize size2)
        {
            return size1.Nominal < size2.Nominal;
        }
        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="size">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object size)
        {
            if (size is RivieraSize)
                return ((RivieraSize)size).Nominal == this.Nominal;
            else
                return false;
        }
        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            var hashCode = 1486821063;
            hashCode = hashCode * -1521134295 + Nominal.GetHashCode();
            hashCode = hashCode * -1521134295 + Real.GetHashCode();
            return hashCode;
        }
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
