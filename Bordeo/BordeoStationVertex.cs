using Autodesk.AutoCAD.DatabaseServices;
using DaSoft.Riviera.Modulador.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaSoft.Riviera.Modulador.Bordeo
{
    public class BordeoStationVertex
    {
        /// <summary>
        /// The index
        /// </summary>
        public int Index;
        /// <summary>
        /// The Entity asociated to this vertex
        /// </summary>
        public RivieraObject RivObj;
        /// <summary>
        /// True if the next entity is found at the start
        /// </summary>
        public Nullable<Boolean> IsAtStart;
        /// <summary>
        /// True if the next entity is found at the end
        /// </summary>
        public Boolean IsAtEnd => !this.IsAtStart.Value;
    }
}
