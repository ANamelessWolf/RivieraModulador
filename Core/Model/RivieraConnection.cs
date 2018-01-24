using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaSoft.Riviera.Modulador.Core.Model
{
    public struct RivieraConnection
    {
        /// <summary>
        /// The direction in wich the element is connected
        /// </summary>
        public String Key;
        /// <summary>
        /// The direction in wich the element is connected
        /// </summary>
        public String Direction;
        /// <summary>
        /// The block name connected in the given direction
        /// </summary>
        public String BlockName;
    }
}
