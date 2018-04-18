using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using DaSoft.Riviera.Modulador.Core.Controller;
using DaSoft.Riviera.Modulador.Core.Runtime;
using Nameless.Libraries.HoukagoTeaTime.Mio.Entities;
using Nameless.Libraries.HoukagoTeaTime.Mio.Model;
using Nameless.Libraries.HoukagoTeaTime.Mio.Utils;
using Nameless.Libraries.HoukagoTeaTime.Ritsu.Utils;
using Nameless.Libraries.Yggdrasil.Lain;
using Nameless.Libraries.Yggdrasil.Lilith;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DaSoft.Riviera.Modulador.Core.Assets.CONST;
using static DaSoft.Riviera.Modulador.Core.Assets.Strings;
namespace DaSoft.Riviera.Modulador.Core.Model
{
    /// <summary>
    /// Represents a Riviera Block that can be drawn in 2D and 3D
    /// </summary>
    public class RivieraBlock
    {
        /// <summary>
        /// The name of the block to manage
        /// </summary>
        public String BlockName;
        /// <summary>
        /// The block directory path
        /// </summary>
        public String BlockDirectoryPath;
        /// <summary>
        /// Gets the name of the blocks from which the Block
        /// Reference will be created.
        /// </summary>
        /// <value>
        /// The name of the instance block.
        /// </value>
        public String InstanceBlockName => String.Format(PREFIX_BLOCK, BlockName);
        /// <summary>
        /// The block 2D block name
        /// </summary>
        public String Block2DName => String.Format(SUFFIX_BLOCK2D, BlockName);
        /// <summary>
        /// The block 2D block name
        /// </summary>
        public String Block3DName => String.Format(SUFFIX_BLOCK3D, BlockName);
        /// <summary>
        /// Gets the blocks 2D directory path.
        /// </summary>
        /// <value>
        /// The block 2D directory path.
        /// </value>
        public String Block2DDirectoryPath => Path.Combine(this.BlockDirectoryPath, FOLDER_2D);
        /// <summary>
        /// Gets the blocks 3D directory path.
        /// </summary>
        /// <value>
        /// The block 3D directory path.
        /// </value>
        public String Block3DDirectoryPath => Path.Combine(this.BlockDirectoryPath, FOLDER_3D);
        /// <summary>
        /// Initializes a new instance of the <see cref="RivieraBlock"/> class.
        /// </summary>
        /// <param name="blockName">Name of the block.</param>
        /// <param name="blockDirPath">The block dir path.</param>
        public RivieraBlock(String blockName, String blockDirPath)
        {
            this.BlockName = blockName;
            this.BlockDirectoryPath = blockDirPath;
        }
        /// <summary>
        /// Gets the block file path.
        /// </summary>
        /// <param name="is2DBlock">if set to <c>true</c> [is a 2D block] otherwise a 3D block.</param>
        /// <returns>The block file path as a file Info</returns>
        public FileInfo GetBlockFilePath(Boolean is2DBlock = true)
        {
            String pth = is2DBlock ? this.Block2DDirectoryPath : this.Block3DDirectoryPath;
            FileInfo[] files;

            if (Directory.Exists(pth))
            {
                Nameless.Libraries.Yggdrasil.Aerith.AerithScanner scn = new Nameless.Libraries.Yggdrasil.Aerith.AerithScanner(pth, true);
                scn.Find();
                files = scn.Files;
            }
            else
                files = new FileInfo[0];
            return files.FirstOrDefault(x => x.Name.ToUpper() == String.Format("{0}.DWG", this.BlockName).ToUpper());
        }
        /// <summary>
        /// Loads the blocks.
        /// </summary>
        /// <param name="doc">The active document.</param>
        /// <param name="tr">The active transaction.</param>
        /// <param name="instance">The instance block</param>
        /// <param name="content">The block content</param>
        /// <param name="is2DBlock">if set to <c>true</c> [is a 2D block] otherwise a 3D block.</param>
        /// <returns></returns>
        public Boolean LoadBlocks(Document doc, Transaction tr, out AutoCADBlock instance, out AutoCADBlock content, Boolean is2DBlock = true)
        {
            BlockTable blockTable = (BlockTable)doc.Database.BlockTableId.GetObject(OpenMode.ForRead);
            instance = null;
            content = null;
            AutoCADBlock block2d, block3d;
            try
            {
                //Esta línea prueba de manerá local la carga de un bloque
                //this.Block2DName._LoadBlock(this.GetBlockFilePath().FullName, tr);
                instance = new AutoCADBlock(this.InstanceBlockName, tr);
                block2d = new AutoCADBlock(this.Block2DName, this.GetBlockFilePath(), tr);
                block3d = new AutoCADBlock(this.Block3DName, this.GetBlockFilePath(false), tr);
                content = is2DBlock ? block2d : block3d;
            }
            catch (Exception exc)
            {
                string msg = String.Format(ERR_LOADING_BLOCK, this.BlockName);
                msg = NamelessUtils.FormatExceptionMessage(exc, msg);
                App.Riviera.Log.AppendEntry(msg, Protocol.Error, "LoadBlocks", "RivieraBlock");
                throw exc.CreateNamelessException<RivieraException>(msg);
            }
            return true;
        }
        /// <summary>
        /// Sets the instance content, depending on the if the application view
        /// is on 2D or 3D
        /// </summary>
        /// <param name="is2DBlock">if set to <c>true</c> [is a 2D block] otherwise a 3D block.</param>
        /// <param name="blkRef">The Block reference</param>
        /// <param name="doc">The active document.</param>
        /// <param name="tr">The active transaction.</param>
        public virtual Boolean SetContent(Boolean is2DBlock, out BlockReference blkRef, Document doc, Transaction tr)
        {
            AutoCADBlock instance, content;
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
        /// <summary>
        /// Inserts this instance block as a BlockReference.
        /// </summary>
        /// <param name="doc">The active document.</param>
        /// <param name="tr">The active transaction.</param>
        /// <param name="insPt">The insertion point.</param>
        /// <param name="angle">The block rotation.</param>
        /// <param name="scale">The block scale.</param>
        /// <returns>The Block Reference</returns>
        public BlockReference Insert(Document doc, Transaction tr, Point3d insPt, Double angle = 0, Double scale = 1)
        {
            BlockTable blockTable = doc.Database.BlockTableId.GetObject(OpenMode.ForRead) as BlockTable;
            AutoCADBlock instance = null, content;
            BlockReference blockContent;
            Boolean is2DBlock = !App.Riviera.Is3DEnabled;
            if (!blockTable.Has(this.InstanceBlockName) && this.LoadBlocks(doc, tr, out instance, out content, is2DBlock))
                this.SetContent(is2DBlock, out blockContent, doc, tr);
            else
            {
                this.LoadBlocks(doc, tr, out instance, out content, is2DBlock);
                instance = new AutoCADBlock(this.InstanceBlockName, tr);
            }
            if (instance != null)
            {
                BlockReference blkRef = instance.CreateReference(insPt, angle, scale);
                BlockTableRecord modelSpace = tr.GetModelSpace();
                blkRef.Draw(modelSpace, tr);
                AutoCADLayer lay = new AutoCADLayer(LAYER_RIVIERA_GEOMETRY, tr);
                lay.SetStatus(LayerStatus.EnableStatus, tr);
                blkRef.UpgradeOpen();
                blkRef.Layer = lay.Layername;
                return blkRef;
            }
            else
                throw new RivieraException(String.Format(ERR_LOADING_BLOCK, this.BlockName));
        }
    }
}
