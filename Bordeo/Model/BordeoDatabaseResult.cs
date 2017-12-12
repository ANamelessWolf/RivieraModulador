using DaSoft.Riviera.Modulador.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaSoft.Riviera.Modulador.Bordeo.Model
{
    /// <summary>
    /// Defines the Database Bordeo Result
    /// </summary>
    public class BordeoDatabaseResult
    {
        /// <summary>
        /// The Bordeo element size collection data
        /// </summary>
        public Dictionary<String, ElementSizeCollection> Sizes;
        /// <summary>
        /// Bordeo design supported codes
        /// </summary>
        public IEnumerable<RivieraCode> Codes;
    }
}
