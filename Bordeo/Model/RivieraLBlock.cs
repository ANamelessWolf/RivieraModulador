using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using DaSoft.Riviera.Modulador.Core.Controller;
using DaSoft.Riviera.Modulador.Core.Model;
using DaSoft.Riviera.Modulador.Core.Runtime;
using Nameless.Libraries.HoukagoTeaTime.Mio.Entities;
using Nameless.Libraries.HoukagoTeaTime.Mio.Model;
using Nameless.Libraries.HoukagoTeaTime.Mio.Utils;
using Nameless.Libraries.Yggdrasil.Lain;
using Nameless.Libraries.Yggdrasil.Lilith;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DaSoft.Riviera.Modulador.Core.Assets.CONST;
namespace DaSoft.Riviera.Modulador.Bordeo.Model
{
    public class RivieraLBlock : RivieraBlock
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="RivieraLBlock"/> class.
        /// </summary>
        /// <param name="blockName">Name of the block.</param>
        /// <param name="blockDirPath">The block dir path.</param>
        public RivieraLBlock(string blockName, string blockDirPath) : 
            base(blockName, blockDirPath)
        {
        }
    }
}
