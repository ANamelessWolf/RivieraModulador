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
    /// <summary>
    /// Define el panel bordeo en L
    /// </summary>
    public class BordeoLPanelStack : RivieraObject, IEnumerable<BordeoLPanel>, ISowable, IBordeoPanelStyler
    {
        /// <summary>
        /// The collection of panels asociated to the stack
        /// </summary>
        public List<BordeoLPanel> Panels;
        /// <summary>
        /// The panel sweep direction
        /// </summary>
        public SweepDirection Rotation => this.Panels.FirstOrDefault().Rotation;
        /// <summary>
        /// The panel angle
        /// </summary>
        public readonly BordeoLPanelAngle PanelAngle;

        /// <summary>
        /// Gets the geometry that stores the riviera extended data.
        /// </summary>
        /// <value>
        /// The CAD geometry
        /// </value>
        public override Entity CADGeometry => this.Panels.FirstOrDefault().CADGeometry;
        /// <summary>
        /// Gets the riviera object available record keys.
        /// </summary>
        /// <value>
        /// The dictionary XRecord Keys.
        /// </value>
        public override string[] Keys => BordeoUtils.BordeoDirectionKeys();

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
        public BordeoLPanelStack(Point3d start, Point3d end, LPanelMeasure measure, SweepDirection rotation = SweepDirection.Clockwise, BordeoLPanelAngle angle = BordeoLPanelAngle.ANG_90) :
            base(BordeoUtils.GetRivieraCode(CODE_PANEL_STACK), measure, start)
        {
            this.Panels = new List<BordeoLPanel>();
            this.PanelAngle = angle;
            BordeoLPanel first;
            if (angle == BordeoLPanelAngle.ANG_90)
                first = new BordeoL90Panel(rotation, start, end, measure);
            else
                first = new BordeoL135Panel(rotation, start, end, measure);
            this.Panels.Add(first);
            this.Direction = this.Panels.FirstOrDefault().Direction;
            this.Regen();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="BordeoPanelStack"/> class.
        /// </summary>
        /// <param name="start">The start point.</param>
        /// <param name="end">The end point.</param>
        /// <param name="measure">The panel measure.</param>
        public BordeoLPanelStack(Point3d start, Point3d end, params BordeoLPanel[] panels) :
            base(BordeoUtils.GetRivieraCode(CODE_PANEL_STACK), panels[0].PanelSize, start)
        {
            this.Panels = new List<BordeoLPanel>();
            this.PanelAngle = panels[0].Code.Code == CODE_PANEL_90 ? BordeoLPanelAngle.ANG_90 : BordeoLPanelAngle.ANG_135;
            foreach (BordeoLPanel panel in this.Panels)
                this.Panels.Add(panel);
            this.Direction = this.Panels.FirstOrDefault().Direction;
            this.Regen();
        }
        /// <summary>
        /// Saves the riviera object.
        /// </summary>
        /// <param name="tr">The active transaction.</param>
        public override void Save(Transaction tr)
        {
            base.Save(tr);
            var dMan = new ExtensionDictionaryManager(this.CADGeometry.Id, tr);
            dMan.Set(tr, KEY_LOCATION, this.Start.ToFormat(5, false), (this.CADGeometry as Polyline).GetPoint2dAt(1).ToFormat(5, false));
            List<String> content = new List<string>();
            foreach (var panel in this)
                content.Add(String.Format("{0}@{1}@{2}@{3}@{4}@{5}@{6}"
                    , panel.Code.Code,
                    panel.PanelSize.FrenteStart.Nominal,
                    panel.PanelSize.FrenteEnd.Nominal,
                    panel.PanelSize.Alto.Nominal,
                    panel.Code.SelectedAcabadoIndex,
                    panel.Elevation,
                    (int)this.Rotation));
        }
        /// <summary>
        /// Updates the panel stack.
        /// </summary>
        /// <param name="newHeight">The new height.</param>
        /// <param name="acabadoLadoA">The acabado lado a.</param>
        /// <param name="acabadosLadoB">The acabados lado b.</param>
        public void UpdatePanelStack(BordeoPanelHeight newHeight, string[] acabadoLadoA, string[] acabadosLadoB)
        {
            //Se borran los tamaños actuales
            while (this.Panels.Count > 1)
                this.Panels.RemoveAt(this.Panels.Count - 1);
            //Se obtienen los tamaños actuales
            String codePanel = this.PanelAngle == BordeoLPanelAngle.ANG_135 ? CODE_PANEL_135 : CODE_PANEL_90;
            var sizes = BordeoUtils.GetDatabase().Sizes[codePanel].Sizes.Select(x => x as LPanelMeasure);
            RivieraSize p15 = sizes.FirstOrDefault(x => x.Alto.Nominal == 15d).Alto,
                        p27 = sizes.FirstOrDefault(x => x.Alto.Nominal == 27d).Alto;
            RivieraSize[] heights = newHeight.GetHeights(p27, p15);
            //Se agregan los tamaños superiores
            LPanelMeasure measure;
            var panelSize = this.FirstOrDefault().PanelSize;
            this.Panels.FirstOrDefault().SetAcabado(acabadoLadoA[0]);
            for (int i = 1; i < heights.Length; i++)
            {
                measure = sizes.FirstOrDefault(
                    x => x.FrenteStart.Nominal == panelSize.FrenteStart.Nominal &&
                    x.FrenteEnd.Nominal == panelSize.FrenteEnd.Nominal &&
                    x.Alto.Nominal == heights[i].Nominal);
                this.AddPanel(measure);
                this.Panels.LastOrDefault().SetAcabado(acabadoLadoA[i]);
            }
        }
        /// <summary>
        /// Adds the panel.
        /// </summary>
        /// <param name="measure">The panel measure.</param>
        public void AddPanel(LPanelMeasure measure)
        {
            Polyline pl = this.CADGeometry as Polyline;
            BordeoLPanel panel;
            if (this.PanelAngle == BordeoLPanelAngle.ANG_90)
                panel = new BordeoL90Panel(this.Rotation, pl.StartPoint, pl.EndPoint, measure);
            else
                panel = new BordeoL135Panel(this.Rotation, pl.StartPoint, pl.EndPoint, measure);
            Double elev = this.Panels.Sum(x => x.PanelSize.Alto.Real);
            panel.Elevation = elev;
            this.Panels.Add(panel);
        }
        /// <summary>
        /// Draws this instance
        /// </summary>
        /// <param name="tr">The active transaction.</param>
        protected override ObjectIdCollection DrawContent(Transaction tr)
        {
            BordeoLPanel firstPanel = this.Panels.FirstOrDefault();
            BlockTableRecord model = tr.GetModelSpace(OpenMode.ForWrite);
            ObjectIdCollection ids = new ObjectIdCollection();
            //Se dibujan todos los paneles en la vista 3D
            if (App.Riviera.Is3DEnabled)
            {
                foreach (var panel in this.Panels)
                {
                    panel.Direction = firstPanel.Direction;
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
            //Se dibuja o actualizá la polilínea
            if (this.Id.IsValid)
            {
                this.CADGeometry.Id.GetObject(OpenMode.ForWrite);
                this.Regen();
            }
            else
                this.CADGeometry.Draw(model, tr);
            return ids;
        }
        /// Devuelve un enumerador que recorre en iteración una colección.
        /// </summary>
        /// <returns>
        /// Objeto <see cref="T:System.Collections.IEnumerator" /> que puede usarse para recorrer en iteración la colección.
        /// </returns>
        public IEnumerator<BordeoLPanel> GetEnumerator()
        {
            return this.Panels.GetEnumerator();
        }
        /// <summary>
        /// Regens this instance geometry <see cref="P:DaSoft.Riviera.Modulador.Core.Model.RivieraObject.CADGeometry" />.
        /// </summary>
        public override void Regen() => this.Panels.FirstOrDefault().Regen();
        /// <summary>
        /// Gets the riviera object end point.
        /// </summary>
        /// <returns>
        /// The riviera end point
        /// </returns>
        protected override Point2d GetEndPoint() => this.Panels.FirstOrDefault().End;
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
        /// Gets the available directions.
        /// </summary>
        /// <returns>
        /// The arrow direction
        /// </returns>
        public IEnumerable<ArrowDirection> GetAvailableDirections()
        {
            ArrowDirection[] supportedDir = new ArrowDirection[] { ArrowDirection.FRONT, ArrowDirection.BACK };
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
        public ObjectId DrawArrow(ArrowDirection arrow, Point3d insertionPt, double rotation, Transaction tr)
        {
            var blockDirPath = Path.Combine(App.Riviera.AppDirectory.FullName, FOLDER_NAME_BLOCKS_BORDEO);
            BordeoLPanel first = this.Panels.FirstOrDefault();
            rotation = first.GetArrowRotation(arrow.IsFront(), rotation);
            return arrow.DrawArrow(insertionPt, rotation, blockDirPath, tr);
        }
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
        {
            ObjectIdCollection ids = new ObjectIdCollection();
            IEnumerable<ArrowDirection> arrows = this.GetAvailableDirections().Where(x => filter(x));
            arrows.ToList().ForEach(x => ids.Add(this.DrawArrow(x, insertionPt, rotation, tr)));
            return ids;
        }
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
            //Se bloquean las llaves que apunten al padre, solo
            //se permite una conexión.
            String key;
            //Solo conecta en dos orientaciones front y back
            if (direction.IsFront())
                direction = ArrowDirection.FRONT;
            else
                direction = ArrowDirection.BACK;
            key = ArrowDirection.BACK.GetArrowDirectionName();
            base.Connect(direction, newObject);
            var d = App.Riviera.Database.Objects.Select(x => x.Handle).ToArray();
            if (newObject.Children.ContainsKey(key))
                newObject.Children[key] = this.Handle.Value;
            else
                newObject.Children.Add(key, this.Handle.Value);
        }


    }
}
