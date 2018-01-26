using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using DaSoft.Riviera.Modulador.Bordeo.Controller;
using DaSoft.Riviera.Modulador.Core.Controller;
using DaSoft.Riviera.Modulador.Core.Model;
using DaSoft.Riviera.Modulador.Core.Model.DB;
using DaSoft.Riviera.Modulador.Core.Runtime;
using Nameless.Libraries.HoukagoTeaTime.Mio.Utils;
using Nameless.Libraries.HoukagoTeaTime.Ritsu.Utils;
using Nameless.Libraries.HoukagoTeaTime.Tsumugi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media;
using static DaSoft.Riviera.Modulador.Bordeo.Assets.Codes;
using static DaSoft.Riviera.Modulador.Bordeo.Assets.Constants;
using static DaSoft.Riviera.Modulador.Core.Controller.AutoCADUtils;
namespace DaSoft.Riviera.Modulador.Bordeo.Model.Enities
{
    public class BordeoPanelStack : RivieraObject, IEnumerable<BordeoPanel>, ISowable, IBordeoPanelStyler
    {
        /// <summary>
        /// The collection of panels asociated to the stack
        /// </summary>
        public List<BordeoPanel> Panels;
        /// <summary>
        /// The panel geometry
        /// </summary>
        public Line PanelGeometry;
        /// <summary>
        /// Gets the riviera object available direction keys.
        /// </summary>
        /// <value>
        /// The dictionary keys.
        /// </value>
        public override string[] Keys => BordeoUtils.BordeoDirectionKeys();
        /// <summary>
        /// Gets the geometry that defines the riviera object data.
        /// </summary>
        /// <value>
        /// The geometry.
        /// </value>
        public override Entity CADGeometry => this.PanelGeometry;
        /// <summary>
        /// Gets or sets the riviera bordeo code.
        /// </summary>
        /// <value>
        /// The riviera bordeo code.
        /// </value>
        public string RivieraBordeoCode { get => this.FirstOrDefault().Code.Code.Substring(0, 6); }
        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>
        /// The size.
        /// </value>
        public RivieraMeasure BordeoPanelSize { get => this.Size; }
        /// <summary>
        /// Gets or sets the heights.
        /// </summary>
        /// <value>
        /// The heights.
        /// </value>
        public BordeoPanelHeight Height { get => this.GetHeights(); }
        /// <summary>
        /// Gets or sets the acabados lado a.
        /// </summary>
        /// <value>
        /// The acabados lado a.
        /// </value>
        public IEnumerable<RivieraAcabado> AcabadosLadoA { get => this.Panels.Select(x => x.Code.SelectedAcabado); }
        /// <summary>
        /// Gets or sets the acabados lado b.
        /// </summary>
        /// <value>
        /// The acabados lado b.
        /// </value>
        public IEnumerable<RivieraAcabado> AcabadosLadoB { get => this.Panels.Select(x => x.Code.SelectedAcabado); }
        /// <summary>
        /// Initializes a new instance of the <see cref="BordeoPanelStack"/> class.
        /// </summary>
        /// <param name="start">The start point.</param>
        /// <param name="end">The end point.</param>
        /// <param name="measure">The panel measure.</param>
        public BordeoPanelStack(Point3d start, Point3d end, PanelMeasure measure) :
            base(BordeoUtils.GetRivieraCode(CODE_PANEL_STACK), measure, start)
        {
            this.Panels = new List<BordeoPanel>();
            var first = new BordeoPanel(start, end, measure);
            this.Panels.Add(first);
            this.Direction = this.Panels.FirstOrDefault().Direction;
            this.Regen();
            this.Description.ClassName = this.GetType().FullName;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="BordeoPanelStack"/> class.
        /// </summary>
        /// <param name="start">The start point.</param>
        /// <param name="end">The end point.</param>
        /// <param name="measure">The panel measure.</param>
        public BordeoPanelStack(Point3d start, Point3d end, BordeoPanel[] panels) :
            base(BordeoUtils.GetRivieraCode(CODE_PANEL_STACK), panels[0].PanelSize, start)
        {
            this.Panels = new List<BordeoPanel>();
            foreach (var panel in panels)
                this.Panels.Add(panel);
            this.Direction = this.Panels.FirstOrDefault().Direction;
            this.Regen();
            this.Description.ClassName = this.GetType().FullName;
        }
        /// <summary>
        /// Saves the riviera object.
        /// </summary>
        /// <param name="tr">The active transaction.</param>
        public override void Save(Transaction tr)
        {
            base.Save(tr);
            var dMan = new ExtensionDictionaryManager(this.CADGeometry.Id, tr);
            dMan.Set(tr, KEY_LOCATION, this.Start.ToFormat(5, false), this.End.ToFormat(5, false));
            List<String> content = new List<string>();
            foreach (var panel in this)
                content.Add(String.Format("{0}@{1}@{2}@{3}@{4}", panel.Code.Code, panel.PanelSize.Frente.Nominal, panel.PanelSize.Alto.Nominal, panel.Code.SelectedAcabadoIndex, panel.Elevation));
            dMan.Set(tr, KEY_CONTENT, content.ToArray());
        }

        /// <summary>
        /// Adds the panel.
        /// </summary>
        /// <param name="measure">The panel measure.</param>
        public void AddPanel(PanelMeasure measure)
        {
            BordeoPanel panel = new BordeoPanel(this.PanelGeometry.StartPoint, this.PanelGeometry.EndPoint, measure);
            Double elev = this.Panels.Sum(x => x.PanelSize.Alto.Real);
            panel.Elevation = elev;
            this.Panels.Add(panel);
        }
        /// <summary>
        /// Draws the specified transaction.
        /// </summary>
        /// <param name="tr">The Active transaction.</param>
        protected override ObjectIdCollection DrawContent(Transaction tr)
        {
            BordeoPanel firstPanel = this.Panels.FirstOrDefault();
            BlockTableRecord model = tr.GetModelSpace(OpenMode.ForWrite);
            ObjectIdCollection ids = new ObjectIdCollection();
            //Se dibujan todos los paneles en la vista 3D
            if (App.Riviera.Is3DEnabled)
            {
                foreach (var panel in this.Panels)
                {
                    panel.Draw(tr);
                    foreach (ObjectId id in panel.Ids)
                        ids.Add(id);
                }
            }
            else //Solo se dibuja el primer panel
            {
                //Si estan dibujados los otros paneles los borramos.
                foreach (var panel in this.Panels.Where(x => x.Elevation > 0))
                    panel.Erase(tr);
                //Se dibuja el primer panel
                firstPanel.Draw(tr);
                foreach (ObjectId id in firstPanel.Ids)
                    ids.Add(id);
            }
            //Se dibuja o actualizá la línea
            if (this.Id.IsValid)
            {
                this.PanelGeometry.Id.GetObject(OpenMode.ForWrite);
                this.Regen();
            }
            else
                this.PanelGeometry.Draw(model, tr);
            return ids;
        }

        /// <summary>
        /// Gets the enumerator that process the collection iteration
        /// </summary>
        /// <returns>
        /// The enum used to iterate the collection
        /// </returns>
        public IEnumerator<BordeoPanel> GetEnumerator()
        {
            return this.Panels.GetEnumerator();
        }
        /// <summary>
        /// Gets the enumerator that process the collection iteration
        /// </summary>
        /// <returns>
        /// The enum used to iterate the collection
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Panels.GetEnumerator();
        }
        /// <summary>
        /// Gets the riviera object end point.
        /// </summary>
        /// <returns>
        /// The riviera end point
        /// </returns>
        protected override Point2d GetEndPoint() => this.Panels.FirstOrDefault().End;
        /// <summary>
        /// Regens this instance geometry <see cref="P:DaSoft.Riviera.Modulador.Core.Model.RivieraObject.CADGeometry" />.
        /// </summary>
        public override void Regen() => this.RegenAsLine(ref this.PanelGeometry);

        /// <summary>
        /// Gets the available directions.
        /// </summary>
        /// <returns>
        /// The arrow direction
        /// </returns>
        public IEnumerable<ArrowDirection> GetAvailableDirections()
        {
            ArrowDirection[] supportedDir = new ArrowDirection[]
            {
                ArrowDirection.FRONT,
                ArrowDirection.FRONT_LEFT_135,
                ArrowDirection.FRONT_LEFT_90,
                ArrowDirection.FRONT_RIGHT_135,
                ArrowDirection.FRONT_RIGHT_90,
                ArrowDirection.BACK,
                ArrowDirection.BACK_LEFT_135,
                ArrowDirection.BACK_LEFT_90,
                ArrowDirection.BACK_RIGHT_135,
                ArrowDirection.BACK_RIGHT_90,
            };
            supportedDir.InitChildren(ref this.Children);
            return this.Children.Where(x => x.Value == 0 && supportedDir.Contains(x.Key.GetArrowDirection())).Select(x => x.Key.GetArrowDirection());
        }
        /// <summary>
        /// Draws the arrow on the given direction
        /// </summary>
        /// <param name="arrow">The arrow to be drawn.</param>
        /// <param name="insertionPt">The insertion point.</param>
        /// <param name="rotation">The arrow rotation rotation.</param>
        /// <param name="tr">The Active transaction.</param>
        /// <returns>
        /// The drew arrow object id
        /// </returns>
        public ObjectId DrawArrow(ArrowDirection arrow, Point3d insertionPt, double rotation, Transaction tr) =>
            arrow.DrawArrow(insertionPt, rotation, Path.Combine(App.Riviera.AppDirectory.FullName, FOLDER_NAME_BLOCKS_BORDEO), tr);
        /// <summary>
        /// Draws all available arrows
        /// </summary>
        /// <param name="insertionPt">The insertion point.</param>
        /// <param name="rotation">The arrow rotation rotation.</param>
        /// <param name="tr">The Active transaction.</param>
        /// <returns>
        /// The drew arrows object ids
        /// </returns>
        public ObjectIdCollection DrawArrows(Func<ArrowDirection, Boolean> filter, Point3d insertionPt, double rotation, Transaction tr)
            => this.DrawArrows(filter, insertionPt, rotation, Path.Combine(App.Riviera.AppDirectory.FullName, FOLDER_NAME_BLOCKS_BORDEO), tr);



        /// <summary>
        /// Picks an arrow direction.
        /// </summary>
        /// <returns>
        /// The arrow direction
        /// </returns>
        public ArrowDirection PickDirection(Transaction tr) => this.GetDirection(tr);
        /// <summary>
        /// Connects the specified object to this instance
        /// </summary>
        /// <param name="direction">The direction to connect this instance.</param>
        /// <param name="newObject">The new object to be added</param>
        public override void Connect(ArrowDirection direction, RivieraObject newObject)
        {
            //Solo conecta en dos orientaciones front y back
            if (direction.IsFront())
                direction = ArrowDirection.FRONT;
            else
                direction = ArrowDirection.BACK;
            base.Connect(direction, newObject);
            String key = ArrowDirection.BACK.GetArrowDirectionName();
            //Se bloquea el nodo en el que se realizo la conexión
            if (newObject.Children.ContainsKey(key))
                newObject.Children[key] = -1;
            else
                newObject.Children.Add(key, -1);
        }

        public void UpdatePanelStack(BordeoPanelHeight newHeight, string[] acabadoLadoA, string[] acabadosLadoB)
        {
            //Se borran los tamaños actuales
            while (this.Panels.Count > 1)
                this.Panels.RemoveAt(this.Panels.Count - 1);
            //Se obtienen los tamaños actuales
            var sizes = BordeoUtils.GetDatabase().Sizes[CODE_PANEL_RECTO].Sizes.Select(x => x as PanelMeasure);
            RivieraSize p15 = sizes.FirstOrDefault(x => x.Alto.Nominal == 15d).Alto,
                        p27 = sizes.FirstOrDefault(x => x.Alto.Nominal == 27d).Alto;
            RivieraSize[] heights = newHeight.GetHeights(p27, p15);
            //Se agregan los tamaños superiores
            PanelMeasure measure;
            var panelSize = this.FirstOrDefault().PanelSize;
            this.Panels.FirstOrDefault().SetAcabado(acabadoLadoA[0]);
            for (int i = 1; i < heights.Length; i++)
            {
                measure = sizes.FirstOrDefault(
                    x => x.Frente.Nominal == panelSize.Frente.Nominal &&
                    x.Alto.Nominal == heights[i].Nominal);
                this.AddPanel(measure);
                this.Panels.LastOrDefault().SetAcabado(acabadoLadoA[i]);
            }
        }
    }
}
