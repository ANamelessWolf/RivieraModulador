using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using NamelessOld.Libraries.HoukagoTeaTime.Runtime;
using System;
using System.Collections.Generic;

namespace NamelessOld.Libraries.HoukagoTeaTime.Mio.Styler
{
    public class LineStyler
    {
        /// <summary>
        /// The types of the lines saved on the drawing
        /// </summary>
        public Dictionary<String, ObjectId> LineTypes;
        /// <summary>
        /// Initializes a new instance of the <see cref="LineStyler"/> class.
        /// </summary>
        /// <param name="tr">The active transaction</param>
        public LineStyler(Transaction tr)
        {
            this.LineTypes = new Dictionary<string, ObjectId>();
            new FastTransactionWrapper(delegate (Document doc, Transaction trans)
            {
                Database db = doc.Database;
                LinetypeTable lineTpTab = (LinetypeTable)trans.GetObject(db.LinetypeTableId, OpenMode.ForRead);
                LinetypeTableRecord lineTpRec;
                foreach (var id in lineTpTab)
                {
                    lineTpRec = (LinetypeTableRecord)id.GetObject(OpenMode.ForRead);
                    if (!this.LineTypes.ContainsKey(lineTpRec.Name))
                        this.LineTypes.Add(lineTpRec.Name, id);
                }

            }).Run();
        }
        /// <summary>
        /// Loads the line style.
        /// </summary>
        /// <param name="tr">The active transaction</param>
        /// <param name="styleName">Name of the style.</param>
        public Boolean LoadLineStyle(Transaction tr, string styleName)
        {
            Boolean isLoaded = false;
            new FastTransactionWrapper(delegate (Document doc, Transaction trans)
            {
                Database db = doc.Database;
                HostApplicationServices.WorkingDatabase.LoadLineTypeFile(styleName, "acad.lin");
                LinetypeTable lineTpTab = (LinetypeTable)trans.GetObject(db.LinetypeTableId, OpenMode.ForRead);
                isLoaded = lineTpTab.Has(styleName);
                if (isLoaded)
                    this.LineTypes.Add(styleName, lineTpTab[styleName]);
            }).Run();
            return isLoaded;
        }
    }
}
