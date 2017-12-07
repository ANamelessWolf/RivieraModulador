using DaSoft.Riviera.Modulador.Core.Model.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaSoft.Riviera.Modulador.Core.Model
{
    /// <summary>
    /// Defines the result for initializing the Riviera Database
    /// </summary>
    public class RivieraDatabaseResult
    {
        /// <summary>
        /// The riviera code rows collection result
        /// </summary>
        public IEnumerable<RivieraCodeRow> RivieraCodeRows;
        /// <summary>
        /// The riviera lines design result
        /// </summary>
        public Dictionary<DesignLine, Object> DesignResult;
        /// <summary>
        /// Initializes a new instance of the <see cref="RivieraDatabaseResult"/> class.
        /// </summary>
        public RivieraDatabaseResult()
        {
            this.DesignResult = new Dictionary<DesignLine, object>();
        }
    }
}
