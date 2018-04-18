using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace DaSoft.Riviera.Modulador.Bordeo.Model
{
    public class RivieraBridgeBlock : RivieraLinearBlock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RivieraBridgeBlock"/> class.
        /// </summary>
        /// <param name="blockName">Name of the block.</param>
        /// <param name="blockDirPath">The block dir path.</param>
        public RivieraBridgeBlock(string blockName, string blockDirPath) 
            : base(blockName, blockDirPath)
        {
        }
        /// <summary>
        /// Sets the instance content, depending on the if the application view
        /// is on 2D or 3D
        /// </summary>
        /// <param name="is2DBlock">if set to <c>true</c> [is a 2D block] otherwise a 3D block.</param>
        /// <param name="blkRef">The Block reference</param>
        /// <param name="doc">The active document.</param>
        /// <param name="tr">The active transaction.</param>
        /// <returns></returns>
        public override bool SetContent(bool is2DBlock, out BlockReference blkRef, Document doc, Transaction tr)
        {
            Nameless.Libraries.HoukagoTeaTime.Mio.Entities.AutoCADBlock instance, content;
            blkRef = null;
            Boolean blocksLoaded = LoadBlocks(doc, tr, out instance, out content, is2DBlock);
            if (blocksLoaded)
            {
                instance.Clear(tr);
                blkRef = content.CreateReference(new Point3d(), 0, 1);
                instance.Draw(tr, blkRef);
            }
            return blocksLoaded;
        }
    }
}
