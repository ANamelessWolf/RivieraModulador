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
using static DaSoft.Riviera.Modulador.Bordeo.Assets.Strings;
using static DaSoft.Riviera.Modulador.Bordeo.Assets.Constants;
namespace DaSoft.Riviera.Modulador.Bordeo.Model
{
    public class RivieraLBlock : RivieraBlock
    {
        /// <summary>
        /// The Riviera block code
        /// </summary>
        public readonly string Code;
        /// <summary>
        /// The Riviera block min size
        /// </summary>
        public readonly int MinSize;
        /// <summary>
        /// The Riviera block max size
        /// </summary>
        public readonly int MaxSize;
        /// <summary>
        /// The variant block name
        /// </summary>
        public String VariantBlockName;

        /// <summary>
        /// Initializes a new instance of the <see cref="RivieraLBlock"/> class.
        /// </summary>
        /// <param name="blockName">Name of the block.</param>
        /// <param name="blockDirPath">The block dir path.</param>
        public RivieraLBlock(string blockName, string blockDirPath) :
            base(blockName, blockDirPath)
        {
            this.Code = blockName.Substring(0, 6);
            this.MinSize = int.Parse(blockName.Substring(6, 2));
            this.MaxSize = int.Parse(blockName.Substring(8, 2));
            this.VariantBlockName = String.Format("{0}{1}{2}T", this.Code, this.MaxSize, this.MinSize);
        }
        /// <summary>
        /// Gets the block file path.
        /// </summary>
        /// <param name="is2DBlock">if set to <c>true</c> [is a 2D block] otherwise a 3D block.</param>
        /// <returns>The block file path as a file Info</returns>
        public FileInfo GetBlockFilePath(String blockName, Boolean is2DBlock = true)
        {
            String pth = is2DBlock ? this.Block2DDirectoryPath : this.Block3DDirectoryPath;
            FileInfo[] files;
            if (Directory.Exists(pth))
                files = new DirectoryInfo(pth).GetFiles();
            else
                files = new FileInfo[0];
            return files.FirstOrDefault(x => x.Name.ToUpper() == String.Format("{0}.DWG", blockName).ToUpper());
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
        public bool LoadBlocks(Document doc, Transaction tr, out Dictionary<LBlockType, AutoCADBlock> blocks2D, out Dictionary<LBlockType, AutoCADBlock> blocks3D)
        {
            AutoCADBlock block2d, block3d, varBlock2d, varBlock3d;
            blocks2D = new Dictionary<LBlockType, AutoCADBlock>();
            blocks3D = new Dictionary<LBlockType, AutoCADBlock>();
            try
            {
                block2d = new AutoCADBlock(String.Format(Block2DName, BlockName), this.GetBlockFilePath(), tr);
                block3d = new AutoCADBlock(String.Format(Block3DName, BlockName), this.GetBlockFilePath(false), tr);
                if (this.MinSize != this.MaxSize)
                {
                    varBlock2d = new AutoCADBlock(String.Format(Block2DName, this.VariantBlockName), this.GetBlockFilePath(this.VariantBlockName), tr);
                    varBlock3d = new AutoCADBlock(String.Format(Block3DName, this.VariantBlockName), this.GetBlockFilePath(this.VariantBlockName, false), tr);
                    //Registros 2D
                    blocks2D.Add(LBlockType.LEFT_START_MIN_SIZE, new AutoCADBlock(String.Format(PREFIX_BLOCK_CONT, this.BlockName, "2D", BLOCK_DIR_LFT), tr));
                    blocks2D.Add(LBlockType.LEFT_START_MIN_SIZE, new AutoCADBlock(String.Format(PREFIX_BLOCK_CONT, this.BlockName, "2D", BLOCK_DIR_RGT), tr));
                    blocks2D.Add(LBlockType.LEFT_START_MIN_SIZE, new AutoCADBlock(String.Format(PREFIX_BLOCK_VAR_CONT, this.VariantBlockName, "2D", BLOCK_DIR_LFT), tr));
                    blocks2D.Add(LBlockType.LEFT_START_MIN_SIZE, new AutoCADBlock(String.Format(PREFIX_BLOCK_VAR_CONT, this.VariantBlockName, "2D", BLOCK_DIR_RGT), tr));
                    //Registros 3D
                    blocks3D.Add(LBlockType.LEFT_START_MIN_SIZE, new AutoCADBlock(String.Format(PREFIX_BLOCK_CONT, this.BlockName, "3D", BLOCK_DIR_LFT), tr));
                    blocks3D.Add(LBlockType.LEFT_START_MIN_SIZE, new AutoCADBlock(String.Format(PREFIX_BLOCK_CONT, this.BlockName, "3D", BLOCK_DIR_RGT), tr));
                    blocks3D.Add(LBlockType.LEFT_START_MIN_SIZE, new AutoCADBlock(String.Format(PREFIX_BLOCK_VAR_CONT, this.VariantBlockName, "3D", BLOCK_DIR_LFT), tr));
                    blocks3D.Add(LBlockType.LEFT_START_MIN_SIZE, new AutoCADBlock(String.Format(PREFIX_BLOCK_VAR_CONT, this.VariantBlockName, "3D", BLOCK_DIR_RGT), tr));
                    this.InitContent(blocks2D, blocks3D, block2d, block3d, varBlock2d, varBlock3d);
                }
                else
                {
                    varBlock2d = new AutoCADBlock(String.Format(Block2DName, this.VariantBlockName), this.GetBlockFilePath(this.VariantBlockName), tr);
                    varBlock3d = new AutoCADBlock(String.Format(Block3DName, this.VariantBlockName), this.GetBlockFilePath(this.VariantBlockName, false), tr);
                    //Registros 2D
                    blocks2D.Add(LBlockType.LEFT_SAME_SIZE, new AutoCADBlock(String.Format(PREFIX_BLOCK_CONT, this.BlockName, "2D", BLOCK_DIR_LFT), tr));
                    blocks2D.Add(LBlockType.RIGHT_SAME_SIZE, new AutoCADBlock(String.Format(PREFIX_BLOCK_CONT, this.BlockName, "2D", BLOCK_DIR_RGT), tr));
                    //Registros 3D
                    blocks3D.Add(LBlockType.LEFT_SAME_SIZE, new AutoCADBlock(String.Format(PREFIX_BLOCK_CONT, this.BlockName, "3D", BLOCK_DIR_LFT), tr));
                    blocks3D.Add(LBlockType.RIGHT_SAME_SIZE, new AutoCADBlock(String.Format(PREFIX_BLOCK_CONT, this.BlockName, "3D", BLOCK_DIR_RGT), tr));
                    this.InitContent(blocks2D, blocks3D, block2d, block3d);
                }
            }
            catch (Exception exc)
            {
                string msg = String.Format(ERR_LOADING_BLOCK, this.BlockName);
                msg = NamelessUtils.FormatExceptionMessage(exc, msg);
                App.Riviera.Log.AppendEntry(msg, Protocol.Error, "LoadBlocks", "RivieraLBlock");
                throw exc.CreateNamelessException<RivieraException>(msg);
            }
            return true;
        }

        private void InitContent(Dictionary<LBlockType, AutoCADBlock> blocks2D, Dictionary<LBlockType, AutoCADBlock> blocks3D, AutoCADBlock block2d, AutoCADBlock block3d, AutoCADBlock varBlock2d= null, AutoCADBlock varBlock3d = null)
        {
            throw new NotImplementedException();
        }







        /// <summary>
        /// Sets the instance content, depending on the if the application view
        /// is on 2D or 3D
        /// </summary>
        /// <param name="is2DBlock">if set to <c>true</c> [is a 2D block] otherwise a 3D block.</param>
        /// <param name="doc">The active document.</param>
        /// <param name="tr">The active transaction.</param>
        public void SetContent(LBlockType block, Boolean is2DBlock, Document doc, Transaction tr)
        {
            Dictionary<LBlockType, AutoCADBlock> blocks;
            AutoCADBlock instance;
            BlockReference blkRef;
            if (LoadBlocks(doc, tr, out instance, out instance, is2DBlock))
            {
                instance.Clear(tr);
                blkRef = content.CreateReference(new Point3d(), 0, 1);
                instance.Draw(tr, blkRef);
            }
        }
    }
}
