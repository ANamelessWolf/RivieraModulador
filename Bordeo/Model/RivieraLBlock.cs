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
using static DaSoft.Riviera.Modulador.Bordeo.Assets.Codes;
using DaSoft.Riviera.Modulador.Bordeo.Controller;

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
        /// The Riviera block max size
        /// </summary>
        public readonly int Height;
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
            this.Height = int.Parse(blockName.Substring(10, 2));
            this.VariantBlockName = String.Format("{0}{1}{2}{3}T", this.Code, this.MaxSize, this.MinSize, this.Height);
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
                    varBlock2d = new AutoCADBlock(String.Format(SUFFIX_BLOCK2D, this.VariantBlockName), this.GetBlockFilePath(this.VariantBlockName), tr);
                    varBlock3d = new AutoCADBlock(String.Format(SUFFIX_BLOCK3D, this.VariantBlockName), this.GetBlockFilePath(this.VariantBlockName, false), tr);
                    //Registros 2D
                    blocks2D.Add(LBlockType.LEFT_START_MIN_SIZE, new AutoCADBlock(String.Format(PREFIX_BLOCK_CONT, this.BlockName, "2D", BLOCK_DIR_LFT), tr));
                    blocks2D.Add(LBlockType.RIGHT_START_MIN_SIZE, new AutoCADBlock(String.Format(PREFIX_BLOCK_CONT, this.BlockName, "2D", BLOCK_DIR_RGT), tr));
                    blocks2D.Add(LBlockType.LEFT_START_MAX_SIZE, new AutoCADBlock(String.Format(PREFIX_BLOCK_VAR_CONT, this.VariantBlockName, "2D", BLOCK_DIR_LFT), tr));
                    blocks2D.Add(LBlockType.RIGHT_START_MAX_SIZE, new AutoCADBlock(String.Format(PREFIX_BLOCK_VAR_CONT, this.VariantBlockName, "2D", BLOCK_DIR_RGT), tr));
                    //Registros 3D
                    blocks3D.Add(LBlockType.LEFT_START_MIN_SIZE, new AutoCADBlock(String.Format(PREFIX_BLOCK_CONT, this.BlockName, "3D", BLOCK_DIR_LFT), tr));
                    blocks3D.Add(LBlockType.RIGHT_START_MIN_SIZE, new AutoCADBlock(String.Format(PREFIX_BLOCK_CONT, this.BlockName, "3D", BLOCK_DIR_RGT), tr));
                    blocks3D.Add(LBlockType.LEFT_START_MAX_SIZE, new AutoCADBlock(String.Format(PREFIX_BLOCK_VAR_CONT, this.VariantBlockName, "3D", BLOCK_DIR_LFT), tr));
                    blocks3D.Add(LBlockType.RIGHT_START_MAX_SIZE, new AutoCADBlock(String.Format(PREFIX_BLOCK_VAR_CONT, this.VariantBlockName, "3D", BLOCK_DIR_RGT), tr));
                    this.InitContent(tr, blocks2D, blocks3D, block2d, block3d, varBlock2d, varBlock3d);
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
                    this.InitContent(tr, blocks2D, blocks3D, block2d, block3d);
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
        /// <summary>
        /// Initializes block content.
        /// </summary>
        /// <param name="blocks2D">The 2D blocks.</param>
        /// <param name="blocks3D">The 3D blocks.</param>
        /// <param name="block2d">The normal 2D block.</param>
        /// <param name="block3d">The normal 3D block</param>
        /// <param name="varBlock2d">The variant 2D block.</param>
        /// <param name="varBlock3d">The variant 3D block</param>
        private void InitContent(Transaction tr, Dictionary<LBlockType, AutoCADBlock> blocks2D, Dictionary<LBlockType, AutoCADBlock> blocks3D, AutoCADBlock block2d, AutoCADBlock block3d, AutoCADBlock varBlock2d = null, AutoCADBlock varBlock3d = null)
        {
            string blockName = block2d.Blockname.Substring(0, block2d.Blockname.Length - 2),
                   variantBlockName = varBlock2d != null ? varBlock2d.Blockname.Substring(0, varBlock2d.Blockname.Length - 2) : null;
            if (variantBlockName != null)
            {
                //Bloques 2D
                this.DrawIn(tr, blocks2D[LBlockType.RIGHT_START_MIN_SIZE], block2d.CreateReference(new Point3d(), 0));
                this.DrawIn(tr, blocks2D[LBlockType.RIGHT_START_MAX_SIZE], varBlock2d.CreateReference(new Point3d(), 0));
                this.DrawIn(tr, blocks2D[LBlockType.LEFT_START_MIN_SIZE], this.CreateLeftReference(this.VariantBlockName, varBlock2d));
                this.DrawIn(tr, blocks2D[LBlockType.LEFT_START_MAX_SIZE], this.CreateLeftReference(this.VariantBlockName, varBlock2d));
                //Bloques 3D
                this.DrawIn(tr, blocks3D[LBlockType.RIGHT_START_MIN_SIZE], block3d.CreateReference(new Point3d(), 0), true);
                this.DrawIn(tr, blocks3D[LBlockType.RIGHT_START_MAX_SIZE], varBlock3d.CreateReference(new Point3d(), 0), true);
                this.DrawIn(tr, blocks3D[LBlockType.LEFT_START_MIN_SIZE], this.CreateLeftReference(this.BlockName, varBlock3d), true);
                this.DrawIn(tr, blocks3D[LBlockType.LEFT_START_MAX_SIZE], this.CreateLeftReference(this.VariantBlockName, varBlock3d), true);
            }
            else
            {
                //Bloques 2D
                this.DrawIn(tr, blocks2D[LBlockType.RIGHT_SAME_SIZE], block3d.CreateReference(new Point3d(), 0));
                this.DrawIn(tr, blocks2D[LBlockType.LEFT_SAME_SIZE], this.CreateLeftReference(this.VariantBlockName, block3d));
                //Bloques 3D
                this.DrawIn(tr, blocks3D[LBlockType.RIGHT_SAME_SIZE], block3d.CreateReference(new Point3d(), 0));
                this.DrawIn(tr, blocks3D[LBlockType.LEFT_SAME_SIZE], this.CreateLeftReference(this.VariantBlockName, block3d));
            }
        }
        /// <summary>
        /// Draws the in the block record the block reference only if the block record is empty.
        /// </summary>
        /// <param name="tr">The active transaction.</param>
        /// <param name="blockRecord">The block table record.</param>
        /// <param name="blkRef">The block reference.</param>
        /// <param name="is3dBlock">if set to <c>true</c> [is a 3d block].</param>
        private void DrawIn(Transaction tr, AutoCADBlock blockRecord, BlockReference blkRef, Boolean is3dBlock = false)
        {
            blockRecord.Open(tr, OpenMode.ForWrite);
            if (blockRecord.Block.OfType<ObjectId>().Count() == 0)
            {
                if (is3dBlock)
                    blkRef.TransformBy(Matrix3d.Rotation(Math.PI / 2, Vector3d.XAxis, new Point3d()));
                blockRecord.Draw(tr, blkRef);
            }
        }

        /// <summary>
        /// Creates the left reference.
        /// </summary>
        /// <param name="blockName">Name of the block.</param>
        /// <param name="block">The block.</param>
        /// <returns></returns>
        private BlockReference CreateLeftReference(string blockName, AutoCADBlock block)
        {
            String code = blockName.Substring(0, 6);
            double frente1 = int.Parse(blockName.Substring(6, 2)),
                frente2 = int.Parse(blockName.Substring(8, 2));
            Double f1, f2;
            Vector3d offset = new Vector3d();
            //Se rota 270°
            BlockReference blkRef = block.CreateReference(new Point3d(), 0);
            blkRef.TransformBy(Matrix3d.Rotation(3 * Math.PI / 2, Vector3d.ZAxis, new Point3d()));
            //Offset BR2020
            if (code == CODE_PANEL_90)
            {
                f1 = frente1.GetPanel90DrawingSize();
                f2 = frente2.GetPanel90DrawingSize();
                offset = new Vector3d(f2, f1, 0);
                offset = new Vector3d(offset.X + 0.1002d, offset.Y + 0.1002d, 0);
            }
            //Offset BR2030
            else
            {
                f1 = frente1.GetPanel135DrawingSize();
                f2 = frente2.GetPanel135DrawingSize();
                offset = new Vector3d(f2, f1, 0);
                offset = new Vector3d(offset.X + 0.07085210d, offset.Y + 0.02934790d, 0);
            }
            //Se desplaza en el punto final al inicial del bloque
            blkRef.TransformBy(Matrix3d.Displacement(new Vector3d(offset.X, offset.Y, 0)));
            return blkRef;
        }
        /// <summary>
        /// Sets the instance content, depending on the if the application view
        /// is on 2D or 3D
        /// </summary>
        /// <param name="is2DBlock">if set to <c>true</c> [is a 2D block] otherwise a 3D block.</param>
        /// <param name="doc">The active document.</param>
        /// <param name="tr">The active transaction.</param>
        public AutoCADBlock SetContent(LBlockType block, Boolean is2DBlock, Document doc, Transaction tr)
        {
            Dictionary<LBlockType, AutoCADBlock> blocks2d, blocks3d;
            AutoCADBlock instance = null;
            BlockReference blkRef;
            if (LoadBlocks(doc, tr, out blocks2d, out blocks3d))
            {
                instance = new AutoCADBlock(this.GetInstanceName(block), tr);
                instance.Clear(tr);
                if (is2DBlock)
                    blkRef = blocks2d[block].CreateReference(new Point3d(), 0, 1);
                else
                    blkRef = blocks3d[block].CreateReference(new Point3d(), 0, 1);
                instance.Draw(tr, blkRef);
            }
            return instance;
        }
        /// <summary>
        /// Gets the name of the instance.
        /// </summary>
        /// <param name="block">The block direction.</param>
        /// <returns>The Instance name</returns>
        public string GetInstanceName(LBlockType blockDirection)
        {
            string code = this.BlockName.Substring(0, 6);
            int frente1 = int.Parse(this.BlockName.Substring(6, 2)),
                frente2 = int.Parse(this.BlockName.Substring(8, 2)),
                alto = int.Parse(this.BlockName.Substring(10, 2));

            if (blockDirection == LBlockType.LEFT_START_MAX_SIZE || blockDirection == LBlockType.RIGHT_START_MAX_SIZE)
            {
                int max = frente1 > frente2 ? frente1 : frente2,
                    min = frente1 < frente2 ? frente1 : frente2;
                frente1 = max;
                frente2 = min;
            }
            else
            {
                int max = frente1 > frente2 ? frente1 : frente2,
                    min = frente1 < frente2 ? frente1 : frente2;
                frente1 = min;
                frente2 = max;
            }

            code = String.Format("{0}{1}{2}{3}T", code, frente1, frente2, alto);
            string dir = new LBlockType[] { LBlockType.RIGHT_SAME_SIZE, LBlockType.RIGHT_START_MAX_SIZE, LBlockType.RIGHT_START_MIN_SIZE }.Contains(blockDirection) ? BLOCK_DIR_RGT : BLOCK_DIR_LFT;
            return String.Format(PREFIX_BLOCK_INST, code, dir);
        }

        /// <summary>
        /// Inserts this instance block as a BlockReference.
        /// </summary>
        /// <param name="doc">The active document.</param>
        /// <param name="tr">The active transaction.</param>
        /// <param name="insPt">The insertion point.</param>
        /// <param name="angle">The block rotation.</param>
        /// <param name="scale">The block scale.</param>
        /// <returns>
        /// The Block Reference
        /// </returns>
        /// <exception cref="RivieraException">Si existe un error al insertar el bloque</exception>
        public BlockReference Insert(Document doc, Transaction tr, LBlockType blockDir, Point3d insPt, double angle = 0, double scale = 1)
        {
            BlockTable blockTable = doc.Database.BlockTableId.GetObject(OpenMode.ForRead) as BlockTable;
            AutoCADBlock instance;
            Boolean is2DBlock = !App.Riviera.Is3DEnabled;
            String instanceName = this.GetInstanceName(blockDir);
            if (!blockTable.Has(instanceName))
                instance = this.SetContent(blockDir, is2DBlock, doc, tr);
            else
            {
                Dictionary<LBlockType, AutoCADBlock> blocks2d, blocks3d;
                this.LoadBlocks(doc, tr, out blocks2d, out blocks3d);
                instance = new AutoCADBlock(instanceName, tr);
            }
            //Se realizá la inserción de la instancia
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
