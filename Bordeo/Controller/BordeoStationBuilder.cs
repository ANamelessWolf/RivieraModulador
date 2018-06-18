using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Nameless.Libraries.HoukagoTeaTime.Ritsu.Shapes2D;
using Nameless.Libraries.HoukagoTeaTime.Ritsu.Utils;
using Nameless.Libraries.HoukagoTeaTime.Tsumugi;
using Nameless.Libraries.HoukagoTeaTime.Yui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DaSoft.Riviera.Modulador.Core.Assets.CONST;
using static DaSoft.Riviera.Modulador.Core.Controller.AutoCADUtils;
using static DaSoft.Riviera.Modulador.Bordeo.Assets.Codes;
using DaSoft.Riviera.Modulador.Core.Model;
using DaSoft.Riviera.Modulador.Bordeo.Model.Enities;
using System.Windows.Media;
using DaSoft.Riviera.Modulador.Bordeo.Model;
using DaSoft.Riviera.Modulador.Core.Runtime;

namespace DaSoft.Riviera.Modulador.Bordeo.Controller
{
    public class BordeoStationBuilder
    {
        /// <summary>
        /// The bordeo entities
        /// </summary>
        public List<Entity> BordeoEntities;
        /// <summary>
        /// The bordeo ends
        /// </summary>
        public List<Entity> BordeoEnds;
        /// <summary>
        /// The active transaction
        /// </summary>
        Transaction ActiveTransaction;
        /// <summary>
        /// Initializes a new instance of the <see cref="BordeoStationBuilder"/> class.
        /// </summary>
        /// <param name="tr">The tr.</param>
        public BordeoStationBuilder(Transaction tr)
        {
            this.ActiveTransaction = tr;
            this.BordeoEntities = new List<Entity>();
            this.BordeoEnds = new List<Entity>();
        }
        /// <summary>
        /// Selects the entities.
        /// </summary>
        public void SelectEntities()
        {
            Database db = Application.DocumentManager.MdiActiveDocument.Database;
            BlockTable blkTab = (BlockTable)db.BlockTableId.GetObject(OpenMode.ForRead);
            BlockTableRecord modelSpace = (BlockTableRecord)blkTab[BlockTableRecord.ModelSpace].GetObject(OpenMode.ForRead);
            DBObject obj;
            SelectionFilterBuilder sFilter = new SelectionFilterBuilder(typeof(Line), typeof(Polyline));
            SelectionFilter filter = sFilter.Filter;
            String code;
            foreach (ObjectId entId in modelSpace)
            {
                obj = entId.GetObject(OpenMode.ForRead);
                code = this.GetCode(obj as Entity);
                if (obj is Entity && (obj is Line || obj is Polyline) && (code == CODE_PANEL_STACK || code == CODE_DPANEL_STACK))
                {
                    this.ZoomIn(obj as Entity);
                    if (this.SearchInStart(obj as Entity, filter).Count() == 0 || this.SearchInEnd(obj as Entity, filter).Count() == 0)
                        this.BordeoEnds.Add(obj as Entity);
                    else
                        this.BordeoEntities.Add(obj as Entity);
                }
            }
        }
        /// <summary>
        /// Loads the riviera object from an drew entity
        /// </summary>
        /// <param name="ent">The entity object where the data is loaded.</param>
        /// <returns>The loaded riviera object</returns>
        public RivieraObject LoadRivieraObject(Entity ent)
        {
            ExtensionDictionaryManager dMan;
            String code;
            Xrecord xRec;
            dMan = new ExtensionDictionaryManager(ent.Id, this.ActiveTransaction);
            code = dMan.TryGetXRecord(KEY_CODE, out xRec, this.ActiveTransaction) ? xRec.GetDataAsString(this.ActiveTransaction)[0] : String.Empty;
            Point3d start, end;
            ObjectIdCollection ids;
            RivieraObject loadObject = null;
            this.GetLocation(dMan, this.ActiveTransaction, out start, out end);
            if (code == CODE_DPANEL_STACK)
            {
                BordeoLPanel[] doublePanels = BordeoLoader.GetDoublePanels(dMan, start, end, this.ActiveTransaction);
                BordeoLPanelStack pdStack = new BordeoLPanelStack(start, end, doublePanels);
                doublePanels[0].PanelGeometry = ent as Polyline;
                ids = this.FindGeometry(ent, this.ActiveTransaction);
                pdStack.Ids = ids;
                doublePanels[0].Ids = ids;
                loadObject = pdStack;
            }
            else if (code == CODE_PANEL_STACK)
            {
                BordeoPanel[] panels = BordeoLoader.GetPanels(dMan, start, end, this.ActiveTransaction);
                BordeoPanelStack pStack = new BordeoPanelStack(start, end, panels);
                pStack.PanelGeometry = ent as Line;
                ids = this.FindGeometry(ent, this.ActiveTransaction);
                pStack.Ids = ids;
                panels[0].Ids = ids;
                loadObject = pStack;
            }
            return loadObject;
        }
        /// <summary>
        /// Finds the geometry of the RivieraObject
        /// </summary>
        /// <param name="start">The object start point.</param>
        /// <param name="end">The object end point.</param>
        /// <param name="activeTransaction">The active transaction.</param>
        /// <returns>The object geometry</returns>
        private ObjectIdCollection FindGeometry(Entity ent, Transaction activeTransaction)
        {
            SelectionFilterBuilder sFilter = new SelectionFilterBuilder(typeof(BlockReference));
            IEnumerable<ObjectId> geomIds = this.SearchInStart(ent, sFilter.Filter).Intersect(this.SearchInEnd(ent, sFilter.Filter));
            return new ObjectIdCollection(geomIds.Select(x => (BlockReference)x.GetObject(OpenMode.ForRead)).Where(x => x.Name.Contains("BR20")).Select(x => x.Id).ToArray());

        }
        /// <summary>
        /// Gets the location.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        public void GetLocation(ExtensionDictionaryManager dMan, Transaction tr, out Point3d start, out Point3d end)
        {
            String[] location = dMan.GetXRecord(KEY_LOCATION, tr).GetDataAsString(tr);
            var coords = location[0].Split(',');
            start = new Point3d(double.Parse(coords[0]), double.Parse(coords[1]), 0);
            coords = location[1].Split(',');
            end = new Point3d(double.Parse(coords[0]), double.Parse(coords[1]), 0);
        }
        /// <summary>
        /// Gets the code.
        /// </summary>
        /// <param name="ent">The entity to extract the code.</param>
        /// <returns>The entity code</returns>
        private string GetCode(Entity ent)
        {
            ExtensionDictionaryManager dMan;
            String code;
            Xrecord xRec;
            dMan = new ExtensionDictionaryManager(ent.Id, this.ActiveTransaction);
            code = dMan.TryGetXRecord(KEY_CODE, out xRec, this.ActiveTransaction) ? xRec.GetDataAsString(this.ActiveTransaction)[0] : String.Empty;
            return code;
        }

        /// <summary>
        /// Zooms in to the entity
        /// </summary>
        /// <param name="ent">The entity to zoom.</param>
        private void ZoomIn(Entity ent)
        {
            ZoomWindow zoom = new ZoomWindow((ent as Entity).GeometricExtents.MinPoint, (ent as Entity).GeometricExtents.MaxPoint);
            zoom.SetView(1.2);
        }

        /// <summary>
        /// Searches the in the start point of the entity. For nearby Riviera entities
        /// </summary>
        /// <param name="ent">The base entity.</param>
        /// <param name="filter">The selection filter.</param>
        /// <returns>The selected entities</returns>
        public IEnumerable<ObjectId> SearchInStart(Entity ent, SelectionFilter filter)
        {
            Point2d start = (ent is Line) ? (ent as Line).StartPoint.ToPoint2d() : (ent as Polyline).StartPoint.ToPoint2d();
            return this.Search(ent, start, filter);
        }
        /// <summary>
        /// Searches the in the end point of the entity. For nearby Riviera entities
        /// </summary>
        /// <param name="ent">The base entity.</param>
        /// <param name="filter">The selection filter.</param>
        /// <returns>The selected entities</returns>
        public IEnumerable<ObjectId> SearchInEnd(Entity ent, SelectionFilter filter)
        {
            Point2d end = (ent is Line) ? (ent as Line).EndPoint.ToPoint2d() : (ent as Polyline).EndPoint.ToPoint2d();
            return this.Search(ent, end, filter);
        }
        /// <summary>
        /// Searches the riviera objects near the entity
        /// </summary>
        /// <param name="ent">The base entity.</param>
        /// <param name="point">The search point.</param>
        /// <param name="filter">The selection filter.</param>
        /// <returns>The selected entities</returns>
        public IEnumerable<ObjectId> Search(Entity ent, Point2d point, SelectionFilter filter)
        {
            Point3dCollection pts = new Point3dCollection();
            for (int i = 0; i < 10; i++)
                pts.Add(point.ToPoint2dByPolar(0.005, Math.PI * 5d * i).ToPoint3d());
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            var result = ed.SelectCrossingPolygon(pts, filter).Value.GetObjectIds();
            return result.Where(x => x != ent.Id);
        }
        /// <summary>
        /// Builds the station.
        /// Load the station members
        /// </summary>
        /// <param name="db">The Riviera database.</param>
        /// <param name="ent">The entity.</param>
        /// <returns>The list of station members</returns>
        public List<BordeoStationVertex> BuildStation(RivieraDatabase db, Entity ent)
        {
            List<BordeoStationVertex> objs = new List<BordeoStationVertex>();
            IEnumerable<ObjectId> ids;
            SelectionFilterBuilder sFilter = new SelectionFilterBuilder(typeof(Line), typeof(Polyline));
            this.Add(ref objs, db, ent, this.SearchInEnd(ent, sFilter.Filter).Count() > 0);
            Boolean stopBuilding = false;
            do
            {
                if (objs.Last().IsAtStart.Value)
                    ids = this.SearchInEnd(ent, sFilter.Filter);
                else
                    ids = this.SearchInStart(ent, sFilter.Filter);

                ent = this.Pop(ids.FirstOrDefault(), ref stopBuilding);
                this.Add(ref objs, db, ent, this.SearchInEnd(ent, sFilter.Filter).Where(x => !objs.Select(y => y.RivObj.CADGeometry.Id).Contains(x)).Count() > 0);
            } while (!stopBuilding);
            return objs;
        }
        /// <summary>
        /// Adds the specified the riviera object asigned to the
        /// given entity.
        /// </summary>
        /// <param name="objs">The Riviera objects collection.</param>
        /// <param name="db">The Riviera database.</param>
        /// <param name="ent">The entity.</param>
        private void Add(ref List<BordeoStationVertex> objs, RivieraDatabase db, Entity ent, Nullable<Boolean> isFoundAtStart)
        {

            if (ent != null)
            {
                RivieraObject obj = db.Objects.FirstOrDefault(x => x.Handle.Value == ent.Handle.Value);
                if (obj == null)
                {
                    obj = this.LoadRivieraObject(ent);
                    ent.UpgradeOpen();
                    obj.Save(this.ActiveTransaction);
                }
                //Bordeo Vertex
                objs.Add(new BordeoStationVertex()
                {
                    Index = objs.Count,
                    IsAtStart = isFoundAtStart,
                    RivObj = obj
                });
            }
        }

        /// <summary>
        /// Pops the specified entity by and object identifier.
        /// </summary>
        /// <param name="objectId">The object identifier.</param>
        /// <param name="stopBuilding">if set to <c>true</c> [stop building].</param>
        /// <returns>The selected entity</returns>
        private Entity Pop(ObjectId objectId, ref Boolean stopBuilding)
        {
            Entity ent = this.BordeoEntities.FirstOrDefault(x => x.Id == objectId);
            if (ent == null)
            {
                ent = this.BordeoEnds.FirstOrDefault(x => x.Id == objectId);
                if (ent == null)
                    stopBuilding = true;
                else
                    this.BordeoEnds.Remove(ent);
            }
            else
                this.BordeoEntities.Remove(ent);
            stopBuilding = (this.BordeoEnds.Count() + this.BordeoEntities.Count()) == 0;
            return ent;
        }
    }
}
