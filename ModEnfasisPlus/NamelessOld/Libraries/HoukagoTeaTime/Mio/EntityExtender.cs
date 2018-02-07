using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using NamelessOld.Libraries.HoukagoTeaTime.Assets;
using NamelessOld.Libraries.HoukagoTeaTime.Mio.Entities;
using NamelessOld.Libraries.HoukagoTeaTime.Mio.Model;
using NamelessOld.Libraries.HoukagoTeaTime.Ritsu;
using NamelessOld.Libraries.HoukagoTeaTime.Ritsu.Shapes2D;
using NamelessOld.Libraries.HoukagoTeaTime.Runtime;
using System;
using SysExc = System.Exception;

namespace NamelessOld.Libraries.HoukagoTeaTime.Mio
{
    public static class EntityExtender
    {
        /// <summary>
        /// Move a selected entity to an especific layer.
        /// The ObjectId must be an entity
        /// </summary>
        /// <param name="id">The id of the entity</param>
        /// <param name="layer">The name of the layer</param>
        public static void MoveToLayer(this ObjectId id, String layer)
        {
            try
            {
                VoidTransactionWrapper<Object> tr =
                    new VoidTransactionWrapper<Object>(MoveToLayerTransaction);
                tr.Run(id, layer);
            }
            catch (SysExc exc)
            {
                throw new RomioException(String.Format("{0}: {1}", String.Format(Errors.CanNotBeMovedToLayer, layer), exc.Message), exc);
            }
        }
        /// <summary>
        /// Move a selected entity to an especific layer.
        /// </summary>
        /// <param name="id">The id of the entity</param>
        /// <param name="layer">The name of the layer</param>
        public static void MoveToLayer(this Entity ent, String layer)
        {
            ent.Id.MoveToLayer(layer);
        }
        /// <summary>
        /// Move a selected entity to an especific layer.
        /// The ObjectId must be an entity
        /// </summary>
        /// <param name="id">The id of the entity</param>
        /// <param name="layer">The name of the layer</param>
        /// <param name="tr">The active transaction</param>
        public static void MoveToLayer(this ObjectId id, String layer, Transaction tr)
        {
            try
            {
                MoveToLayerTransaction(Application.DocumentManager.MdiActiveDocument, tr, id, layer);
            }
            catch (SysExc exc)
            {
                throw new RomioException(String.Format("{0}: {1}", String.Format(Errors.CanNotBeMovedToLayer, layer), exc.Message), exc);
            }
        }
        /// <summary>
        /// Move a selected entity to an especific layer.
        /// </summary>
        /// <param name="id">The id of the entity</param>
        /// <param name="layer">The name of the layer</param>
        /// <param name="tr">The active transaction</param>
        public static void MoveToLayer(this Entity ent, String layer, Transaction tr)
        {
            ent.Id.MoveToLayer(layer, tr);
        }

        /// <summary>
        /// Opens an object id into an entity type
        /// </summary>
        /// <param name="id">The object id used to open the entity</param>
        /// <returns>The entity of the object id</returns>
        public static Entity OpenEntity(this ObjectId id)
        {
            try
            {
                TransactionWrapper<ObjectId, Entity> tr =
                    new TransactionWrapper<ObjectId, Entity>(OpenEntityTransaction);
                return tr.Run(id);
            }
            catch (SysExc exc)
            {
                throw new RomioException(String.Format("{0}: {1}", String.Format(Errors.CanNotSummonAnEntity), exc.Message), exc);
            }
        }
        /// <summary>
        /// Opens an object id into an entity type
        /// </summary>
        /// <param name="id">The object id used to open the entity</param>
        /// <param name="tr">The active transaction</param>
        /// <returns>The entity of the object id</returns>
        public static Entity OpenEntity(this ObjectId id, Transaction tr)
        {
            try
            {
                return OpenEntityTransaction(Application.DocumentManager.MdiActiveDocument, tr, id);
            }
            catch (SysExc exc)
            {
                throw new RomioException(String.Format("{0}: {1}", String.Format(Errors.CanNotSummonAnEntity), exc.Message), exc);
            }
        }
        /// <summary>
        /// Opens an object id into an entity type
        /// </summary>
        /// <param name="id">The object id used to open the entity</param>
        /// <param name="tr">The active transaction</param>
        /// <returns>The entity of the object id</returns>
        public static T OpenObject<T>(this ObjectId id, Transaction tr) where T : DBObject
        {
            try
            {
                return (T)Open(Application.DocumentManager.MdiActiveDocument, tr, id);
            }
            catch (SysExc exc)
            {
                throw new RomioException(String.Format("{0}: {1}", String.Format(Errors.CanNotSummonAnObject), exc.Message), exc);
            }
        }
        /// <summary>
        /// Transforms an entity
        /// </summary>
        /// <param name="entId">The entity id</param>
        /// <param name="tr">The active transaction</param>
        public static void TransformBy(this ObjectId entId, Matrix3d matrix)
        {
            try
            {
                VoidTransactionWrapper<Object> trW =
                    new VoidTransactionWrapper<Object>(delegate (Document doc, Transaction tr, Object[] data)
                    {
                        Entity ent = OpenEntityTransaction(Application.DocumentManager.MdiActiveDocument, tr, entId);
                        ent.TransformBy(matrix);
                    });
                trW.Run(entId, matrix);
            }
            catch (SysExc exc)
            {
                throw new RomioException(String.Format("{0}: {1}", String.Format(Errors.FailedToTransform), exc.Message), exc);
            }
        }

        /// <summary>
        /// Transforms an entity
        /// </summary>
        /// <param name="entId">The entity id</param>
        /// <param name="tr">The active transaction</param>
        public static void TransformBy(this ObjectId entId, Transaction tr, Matrix3d matrix)
        {
            try
            {
                Entity ent = OpenEntityTransaction(Application.DocumentManager.MdiActiveDocument, tr, entId);
                ent.UpgradeOpen();
                ent.TransformBy(matrix);
            }
            catch (SysExc exc)
            {
                throw new RomioException(String.Format("{0}: {1}", String.Format(Errors.FailedToTransform), exc.Message), exc);
            }
        }


        /// <summary>
        /// Highlight an entity
        /// </summary>
        /// <param name="ent">The entity to highlight</param>
        /// <param name="col">The color for the highlight</param>
        /// <returns>The entity of the object id</returns>
        public static void Highlight(this Entity ent, Color col)
        {
            try
            {
                VoidTransactionWrapper<Object> tr =
                    new VoidTransactionWrapper<Object>(HighlightTransaction);
                tr.Run(ent.Id, col);
            }
            catch (SysExc exc)
            {
                throw new RomioException(String.Format("{0}: {1}", String.Format(Errors.CanNotSummonAnEntity), exc.Message), exc);
            }
        }
        /// <summary>
        /// Highlight an entity
        /// </summary>
        /// <param name="ent">The entity to highlight</param>
        /// <param name="col">The color for the highlight</param>
        /// <param name="tr">The active transaction</param>
        /// <returns>The entity of the object id</returns>
        public static void Highlight(this Entity ent, Color col, Transaction tr)
        {
            try
            {
                HighlightTransaction(Application.DocumentManager.MdiActiveDocument, tr, ent.Id, col);
            }
            catch (SysExc exc)
            {
                throw new RomioException(String.Format("{0}: {1}", String.Format(Errors.CanNotSummonAnEntity), exc.Message), exc);
            }
        }

        /// <summary>
        /// Search for the block table record id by its name.
        /// </summary>
        /// <param name="blockName">The name of the block table record</param>
        /// <returns>The object id of the existant block</returns>
        public static ObjectId FindBlockTableRecordId(this String blockName)
        {
            try
            {
                TransactionWrapper<String, ObjectId> tr =
                    new TransactionWrapper<String, ObjectId>(FindBlockTableRecordIdTransaction);
                return tr.Run(blockName);
            }
            catch (SysExc exc)
            {
                throw new RomioException(String.Format("{0}: {1}", String.Format(Errors.CanNotSummonAnEntity), exc.Message), exc);
            }
        }
        /// <summary>
        /// Search for the block table record id by its name.
        /// </summary>
        /// <param name="blockName">The name of the block table record</param>
        /// <returns>The object id of the existant block</returns>
        public static ObjectId FindBlockTableRecordId(this String blockName, Transaction tr)
        {
            try
            {
                return FindBlockTableRecordIdTransaction(Application.DocumentManager.MdiActiveDocument, tr, blockName);
            }
            catch (SysExc exc)
            {
                throw new RomioException(String.Format("{0}: {1}", String.Format(Errors.CanNotSummonAnEntity), exc.Message), exc);
            }
        }


        /// <summary>
        /// Create a bounding box from an entity
        /// </summary>
        /// <param name="ent">The entity to create the bounding box</param>
        /// <returns>The entity bounding box</returns>
        public static BoundingBox2D CreateBoundingBox(this Entity ent)
        {
            return new BoundingBox2D(ent);
        }


        /// <summary>
        /// Add a text tag to an entity by its Id
        /// </summary>
        /// <param name="id">The id of the entity</param>
        /// <param name="margin">The dbText margin</param>
        /// <param name="tagContent">The content of the tag</param>
        /// <param name="textHeight">The height of the text</param>
        /// <param name="rotation">Angle text rotation</param>
        public static void Tag(this ObjectId id, String tagContent, Double textHeight, Double rotation, Margin margin)
        {
            try
            {
                TransactionWrapper<Object, ObjectId> tr =
                             new TransactionWrapper<Object, ObjectId>(TagEntityTransaction);
                tr.Run(id, tagContent, textHeight, margin, rotation);
            }
            catch (SysExc exc)
            {
                throw new RomioException(String.Format("{0}: {1}", Errors.EntityCanNotBeTagged, exc.Message), exc);
            }
        }
        /// <summary>
        /// Add a text tag to an entity by its Id
        /// </summary>
        /// <param name="id">The id of the entity</param>
        /// <param name="margin">The dbText margin</param>
        /// <param name="tagContent">The content of the tag</param>
        /// <param name="textHeight">The height of the text</param>
        /// <param name="tr">The Active transaction</param>
        /// <returns>The ObjectId of the new tag</returns>
        public static ObjectId Tag(this ObjectId id, String tagContent, Double textHeight, Double rotation, Margin margin, Transaction tr)
        {
            try
            {
                return TagEntityTransaction(Application.DocumentManager.MdiActiveDocument, tr, id, tagContent, textHeight, margin, rotation);
            }
            catch (SysExc exc)
            {
                throw new RomioException(String.Format("{0}: {1}", Errors.EntityCanNotBeTagged, exc.Message), exc);
            }
        }
        /// <summary>
        /// Creates a DB Text on an entity
        /// </summary>
        /// <param name="doc">The AutoCAD Active Document</param>
        /// <param name="tr">The active transaction</param>
        /// <param name="data">The parameters that receive the transaction</param>
        /// <returns>The ObjectId for the entity</returns>
        private static ObjectId TagEntityTransaction(Document doc, Transaction tr, params object[] data)
        {
            Entity ent = ((ObjectId)data[0]).OpenEntity(tr);
            String txtString = data[1] as String;
            Double height = (Double)data[2];
            Margin margin = (Margin)data[3];
            Double rotation = (Double)data[4];
            Point3d pt = ent.GeometricExtentsCenter();
            DBText txt = Drawer.CreateDbText(txtString, pt, rotation, margin, height);
            txt.Layer = ent.Layer;
            return Drawer.Entity(txt, tr, ent.BlockName);
        }
        /// <summary>
        /// Finds a block table record Id by its name
        /// </summary>
        /// <param name="doc">The AutoCAD Active Document</param>
        /// <param name="tr">The active transaction</param>
        /// <param name="data">The parameters that receive the transaction</param>
        /// <returns>The ObjectId for the existant record</returns>
        private static ObjectId FindBlockTableRecordIdTransaction(Document doc, Transaction tr, params String[] data)
        {
            string blkname = data[0];
            ObjectId blkId;
            BlockTable blkTab = (BlockTable)doc.Database.BlockTableId.GetObject(OpenMode.ForRead);
            if (blkTab.Has(blkname))
                blkId = blkTab[blkname];
            else
                throw new RomioException(String.Format(Errors.BlockTableRecordMissing, blkname));
            return blkId;
        }
        /// <summary>
        /// Move an ObjectId to another layer
        /// </summary>
        /// <param name="tr">An active transaction</param>
        private static void MoveToLayerTransaction(Document doc, Transaction tr, params object[] data)
        {
            ObjectId id = (ObjectId)data[0];
            String layerName = (String)data[1];
            AutoCADLayer layer = new AutoCADLayer(layerName, tr);
            DBObject obj = id.GetObject(OpenMode.ForRead);
            if (obj is Entity)
            {
                Entity ent = obj as Entity;
                if (ent.Layer != layerName)
                {
                    ent.UpgradeOpen();
                    ent.Layer = layerName;
                }
            }
            else
                throw new RomioException(Errors.NotAnEntity);
        }
        /// <summary>
        /// Open an entity from and ObjectId
        /// </summary>
        /// <param name="doc">The AutoCAD Active Document</param>
        /// <param name="tr">The active transaction</param>
        /// <param name="data">The parameters that receive the transaction</param>
        /// <returns>The entity</returns>
        private static Entity OpenEntityTransaction(Document doc, Transaction tr, params ObjectId[] data)
        {
            DBObject obj = data[0].GetObject(OpenMode.ForRead);
            if (obj is Entity)
                return obj as Entity;
            else
                throw new RomioException(Errors.NotAnEntity);
        }
        /// <summary>
        /// Open an Object to an specific type
        /// </summary>
        /// <param name="doc">The AutoCAD Active Document</param>
        /// <param name="tr">The active transaction</param>
        /// <param name="data">The parameters that receive the transaction</param>
        /// <returns>The open object</returns>
        private static DBObject Open(Document doc, Transaction tr, params ObjectId[] data)
        {
            return data[0].GetObject(OpenMode.ForRead);
        }
        /// <summary>
        /// Highlight a selected entity
        /// </summary>
        /// <param name="doc">The AutoCAD Active Document</param>
        /// <param name="tr">The active transaction</param>
        /// <param name="data">The parameters that receive the transaction</param>
        private static void HighlightTransaction(Document doc, Transaction tr, params object[] data)
        {
            DBObject obj = ((ObjectId)data[0]).GetObject(OpenMode.ForWrite);
            if (obj is Entity)
                (obj as Entity).Color = (Color)data[1];
            else
                throw new RomioException(Errors.NotAnEntity);
        }

        /// <summary>
        /// Gets the typed value class dxfname from an entity type
        /// </summary>
        /// <param name="type">The entity type</typeparam>
        /// <returns>The Typed value</returns>
        public static TypedValue GetDxfName(this Type type)
        {
            return new TypedValue((int)DxfCode.Start, RXClass.GetClass(type).DxfName);
        }

        /// <summary>
        /// Obtiene un id desde un handle
        /// </summary>
        /// <param name="handleValue">El valor del handle</param>
        /// <returns>El id del handle seleccionado</returns>
        public static ObjectId GetId(this long handleValue)
        {
            Database db = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database;
            return db.GetObjectId(false, new Handle(handleValue), 0);
        }
    }
}
