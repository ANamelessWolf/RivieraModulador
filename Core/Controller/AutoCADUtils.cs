using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using DaSoft.Riviera.Modulador.Core.Runtime;
using Nameless.Libraries.HoukagoTeaTime.Mio.Entities;
using Nameless.Libraries.HoukagoTeaTime.Runtime;
using Nameless.Libraries.HoukagoTeaTime.Tsumugi;
using Nameless.Libraries.Yggdrasil.Lain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Nameless.Libraries.HoukagoTeaTime.Mio.Utils.MioUtils;
namespace DaSoft.Riviera.Modulador.Core.Controller
{
    public static class AutoCADUtils
    {
        public const String KEY_ID = "ID";
        /// <summary>
        /// Sets the specified field.
        /// </summary>
        /// <param name="dMan">The extension dictionary manager.</param>
        /// <param name="field">The field name.</param>
        /// <param name="value">The string value.</param>
        /// <param name="tr">The active transaction</param>
        public static void Set(this ExtensionDictionaryManager dMan, Transaction tr, String field, params string[] values)
        {
            dMan.AddXRecord(field, tr).SetData(tr, values);
        }
        /// <summary>
        /// Blocks the content.
        /// </summary>
        /// <param name="obj">The Riviera object that has a Block Reference as content.</param>
        /// <param name="code">The Block code name.</param>
        /// <param name="tr">The Active transaction</param>
        /// <returns>True if the block content is loaded</returns>
        public static Boolean BlockContent(this IBlockObject obj, String code, Transaction tr)
        {
            FileInfo block;
            String blockName;
            //1: Se realiza la selección del bloque segun el modo que se encuentre activo
            obj.ExtractBlockData(code, out block, out blockName);
            //2: Se realiza la carga de los bloques
            if (block != null && File.Exists(block.FullName))
            {
                //Este es el bloque cargado en el archivo, se va a dibujar en 
                //el bloque contenedor fantasma, codigo y prefijo SPACE_
                try
                {
                    obj.Block = new AutoCADBlock(blockName, block, tr);
                    return true;
                }
                catch (Exception exc)
                {
                    Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("Error al cargar el bloque {0}\n{1}", blockName, exc.Message);
                    App.Riviera.Log.AppendEntry(exc.Message, Protocol.Error, "BlockContent");
                    return false;
                }

            }
            else
                return false;
        }
        /// <summary>
        /// Extracts the block data.
        /// </summary>
        /// <param name="obj">The Riviera object that has a Block Reference as content.</param>
        /// <param name="code">The Block code name.</param>
        /// <param name="block">The block file name.</param>
        /// <param name="blockName">The block name to load.</param>
        public static void ExtractBlockData(this IBlockObject obj, String code, out FileInfo block, out string blockName)
        {
            if (App.Riviera.Is3DEnabled)
            {
                block = obj.BlockFile3d;
                blockName = code + "3D";
            }
            else
            {
                block = obj.BlockFile2d;
                blockName = code + "2D";
            }
        }
        /// <summary>
        /// Creates the content of the block.
        /// </summary>
        /// <param name="obj">The Riviera object that has a Block Reference as content.</param>
        /// <param name="code">The Block code name.</param>
        /// <param name="tr">The active transaction.</param>
        /// <returns>The AutoCAD block</returns>
        public static AutoCADBlock CreateBlockContent(this IBlockObject obj, String code, Transaction tr)
        {
            AutoCADBlock block = null;
            if (obj.BlockContent(code, tr))
                block = new AutoCADBlock(obj.Spacename, tr);
            return block;
        }
        /// <summary>
        /// Gets the model space.
        /// </summary>
        /// <param name="tr">The active transaction.</param>
        /// <returns>The block table record for the model space</returns>
        public static BlockTableRecord GetModelSpace(this Transaction tr, OpenMode openMode = OpenMode.ForWrite)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            BlockTable blockTable =(BlockTable) doc.Database.BlockTableId.GetObject(OpenMode.ForRead);
            return (BlockTableRecord)blockTable[BlockTableRecord.ModelSpace].GetObject(openMode);
        }
        /// <summary>
        /// Draws the content of the block.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="code">The code.</param>
        /// <param name="tr">The tr.</param>
        /// <param name="space">The space.</param>
        /// <returns></returns>
        public static Boolean DrawBlockContent(this IBlockObject obj, String code, Transaction tr, out AutoCADBlock space)
        {
            FileInfo block;
            String blockName;
            space = obj.CreateBlockContent(code, tr);
            //1: Se realiza la selección del bloque segun el modo que se encuentre activo
            obj.ExtractBlockData(code, out block, out blockName);
            //2: Se realiza la carga de los bloques
            if (block != null && File.Exists(block.FullName) && space != null)
            {
                ObjectIdCollection coll = space.List(tr);
                ObjectId blockRefId;
                //Se borra el espacio antes de cambiar el contenido
                if (coll.Count > 0 && coll.Count != 1)
                    space.Clear(tr);

                if ((coll.Count == 0) || (coll.Count > 0 && coll.Count != 1))
                    blockRefId = obj.Block.CreateReference(new Point3d(), 0).Draw(obj.Block.Block, tr);
                else
                {
                    BlockReference blk = coll[0].Open<BlockReference>(tr);
                    if (blk.Name != blockName)
                    {
                        space.Clear(tr);
                        blockRefId = obj.Block.CreateReference(new Point3d(), 0).Draw(obj.Block.Block, tr);
                    }
                }
                return true;
            }
            else
                return false;
        }
    }
}
