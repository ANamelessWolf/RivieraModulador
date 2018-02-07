using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using NamelessOld.Libraries.HoukagoTeaTime.Assets;
using NamelessOld.Libraries.HoukagoTeaTime.Mio.Entities;
using NamelessOld.Libraries.HoukagoTeaTime.Mio.Model;
using NamelessOld.Libraries.HoukagoTeaTime.Runtime;
using NamelessOld.Libraries.HoukagoTeaTime.Yui;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NamelessOld.Libraries.HoukagoTeaTime.Mio
{
    public class Drawing
    {
        /// <summary>
        /// The name of the layer
        /// </summary>
        public String Layername;
        /// <summary>
        /// The name of the AutoCAD block table record
        /// </summary>
        public String Blockname;
        /// <summary>
        /// Failed to draw entities
        /// </summary>
        public List<Entity> FailedDrewEntities;
        /// <summary>
        /// The collection of drew ids
        /// </summary>
        public ObjectIdCollection Ids;
        /// <summary>
        /// The first id
        /// </summary>
        public ObjectId FirstId
        {
            get { return this.Ids.OfType<ObjectId>().FirstOrDefault(); }
        }
        /// <summary>
        /// The list of the entities
        /// </summary>
        Entity[] Entities;
        /// <summary>
        /// Creates a new drawing
        /// </summary>
        /// <param name="ents">The collection of entities to be draw</param>
        public Drawing(params Entity[] ents)
        {
            this.Ids = new ObjectIdCollection();
            this.FailedDrewEntities = new List<Entity>();
            this.Entities = ents;
        }
        /// <summary>
        /// Draws the current entities on the especific layer,
        /// and the block table record
        /// </summary>
        /// <param name="tr">The active transaction</param>
        public void Draw(Transaction tr)
        {
            Database db = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database;
            BlockTableRecord drwRec;
            //Abrimos el bloque
            if (this.Blockname == null || this.Blockname == String.Empty)
                drwRec = db.CurrentSpaceId.GetObject(OpenMode.ForWrite) as BlockTableRecord;
            else
            {
                BlockTable blkTab = db.BlockTableId.GetObject(OpenMode.ForRead) as BlockTable;
                if (blkTab.Has(this.Blockname))
                    drwRec = blkTab[this.Blockname].GetObject(OpenMode.ForWrite) as BlockTableRecord;
                else
                {
                    blkTab.UpgradeOpen();
                    drwRec = new BlockTableRecord();
                    drwRec.Name = this.Blockname;
                    blkTab.Add(drwRec);
                    tr.AddNewlyCreatedDBObject(drwRec, true);
                }
            }
            //Validamos que exista la capa, en caso de que el usuario haya definido alguna
            if (this.Layername != null && this.Layername != String.Empty)
            {
                AutoCADLayer layer = new AutoCADLayer(this.Layername, tr);
                layer.SetStatus(LayerStatus.EnableStatus);
                foreach (Entity ent in this.Entities)
                    ent.Layer = layer.Layername;
            }
            //Realiza el dibujado de las entidades
            foreach (Entity ent in this.Entities)
            {
                try
                {
                    drwRec.AppendEntity(ent);
                    tr.AddNewlyCreatedDBObject(ent, true);
                    this.Ids.Add(ent.Id);
                }
                catch (Exception exc)
                {
                    Selector.Ed.WriteMessage("\n{0}", exc.Message);
                    this.FailedDrewEntities.Add(ent);
                }
            }
        }
        /// <summary>
        /// Draws the current entities on the especific layer,
        /// and the block table record
        /// </summary>
        public void Draw()
        {
            try
            {
                new FastTransactionWrapper(delegate (Document doc, Transaction tr)
                {
                    this.Draw(tr);
                }).Run();
            }
            catch (Exception exc)
            {
                throw new RomioException(String.Format("{0}: {1}", String.Format(Errors.CanNotDraw), exc.Message), exc);
            }
        }

    }
}
