using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaSoft.Riviera.Modulador.Core.Model
{
    public class RivieraDescription
    {
        public String ClassName;
        /// <summary>
        /// The connections
        /// </summary>
        public List<RivieraConnection> Connections;
        /// <summary>
        /// Initializes a new instance of the <see cref="RivieraDescription"/> class.
        /// </summary>
        public RivieraDescription()
        {
            this.Connections = new List<RivieraConnection>();
        }
    }
}
