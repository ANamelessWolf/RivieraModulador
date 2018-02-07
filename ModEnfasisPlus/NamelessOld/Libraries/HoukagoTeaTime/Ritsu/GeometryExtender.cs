using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using NamelessOld.Libraries.HoukagoTeaTime.Assets;
using NamelessOld.Libraries.HoukagoTeaTime.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessOld.Libraries.HoukagoTeaTime.Ritsu
{
    public static class GeometryExtender
    {
        /// <summary>
        /// Get the angle direction of a line
        /// </summary>
        /// <param name="line">The line direction</param>
        /// <returns>The angle direction in radians</returns>
        public static Double GetDirection(this Line line)
        {
            return new Vector2d(line.EndPoint.X - line.StartPoint.X, line.EndPoint.Y - line.StartPoint.Y).Angle;
        }
        /// <summary>
        /// Transform a collection of entities
        /// </summary>
        /// <param name="ids">The object id collection of the entities to transform</param>
        /// <param name="matrix">The matrix transform</param>
        public static void Transform(this ObjectIdCollection ids, Matrix3d matrix)
        {
            try
            {
                VoidTransactionWrapper<Object> tr =
                            new VoidTransactionWrapper<Object>(TransformEntitiesTransaction);
                tr.Run(ids, matrix);
            }
            catch (Exception exc)
            {
                throw new RomioException(String.Format("{0}: {1}", Errors.EntityCanNotBeTransform, exc.Message), exc);
            }
        }
        /// <summary>
        /// Transform a collection of entities
        /// </summary>
        /// <param name="ids">The object id collection of the entities to transform</param>
        /// <param name="tr">The active transaction</param>
        /// <param name="matrix">The matrix transform</param>
        public static void Transform(this ObjectIdCollection ids, Matrix3d matrix, Transaction tr)
        {
            try
            {
                TransformEntitiesTransaction(Application.DocumentManager.MdiActiveDocument, tr, ids, matrix);
            }
            catch (Exception exc)
            {
                throw new RomioException(String.Format("{0}: {1}", Errors.EntityCanNotBeTransform, exc.Message), exc);
            }
        }
        /// <summary>
        /// Transform a an entity
        /// </summary>
        /// <param name="ids">The id of the entity to move</param>
        /// <param name="matrix">The matrix transform</param>
        public static void Transform(this ObjectId id, Matrix3d matrix)
        {
            try
            {
                VoidTransactionWrapper<Object> tr =
                            new VoidTransactionWrapper<Object>(TransformEntitiesTransaction);
                tr.Run(id, matrix);
            }
            catch (Exception exc)
            {
                throw new RomioException(String.Format("{0}: {1}", Errors.EntityCanNotBeTransform, exc.Message), exc);
            }
        }
        /// <summary>
        /// Transform a an entity
        /// </summary>
        /// <param name="ids">The id of the entity to move</param>
        /// <param name="tr">The active transaction</param>
        /// <param name="matrix">The matrix transform</param>
        public static void Transform(this ObjectId id, Matrix3d matrix, Transaction tr)
        {
            try
            {
                TransformEntitiesTransaction(Application.DocumentManager.MdiActiveDocument, tr, id, matrix);
            }
            catch (Exception exc)
            {
                throw new RomioException(String.Format("{0}: {1}", Errors.EntityCanNotBeTransform, exc.Message), exc);
            }
        }
        /// <summary>
        /// Transform a collection of entities
        /// </summary>
        /// <param name="doc">The AutoCAD Active Document</param>
        /// <param name="tr">The active transaction</param>
        /// <param name="data">The parameters that receive the transaction</param>
        private static void TransformEntitiesTransaction(Document doc, Transaction tr, params Object[] data)
        {
            Matrix3d matrix = (Matrix3d)data[1];
            if (data[0] is ObjectIdCollection)
            {
                ObjectIdCollection objIds = data[0] as ObjectIdCollection;

                foreach (ObjectId id in objIds)
                {
                    DBObject obj = id.GetObject(OpenMode.ForRead);
                    if (obj is Entity)
                    {
                        obj.UpgradeOpen();
                        (obj as Entity).TransformBy(matrix);
                    }
                    else
                        throw new RomioException(Errors.NotAnEntity);
                }
            }
            else if (data[0] is ObjectId)
            {
                ObjectId entId = (ObjectId)data[0];
                DBObject obj = entId.GetObject(OpenMode.ForRead);
                if (obj is Entity)
                {
                    obj.UpgradeOpen();
                    (obj as Entity).TransformBy(matrix);
                }
                else
                    throw new RomioException(Errors.NotAnEntity);
            }
        }

    }
}
