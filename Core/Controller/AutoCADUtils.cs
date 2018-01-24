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
        public const String KEY_PARENT = "Parent";
        public const String KEY_CONTENT = "Content";
        public const String KEY_LOCATION = "Location";
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
        /// Gets the model space.
        /// </summary>
        /// <param name="tr">The active transaction.</param>
        /// <returns>The block table record for the model space</returns>
        public static BlockTableRecord GetModelSpace(this Transaction tr, OpenMode openMode = OpenMode.ForWrite)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            BlockTable blockTable = (BlockTable)doc.Database.BlockTableId.GetObject(OpenMode.ForRead);
            return (BlockTableRecord)blockTable[BlockTableRecord.ModelSpace].GetObject(openMode);
        }

        /// <summary>
        /// Loads a block table record from an external database to the current database.
        /// If the blockname exists on the table record, the block table record id is taken from the curren database block table.
        /// </summary>
        /// <param name="blockname">The block table record name.</param>
        /// <param name="filePath">The dwg block file path.</param>
        /// <param name="tr">The active transaction.</param>
        /// <returns>The object id of the block table record</returns>
        public static ObjectId _LoadBlock(this String blockname, String filePath, Transaction tr)
        {
            ObjectId id = new ObjectId();
            Database dwg = Application.DocumentManager.MdiActiveDocument.Database;
            try
            {
                BlockTable blkTab = (BlockTable)dwg.BlockTableId.GetObject(OpenMode.ForRead);
                if (!blkTab.Has(blockname))
                {
                    //1: Se crea un registro de bloque
                    blkTab.UpgradeOpen();
                    BlockTableRecord newRecord = new BlockTableRecord();
                    newRecord.Name = blockname;
                    //2: Se agregá el registro a la tabla
                    blkTab.Add(newRecord);
                    tr.AddNewlyCreatedDBObject(newRecord, true);
                    //3: Se abre la base de datos externa
                    Database externalDB = new Database(false, true);
                    externalDB.ReadDwgFile(filePath, FileShare.Read, true, "");
                    //4: Con una subtransacción se clonán los elementos que esten contenidos en el espcio de modelo de la
                    //base de datos externa
                    ObjectIdCollection ids;
                    using (Transaction subTr = externalDB.TransactionManager.StartTransaction())
                    {
                        //4.1: Abrir el espacio de modelo de la base de datos externa
                        ObjectId modelId = SymbolUtilityServices.GetBlockModelSpaceId(externalDB);
                        BlockTableRecord model = subTr.GetObject(modelId, OpenMode.ForRead) as BlockTableRecord;
                        //4.2: Se extraen y clonan los elementos mediante la clase IdMapping
                        ids = new ObjectIdCollection(model.OfType<ObjectId>().ToArray());
                        //IEnumerable<DBObject> objs = ids.OfType<ObjectId>().Select(x => x.GetObject(OpenMode.ForRead));
                        int erased = ids.OfType<ObjectId>().Count(x => x.IsValid);
                        using (IdMapping iMap = new IdMapping())
                            dwg.WblockCloneObjects(ids, newRecord.Id, iMap, DuplicateRecordCloning.Replace, false);
                    }
                    id = newRecord.Id;
                }
                else
                    id = blkTab[blockname];
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return id;
        }
    }
}
