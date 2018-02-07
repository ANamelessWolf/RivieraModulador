using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using NamelessOld.Libraries.HoukagoTeaTime.Runtime;
using NamelessOld.Libraries.Yggdrasil.Lilith;
using System;
using System.Linq;
using static NamelessOld.Libraries.HoukagoTeaTime.Assets.Errors;
namespace NamelessOld.Libraries.HoukagoTeaTime.Tsumugi
{
    public class XDataManager : NamelessObject
    {
        /// <summary>
        /// The id of the entity
        /// </summary>
        public new ObjectId Id;
        /// <summary>
        /// Creates a new XData manager from an entity
        /// </summary>
        /// <param name="ent">An entity of the drawing</param>
        public XDataManager(Entity ent)
        {
            this.Id = ent.Id;
        }
        /// <summary>
        /// Creates a new XData manager from an entity id
        /// </summary>
        /// <param name="entId">The entity id</param>
        public XDataManager(ObjectId entId)
        {
            this.Id = entId;
        }
        /// <summary>
        /// Set the xdata value form an array of typed values
        /// </summary>
        /// <param name="tr">Active transaction</param>
        /// <param name="appName">The application name</param>
        /// <param name="data">The typed value data</param>
        public void Set(Transaction tr, string appName, params TypedValue[] data)
        {
            if (this.Id.IsValid)
            {
                DBObject obj = this.Id.GetObject(OpenMode.ForWrite);
                if (obj is Entity)
                {
                    AddRegAppTableRecord(appName);
                    TypedValue[] newData = new TypedValue[data.Length + 1];
                    newData[0] = new TypedValue(1001, appName);
                    for (int i = 0; i < data.Length; i++)
                        newData[i + 1] = data[i];
                    ResultBuffer rB = new ResultBuffer(newData);
                    obj.XData = rB;
                    rB.Dispose();
                }
                else
                    throw new RomioException(NotAnEntity);
            }
            else
                throw new RomioException(BadId);
        }
        /// <summary>
        /// Set the xdata value form an array of typed values
        /// </summary>
        /// <param name="tr">Active transaction</param>
        /// <param name="appName">The application name</param>
        /// <param name="data">The string data</param>
        public void SetString(Transaction tr, string appName, params String[] data)
        {
            try
            {
                Set(tr, appName, data.Select<String, TypedValue>(x => new TypedValue((int)DxfCode.ExtendedDataAsciiString, x)).ToArray());
            }
            catch (Exception exc)
            {
                new RomioException(exc.Message);
            }
        }
        /// <summary>
        /// Get the xdata value form the entity
        /// </summary>
        /// <param name="tr">Active transaction</param>
        public TypedValue[] Get(Transaction tr)
        {
            TypedValue[] res = new TypedValue[0];
            if (this.Id.IsValid)
            {
                DBObject obj = this.Id.GetObject(OpenMode.ForRead);
                if (obj is Entity)
                {
                    ResultBuffer rb = obj.XData;
                    if (rb != null)
                        res = rb.AsArray();
                }
                else
                    throw new RomioException(NotAnEntity);
            }
            else
                throw new RomioException(BadId);
            return res;
        }
        /// <summary>
        /// Get the xdata value form the entity as string
        /// </summary>
        /// <param name="tr">Active transaction</param>
        public string[] GetAsString(Transaction tr)
        {
            try
            {
                return Get(tr).Select<TypedValue, String>(x => x.Value.ToString()).ToArray();
            }
            catch (System.Exception exc)
            {
                throw new RomioException(exc.Message);
            }
        }
        /// <summary>
        /// Register an application name to the AutoCAD 
        /// RegAppTable Record
        /// </summary>
        /// <param name="regAppName">The name of the application to be registered</param>
        void AddRegAppTableRecord(string regAppName)

        {
            FastTransactionWrapper ft = new FastTransactionWrapper(delegate (Document doc, Transaction tr)
            {
                Editor ed = doc.Editor;
                RegAppTable regAppTable = doc.Database.RegAppTableId.GetObject(OpenMode.ForRead) as RegAppTable;
                if (!regAppTable.Has(regAppName))
                {
                    regAppTable.UpgradeOpen();
                    RegAppTableRecord regAppRec = new RegAppTableRecord();
                    regAppRec.Name = regAppName;
                    regAppTable.Add(regAppRec);
                    tr.AddNewlyCreatedDBObject(regAppRec, true);
                }
            });
            ft.Run();
        }
    }
}
