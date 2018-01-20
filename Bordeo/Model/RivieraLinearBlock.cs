using DaSoft.Riviera.Modulador.Core.Model;
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
    public class RivieraLinearBlock : RivieraBlock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RivieraLinearBlock"/> class.
        /// </summary>
        /// <param name="blockName">Name of the block.</param>
        /// <param name="blockDirPath">The block dir path.</param>
        public RivieraLinearBlock(string blockName, string blockDirPath) : base(blockName, blockDirPath)
        {
        }
        /// <summary>
        /// Sets the instance content, depending on the if the application view
        /// is on 2D or 3D
        /// </summary>
        /// <param name="is2DBlock">if set to <c>true</c> [is a 2D block] otherwise a 3D block.</param>
        /// <param name="doc">The active document.</param>
        /// <param name="blkRef">The Block reference</param>
        /// <param name="tr">The active transaction.</param>
        public override bool SetContent(Boolean is2DBlock, out BlockReference blkRef, Document doc, Transaction tr)
        {
            Boolean blockIsLoaded = base.SetContent(is2DBlock, out blkRef, doc, tr);
            if (blockIsLoaded && !is2DBlock)
            {
                //Se rota el bloque para la vista 3D
                Point3d insPoint = blkRef.Position;
                Vector3d v = Vector3d.XAxis;
                blkRef.TransformBy(Matrix3d.Rotation(Math.PI / 2, v, blkRef.Position));
            }
            return blockIsLoaded;
        }
    }
}
