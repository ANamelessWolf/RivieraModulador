using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;

namespace NamelessOld.Libraries.HoukagoTeaTime.Tsumugi
{
    public class GroupManager
    {
        /// <summary>
        /// The group id
        /// </summary>
        public ObjectId GroupId;
        /// <summary>
        /// La lista de los nombres
        /// </summary>
        /// <param name="tr">La transacción activa</param>
        /// <returns>La lista de nombres de grupos existentes</returns>
        public static List<String> GetGroupNames(Transaction tr)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            DBDictionary dic = (DBDictionary)doc.Database.GroupDictionaryId.GetObject(OpenMode.ForRead);
            List<String> items = new List<string>();
            foreach (var item in dic)
                items.Add(item.Key);
            return items;
        }

        /// <summary>
        /// Creates a new group if the group does not exists
        /// </summary>
        /// <param name="tr">The active transaction</param>
        /// <param name="grpName">The group name</param>
        public GroupManager(Transaction tr, String grpName)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            DBDictionary dic = (DBDictionary)doc.Database.GroupDictionaryId.GetObject(OpenMode.ForRead);
            if (!dic.Contains(grpName))
            {
                dic.UpgradeOpen();
                Group gp = new Group();
                dic.SetAt(grpName, gp);
                tr.AddNewlyCreatedDBObject(gp, true);
                this.GroupId = gp.Id;
            }
            else
                this.GroupId = dic.GetAt(grpName);
        }
        /// <summary>
        /// Appends an entity to the group with its object id
        /// </summary>
        /// <param name="tr">The active transaction</param>
        /// <param name="ids">The entities ids</param>
        public void AppendEntity(Transaction tr, ObjectIdCollection ids)
        {
            Group gp = GroupId.GetObject(OpenMode.ForWrite) as Group;
            foreach (ObjectId id in ids)
                gp.Append(id);
        }

    }
}
