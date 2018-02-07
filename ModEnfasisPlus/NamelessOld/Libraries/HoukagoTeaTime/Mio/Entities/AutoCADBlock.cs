using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using NamelessOld.Libraries.HoukagoTeaTime.Assets;
using NamelessOld.Libraries.HoukagoTeaTime.Runtime;
using NamelessOld.Libraries.Yggdrasil.Exceptions;
using NamelessOld.Libraries.Yggdrasil.Lilith;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessOld.Libraries.HoukagoTeaTime.Mio.Entities
{
    public class AutoCADBlock : NamelessObject
    {
        /// <summary>
        /// The name of the Block
        /// </summary>
        public readonly String Blockname;
        /// <summary>
        /// The Block Table Record Object Id
        /// </summary>
        public readonly new ObjectId Id;

        #region Constructor
        /// <summary>
        /// Create a new autocad block table record
        /// The block is created but not inserted
        /// </summary>
        /// <param name="blockname">The name of the block</param>
        public AutoCADBlock(string blockname)
        {
            try
            {
                this.Blockname = blockname;
                TransactionWrapper<String, ObjectId> tr =
                    new TransactionWrapper<String, ObjectId>(CreateBlockTableRecord);
                this.Id = tr.Run(blockname);

            }
            catch (Exception exc)
            {
                throw new RomioException(String.Format("{0}: {1}", Errors.CanNotCreateBlock, exc.Message), exc);
            }
        }

        /// <summary>
        /// Create a new autocad block table record
        /// The block is created but not inserted
        /// </summary>
        /// <param name="blockname">The name of the block</param>
        /// <param name="file">The dwg block file</param>
        public AutoCADBlock(string blockname, FileInfo file)
        {
            try
            {
                if (File.Exists(file.FullName))
                {
                    this.Blockname = blockname;
                    TransactionWrapper<String, ObjectId> trW =
                        new TransactionWrapper<String, ObjectId>(delegate (Document doc, Transaction tr, string[] data)
                        {
                            return this.Load(data[0], data[1], tr);
                        });
                    this.Id = trW.Run(blockname, file.FullName);
                }
                else
                    throw BlackMateriaException.FileNotFound;
            }
            catch (Exception exc)
            {
                throw new RomioException(String.Format("{0}: {1}", Errors.CanNotCreateBlock, exc.Message), exc);
            }
        }
        /// <summary>
        /// Create a new autocad Block with an active transaction
        /// </summary>
        /// <param name="blockname">The name of the block</param>
        /// <param name="tr">An active transaction</param>
        public AutoCADBlock(string blockname, Transaction tr)
        {
            try
            {
                this.Blockname = blockname;
                this.Id = this.CreateBlockTableRecord(Application.DocumentManager.MdiActiveDocument, tr, blockname);
            }
            catch (Exception exc)
            {
                throw new RomioException(String.Format("{0}: {1}", Errors.CanNotCreateBlock, exc.Message), exc);
            }
        }
        /// <summary>
        /// Create a new autocad block table record
        /// The block is created but not inserted
        /// </summary>
        /// <param name="blockname">The name of the block</param>
        /// <param name="file">The dwg block file</param>
        /// <param name="tr">The active transaction</param>
        public AutoCADBlock(string blockname, FileInfo file, Transaction tr) : this(blockname, file)
        {
            try
            {
                if (File.Exists(file.FullName))
                    this.Id = this.Load(blockname, file.FullName, tr);
                else
                    throw BlackMateriaException.FileNotFound;
            }
            catch (Exception exc)
            {
                throw new RomioException(String.Format("{0}: {1}", Errors.CanNotCreateBlock, exc.Message), exc);
            }
        }
        #endregion
        /// <summary>
        /// Create a new autocad block table record
        /// The block is created but not inserted
        /// </summary>
        /// <param name="blockname">The name of the block</param>
        /// <param name="file">The dwg block file</param>
        public BlockReference CreateReference(Point3d insPt, Double angle, double scale = 1)
        {
            BlockReference blkRef = new BlockReference(insPt, this.Id);
            blkRef.Rotation = angle;
            blkRef.ScaleFactors = new Scale3d(scale);
            return blkRef;
        }



        /// <summary>
        /// List all the elements appended in the block table record
        /// </summary>
        /// <returns>Return the collection</returns>
        public ObjectIdCollection ListEntities()
        {
            ObjectIdCollection ids = new ObjectIdCollection();
            try
            {
                BlankTransactionWrapper<ObjectIdCollection> tr =
                        new BlankTransactionWrapper<ObjectIdCollection>(ExtractBlockTableRecordIds);
                ids = tr.Run();
            }
            catch (Exception) { }
            return ids;
        }
        /// <summary>
        /// Erase the block content
        /// </summary>
        /// <param name="tr">The active transaction</param>
        public void Clear(Transaction tr)
        {
            BlockTableRecord myBlock = this.Id.GetObject(OpenMode.ForRead) as BlockTableRecord;
            foreach (ObjectId id in myBlock)
                id.GetObject(OpenMode.ForWrite).Erase();
        }
        /// <summary>
        /// List all the elements appended in the block table record
        /// </summary>
        /// <returns>Return the collection</returns>
        public ObjectIdCollection ListEntities(Transaction tr)
        {
            ObjectIdCollection ids = new ObjectIdCollection();
            try
            {
                BlockTableRecord myBlock = this.Id.GetObject(OpenMode.ForRead) as BlockTableRecord;
                ids = new ObjectIdCollection(myBlock.OfType<ObjectId>().Where(x => x.GetObject(OpenMode.ForRead) is Entity).ToArray());
                return ids;
            }
            catch (Exception) { }
            return ids;
        }

        /// <summary>
        /// Creates a block table record or open an existant record by its name
        /// </summary>
        /// <param name="doc">The AutoCAD Active Document</param>
        /// <param name="tr">The active transaction</param>
        /// <param name="data">The parameters that receive the transaction</param>
        /// <returns>The ObjectId for the existant record</returns>
        private ObjectId CreateBlockTableRecord(Document doc, Transaction tr, params string[] data)
        {
            string blkname = data[0];
            ObjectId blkId;
            BlockTable blkTab = (BlockTable)doc.Database.BlockTableId.GetObject(OpenMode.ForRead);
            BlockTableRecord blkRec;
            if (blkTab.Has(blkname))
            {
                blkId = blkTab[blkname];
                blkRec = blkId.GetObject(OpenMode.ForRead) as BlockTableRecord;
            }
            else
            {
                blkTab.UpgradeOpen();
                blkRec = new BlockTableRecord();
                blkRec.Name = blkname;
                blkId = blkTab.Add(blkRec);
                tr.AddNewlyCreatedDBObject(blkRec, true);
            }
            return blkId;
        }
        /// <summary>
        /// Extract all the object ids of the saved entities
        /// </summary>
        /// <param name="doc">The AutoCAD Active Document</param>
        /// <param name="tr">The active transaction</param>
        /// <returns>The ObjectId collection of the existant entities</returns>
        private ObjectIdCollection ExtractBlockTableRecordIds(Document doc, Transaction tr)
        {
            ObjectIdCollection ids = new ObjectIdCollection();
            BlockTableRecord myBlock = this.Id.GetObject(OpenMode.ForRead) as BlockTableRecord;
            ids = new ObjectIdCollection(myBlock.OfType<ObjectId>().Where(x => x.GetObject(OpenMode.ForRead) is Entity).ToArray());
            return ids;
        }
        /// <summary>
        /// Esta función se encarga de cargar el bloque
        /// </summary>
        private ObjectId Load(String blockname, String filePath, Transaction tr)
        {
            ObjectId id = new ObjectId();
            Database dwg = Application.DocumentManager.MdiActiveDocument.Database;
            BlockTable blkTab = (BlockTable)dwg.BlockTableId.GetObject(OpenMode.ForRead);
            if (!blkTab.Has(blockname))
            {
                //Se inserta un registro de bloque vacio
                blkTab.UpgradeOpen();
                BlockTableRecord newRecord = new BlockTableRecord();
                newRecord.Name = blockname;
                blkTab.Add(newRecord);
                tr.AddNewlyCreatedDBObject(newRecord, true);
                //Abrir el archivo dwg como base de datos
                Database externalDB = new Database();
                externalDB.ReadDwgFile(filePath, FileShare.Read, true, null);
                ObjectIdCollection ids;
                //Con una subtransacción se clonaran los elementos del espacio del modelo
                //de la bd externa
                using (Transaction subTr = externalDB.TransactionManager.StartTransaction())
                {
                    //Se accede al espacio de modelo de la  base de datos externa
                    ObjectId modelId = SymbolUtilityServices.GetBlockModelSpaceId(externalDB);
                    BlockTableRecord model = subTr.GetObject(modelId, OpenMode.ForRead)
                        as BlockTableRecord;
                    //Se extraen los elementos y se clonan con la función
                    //IdMapping
                    ids = new ObjectIdCollection(model.OfType<ObjectId>().ToArray());
                    using (IdMapping iMap = new IdMapping())
                        dwg.WblockCloneObjects(ids, newRecord.Id, iMap, DuplicateRecordCloning.Replace, false);
                }
                id = newRecord.Id;
            }
            else
                id = blkTab[blockname];
            return id;
        }

    }
}
