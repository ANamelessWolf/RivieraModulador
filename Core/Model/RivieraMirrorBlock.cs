using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaSoft.Riviera.Modulador.Core.Model
{
    public class RivieraMirrorBlock : RivieraBlock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RivieraMirrorBlock"/> class.
        /// </summary>
        /// <param name="blockName">Name of the block.</param>
        /// <param name="blockDirPath">The block dir path.</param>
        public RivieraMirrorBlock(string blockName, string blockDirPath) : 
            base(blockName, blockDirPath)
        {
        }
    }
}
