using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using NamelessOld.Libraries.HoukagoTeaTime.Assets;
using NamelessOld.Libraries.HoukagoTeaTime.Mio.Entities;
using NamelessOld.Libraries.HoukagoTeaTime.Mio.Model;
using NamelessOld.Libraries.HoukagoTeaTime.Ritsu;
using NamelessOld.Libraries.HoukagoTeaTime.Ritsu.Shapes2D;
using NamelessOld.Libraries.HoukagoTeaTime.Runtime;
using System;
using AcadExc = Autodesk.AutoCAD.Runtime.Exception;
namespace NamelessOld.Libraries.HoukagoTeaTime.Mio
{
    public class Drawer
    {
        /// <summary>
        /// Draws a 2D Geometry
        /// </summary>
        /// <param name="geometry">The geometry to draw.</param>
        /// <param name="blockname">The name of the block table record to draw the polyline, 
        /// if empty the polyline is draw on the current space</param>
        /// <param name="color">The color for the entity</param>
        /// <param name="closed">True if the geometry is closed</param>
        /// <returns>The ObjectId of the drew polygon.</returns>
        public static ObjectId Geometry2D(Geometry2D geometry, Color color, string blockname = "", bool closed = true)
        {
            Polyline pl = new Polyline();
            for (int vertexIndex = 0; vertexIndex < geometry.Vertices.Count; vertexIndex++)
                pl.AddVertexAt(vertexIndex, geometry.Vertices[vertexIndex], 0, 0, 0);
            pl.Color = color;
            pl.Closed = closed;
            return Drawer.Entity(pl, color, blockname);
        }
        /// <summary>
        /// Draws a 2D Geometry
        /// </summary>
        /// <param name="geometry">The geometry to draw.</param>
        /// <param name="blockname">The name of the block table record to draw the polyline, 
        /// if empty the polyline is draw on the current space</param>
        /// <param name="closed">True if the geometry is closed</param>
        /// <returns>The ObjectId of the drew polygon.</returns>
        public static ObjectId Geometry2D(Geometry2D geometry, string blockname = "", bool closed = true)
        {
            Polyline pl = CreatePolyline(geometry, closed);
            return Drawer.Entity(pl, blockname);
        }
        /// <summary>
        /// Creates an entity from a 2D Geometry
        /// </summary>
        /// <param name="geometry">The 2d geometry</param>
        /// <param name="closed">True if the geometry is closed</param>
        /// <returns>The entity as polyline</returns>
        public static Polyline CreatePolyline(Geometry2D geometry, bool closed = true)
        {
            Polyline pl = new Polyline();
            for (int vertexIndex = 0; vertexIndex < geometry.Vertices.Count; vertexIndex++)
                pl.AddVertexAt(vertexIndex, geometry.Vertices[vertexIndex], 0, 0, 0);
            pl.Closed = closed;
            return pl;
        }
        /// <summary>
        /// Draws a 2D Geometry
        /// </summary>
        /// <param name="geometry">The geometry to draw.</param>
        /// <param name="blockname">The name of the block table record to draw the polyline, 
        /// if empty the polyline is draw on the current space</param>
        /// <param name="closed">True if the polyline is closed</param>
        /// <param name="color">The geometry color</param>
        /// <param name="tr">The active transaction</param>
        /// <returns>The ObjectId of the drew polygon.</returns>
        public static ObjectId Geometry2D(Geometry2D geometry, Color color, Transaction tr, string blockname = "", bool closed = true)
        {
            Polyline pl = new Polyline();
            for (int vertexIndex = 0; vertexIndex < geometry.Vertices.Count; vertexIndex++)
                pl.AddVertexAt(vertexIndex, geometry.Vertices[vertexIndex], 0, 0, 0);
            pl.Color = color;
            pl.Closed = closed;
            return Drawer.Entity(pl, color, tr, blockname);
        }
        /// <summary>
        /// Draws a 2D Geometry
        /// </summary>
        /// <param name="geometry">The geometry to draw.</param>
        /// <param name="blockname">The name of the block table record to draw the polyline, 
        /// if empty the polyline is draw on the current space</param>
        /// <param name="closed">True if the polyline is closed</param>
        /// <param name="tr">The active transaction</param>
        /// <returns>The ObjectId of the drew polygon.</returns>
        public static ObjectId Geometry2D(Geometry2D geometry, Transaction tr, string blockname = "", bool closed = true)
        {
            Polyline pl = new Polyline();
            for (int vertexIndex = 0; vertexIndex < geometry.Vertices.Count; vertexIndex++)
                pl.AddVertexAt(vertexIndex, geometry.Vertices[vertexIndex], 0, 0, 0);
            pl.Closed = closed;
            return Drawer.Entity(pl, tr, blockname);
        }
        /// <summary>
        /// Draws an entity in the current drawing specifying the block table record name, if the name
        /// is empty the default block table record would be the current space.
        /// </summary>
        /// <param name="ent">The entity to draw</param>
        /// <param name="blockname">The name of the block table record to draw the polyline, 
        /// if empty the polyline is draw on the current space</param>
        /// <param name="color">The color for the entity</param>
        /// <returns>The ObjectId of the drew entity</returns>
        public static ObjectId Entity(Entity ent, Color color, string blockname = "")
        {
            try
            {
                ObjectId BlockTableRecordId = new ObjectId();
                if (blockname == "")
                    BlockTableRecordId = Application.DocumentManager.MdiActiveDocument.Database.CurrentSpaceId;
                else
                    BlockTableRecordId = blockname.FindBlockTableRecordId();
                TransactionWrapper<Object, ObjectIdCollection> trMom =
                    new TransactionWrapper<Object, ObjectIdCollection>(DrawEntityTransactionMoment);
                return trMom.Run(new Entity[] { ent }, color, BlockTableRecordId)[0];
            }
            catch (Exception exc)
            {
                throw new RomioException(String.Format("{0}: {1}", String.Format(Errors.CanNotDraw), exc.Message), exc);
            }
        }
        /// <summary>
        /// Draws a collection of entities in the current space
        /// </summary>
        /// <param name="ent">The collection of entities to draw</param>
        /// <returns>The ObjectIdCollection of the drew entities</returns>
        public static ObjectIdCollection Entity(Transaction tr, params Entity[] ent)
        {
            try
            {
                return DrawEntityTransactionMoment(Application.DocumentManager.MdiActiveDocument, tr,
                    ent, //Colección de entidades
                    Application.DocumentManager.MdiActiveDocument.Database.CurrentSpaceId); //Bloque actual;
            }
            catch (Exception exc)
            {
                throw new RomioException(String.Format("{0}: {1}", String.Format(Errors.CanNotDraw), exc.Message), exc);
            }
        }
        /// <summary>
        /// Draws an entity in the current drawing specifying the block table record name, if the name
        /// is empty the default block table record would be the current space.
        /// </summary>
        /// <param name="ent">The entity to draw</param>
        /// <param name="blockname">The name of the block table record to draw the polyline, 
        /// if empty the polyline is draw on the current space</param>
        /// <returns>The ObjectId of the drew entity</returns>
        public static ObjectId Entity(Entity ent, string blockname = "")
        {
            try
            {
                ObjectId BlockTableRecordId = new ObjectId();
                if (blockname == "")
                    BlockTableRecordId = Application.DocumentManager.MdiActiveDocument.Database.CurrentSpaceId;
                else
                    BlockTableRecordId = blockname.FindBlockTableRecordId();
                TransactionWrapper<Object, ObjectIdCollection> trMom =
                    new TransactionWrapper<Object, ObjectIdCollection>(DrawEntityTransactionMoment);
                return trMom.Run(new Entity[] { ent }, BlockTableRecordId)[0];
            }
            catch (Exception exc)
            {
                throw new RomioException(String.Format("{0}: {1}", String.Format(Errors.CanNotDraw), exc.Message), exc);
            }
        }

        /// <summary>
        /// Draws an entity in the current drawing specifying the block table record name, if the name
        /// is empty the default block table record would be the current space.
        /// </summary>
        /// <param name="ent">The entity to draw</param>
        /// <param name="blockname">The name of the block table record to draw the polyline, 
        /// if empty the polyline is draw on the current space</param>
        /// <param name="color">The color for the entity</param>
        /// <param name="tr">The Active transaction</param>
        /// <returns>The ObjectId of the drew entity</returns>
        public static ObjectId Entity(Entity ent, Color color, Transaction tr, string blockname = "")
        {
            try
            {
                ObjectId BlockTableRecordId = new ObjectId();
                if (blockname == "")
                    BlockTableRecordId = Application.DocumentManager.MdiActiveDocument.Database.CurrentSpaceId;
                else
                    BlockTableRecordId = blockname.FindBlockTableRecordId(tr);
                return DrawEntityTransactionMoment(Application.DocumentManager.MdiActiveDocument, tr, new Entity[] { ent }, color, BlockTableRecordId)[0];
            }
            catch (Exception exc)
            {
                throw new RomioException(String.Format("{0}: {1}", String.Format(Errors.CanNotDraw), exc.Message), exc);
            }
        }
        /// <summary>
        /// Draws an entity in the current drawing specifying the block table record name, if the name
        /// is empty the default block table record would be the current space.
        /// </summary>
        /// <param name="ent">The entity to draw</param>
        /// <param name="blockname">The name of the block table record to draw the polyline, 
        /// if empty the polyline is draw on the current space</param>
        /// <param name="tr">The Active transaction</param>
        /// <returns>The ObjectId of the drew entity</returns>
        public static ObjectId Entity(Entity ent, Transaction tr, string blockname = "")
        {
            try
            {
                ObjectId BlockTableRecordId = new ObjectId();
                if (blockname == "")
                    BlockTableRecordId = Application.DocumentManager.MdiActiveDocument.Database.CurrentSpaceId;
                else
                    BlockTableRecordId = blockname.FindBlockTableRecordId(tr);
                return DrawEntityTransactionMoment(Application.DocumentManager.MdiActiveDocument, tr, new Entity[] { ent }, BlockTableRecordId)[0];
            }
            catch (Exception exc)
            {
                throw new RomioException(String.Format("{0}: {1}", String.Format(Errors.CanNotDraw), exc.Message), exc);
            }
        }
        /// <summary>
        /// Draw a text
        /// </summary>
        /// <param name="textString">The text content</param>
        /// <param name="rotation">The text rotation angle</param>
        /// <param name="height">The text height</param>
        /// <param name="location">The text location</param>
        /// <param name="margin">The text margin</param>
        /// <param name="blockname">The name of the block table record to draw the polyline, 
        /// if empty the polyline is draw on the current space</param>
        /// <returns>The DbText objectId</returns>
        public static ObjectId Text(string textString, double rotation, Point3d location, double height, Margin margin, string blockname)
        {
            try
            {
                DBText txt = CreateDbText(textString, location, rotation, margin, height);
                return Drawer.Entity(txt, blockname);
            }
            catch (Exception exc)
            {
                throw new RomioException(String.Format("{0}: {1}", String.Format(Errors.CanNotDraw), exc.Message), exc);
            }
        }


        /// <summary>
        /// Draw a text
        /// </summary>
        /// <param name="textString">The text content</param>
        /// <param name="rotation">The text rotation angle</param>
        /// <param name="height">The text height</param>
        /// <param name="location">The text location</param>
        /// <param name="margin">The text margin</param>
        /// <param name="tr">The Active transaction</param>
        /// <param name="blockname">The name of the block table record to draw the polyline, 
        /// if empty the polyline is draw on the current space</param>
        /// <returns>The DbText objectId</returns>
        public static ObjectId Text(string textString, double rotation, Point3d location, double height, Margin margin, Transaction tr, string blockname)
        {
            try
            {
                DBText txt = CreateDbText(textString, location, rotation, margin, height);
                return Drawer.Entity(txt, tr, blockname);
            }
            catch (Exception exc)
            {
                throw new RomioException(String.Format("{0}: {1}", String.Format(Errors.CanNotDraw), exc.Message), exc);
            }
        }
        /// <summary>
        /// Create a text
        /// </summary>
        /// <param name="textString">The text content</param>
        /// <param name="rotation">The text rotation angle</param>
        /// <param name="height">The text height</param>
        /// <param name="location">The text location</param>
        /// <param name="margin">The text margin</param>
        /// if empty the polyline is draw on the current space</param>
        /// <returns>The DbText objectId</returns>
        public static DBText CreateText(string textString, double rotation, Point3d location, double height, Margin margin)
        {
            try
            {
                return CreateDbText(textString, location, rotation, margin, height);
            }
            catch (Exception exc)
            {
                throw new RomioException(String.Format("{0}: {1}", String.Format(Errors.CanNotDraw), exc.Message), exc);
            }
        }
        /// <summary>
        /// Delete every entity given the ObjectId
        /// </summary>
        /// <param name="objIds">The ObjectIds of the entities to be erased.</param>
        public static void Erase(ObjectIdCollection objIds)
        {
            VoidTransactionWrapper<ObjectIdCollection> trMom =
                 new VoidTransactionWrapper<ObjectIdCollection>(EraseTransactionMoment);
            trMom.Run(objIds);
        }
        /// <summary>
        /// Delete every entity given the ObjectId
        /// </summary>
        /// <param name="tr">The Active transaction</param>
        /// <param name="objIds">The ObjectIds of the entities to be erased.</param>
        public static void Erase(ObjectIdCollection objIds, Transaction tr)
        {
            EraseTransactionMoment(Application.DocumentManager.MdiActiveDocument, tr, objIds);
        }
        /// <summary>
        /// Delete every entity given the ObjectId
        /// </summary>
        /// <param name="tr">The Active transaction</param>
        /// <param name="objIds">The ObjectIds of the entities to be erased.</param>
        public static void Erase(Transaction tr, params ObjectId[] objIds)
        {
            if (objIds.Length > 0)
                Erase(new ObjectIdCollection(objIds), tr);
        }

        /// <summary>
        /// Creates a DBText Object
        /// </summary>
        /// <param name="txtString">The text string of the object</param>
        /// <param name="pt">The text insertion point</param>
        /// <param name="rotation">The text rotation</param>
        /// <param name="margin">The text margin</param>
        /// <param name="height">The text height</param>
        /// <returns>The DbText Object</returns>
        public static DBText CreateDbText(String txtString, Point3d pt, Double rotation, Margin margin, Double height)
        {
            pt = pt.ToPoint2d().SetMargin(rotation, margin).ToPoint3d();
            DBText txt = new DBText();
            txt.Height = height;
            txt.Position = pt;
            txt.Rotation = rotation;
            txt.TextString = txtString;
            return txt;
        }


        /// <summary>
        /// Erase a collection of entities
        /// </summary>
        /// <param name="doc">The AutoCAD Active Document</param>
        /// <param name="tr">The active transaction</param>
        /// <param name="data">The parameters that receive the transaction</param>
        private static void EraseTransactionMoment(Document doc, Transaction tr, params ObjectIdCollection[] data)
        {
            DBObject obj;
            foreach (ObjectId id in data[0])
            {
                try
                {
                    if (id.IsValid)
                    {
                        obj = (DBObject)id.GetObject(OpenMode.ForWrite);
                        if (!obj.IsErased)
                            obj.Erase();
                    }
                }
                catch (AcadExc) { continue; }
            }
        }
        /// <summary>
        /// Erase a collection of entities
        /// </summary>
        /// <param name="doc">The AutoCAD Active Document</param>
        /// <param name="tr">The active transaction</param>
        /// <param name="data">The parameters that receive the transaction</param>
        public static void EraseObject(Document doc, Transaction tr, ObjectId id)
        {
            DBObject obj;
            try
            {
                if (id.IsValid)
                {
                    obj = (DBObject)id.GetObject(OpenMode.ForWrite);
                    if (!obj.IsErased)
                        obj.Erase();
                }
            }
            catch (AcadExc) { }
        }

        /// <summary>
        /// Draw a collection of entities
        /// </summary>
        /// <param name="doc">The AutoCAD Active Document</param>
        /// <param name="tr">The active transaction</param>
        /// <param name="data">The parameters that receive the transaction</param>
        /// <returns>The ObjectId of the new drew entity</returns>
        private static ObjectIdCollection DrawEntityTransactionMoment(Document doc, Transaction tr, params object[] data)
        {
            Entity[] ents;
            Color color;
            ObjectId blockTableRecordId;
            if (data.Length == 3)
            {
                ents = data[0] as Entity[];
                color = (Autodesk.AutoCAD.Colors.Color)data[1];
                blockTableRecordId = (ObjectId)data[2];
            }
            else
            {
                ents = data[0] as Entity[];
                blockTableRecordId = (ObjectId)data[1];
                color = AutoCADLayer.GetActiveLayer(tr).GetColor();
            }
            BlockTableRecord myRecord = (BlockTableRecord)blockTableRecordId.GetObject(OpenMode.ForWrite);
            ObjectIdCollection drewEntities = new ObjectIdCollection();
            foreach (Entity ent in ents)
            {
                if (data.Length == 3)
                    ent.Color = color;
                myRecord.AppendEntity(ent);
                tr.AddNewlyCreatedDBObject(ent, true);
                drewEntities.Add(ent.ObjectId);
            }
            return drewEntities;
        }

    }

}
