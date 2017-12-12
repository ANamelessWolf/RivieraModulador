using DaSoft.Riviera.Modulador.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using static DaSoft.Riviera.Modulador.Bordeo.Controller.BordeoUtils;
using static DaSoft.Riviera.Modulador.Bordeo.Assets.Codes;
using static DaSoft.Riviera.Modulador.Core.Assets.CONST;
using Nameless.Libraries.HoukagoTeaTime.Ritsu.Utils;
using DaSoft.Riviera.Modulador.Bordeo.Controller;
using System.Collections;
using DaSoft.Riviera.Modulador.Core.Runtime;
using Nameless.Libraries.HoukagoTeaTime.Mio.Entities;
using DaSoft.Riviera.Modulador.Core.Controller;
using Nameless.Libraries.HoukagoTeaTime.Mio.Model;
using Nameless.Libraries.HoukagoTeaTime.Mio.Utils;

namespace DaSoft.Riviera.Modulador.Bordeo.Model.Enities
{
    public class BordeoPanelStack : RivieraObject, IEnumerable<BordeoPanel>
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
        /// The Riviera object start point
        /// </summary>
        public override Point2d Start => this.PanelGeometry.StartPoint.ToPoint2d();
        /// <summary>
        /// The Riviera object end point
        /// </summary>
        public override Point2d End => this.PanelGeometry.GetEndPoint(this.Size as PanelMeasure);
        /// <summary>
        /// Gets the riviera object available direction keys.
        /// </summary>
        /// <value>
        /// The dictionary keys.
        /// </value>
        public override string[] DirectionKeys => BordeoUtils.BordeoDirectionKeys();
        /// <summary>
        /// Gets the geometry that defines the riviera object data.
        /// </summary>
        /// <value>
        /// The geometry.
        /// </value>
        protected override Entity CADGeometry => this.PanelGeometry;
        /// <summary>
        /// Gets the measure of the first panel
        /// </summary>
        protected override RivieraMeasure Size => this.Panels.FirstOrDefault().PanelSize;
        /// <summary>
        /// Initializes a new instance of the <see cref="BordeoPanelStack"/> class.
        /// </summary>
        public BordeoPanelStack(Point3d start, Point3d end) :
            base(GetRivieraCode(CODE_PANEL_STACK))
        {
            this.Panels = new List<BordeoPanel>();
            this.PanelGeometry = new Line(start, end);
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
        public override void Draw(Transaction tr)
        {
            
            BordeoPanel firstPanel = this.Panels.FirstOrDefault();
            if (App.Riviera.Is3DEnabled)
            {
                foreach (var panel in this.Panels)
                    panel.Draw(tr);
            }
            else
            {
                //Se borran todos los paneles menos el primero
                foreach (var panel in this.Panels.Where(x => x.Elevation > 0))
                    panel.Erase(tr);
                firstPanel.Draw(tr);
            }
            if (this.Id.IsValid)
            {
                this.PanelGeometry.UpgradeOpen();
                this.PanelGeometry.EndPoint = this.End.ToPoint3d();
            }
            else
            {
                BlockTableRecord model = tr.GetModelSpace(OpenMode.ForWrite);
                this.PanelGeometry.EndPoint = this.End.ToPoint3d();
                this.PanelGeometry.Draw(model, tr);
            }
        }
        /// <summary>
        /// Devuelve un enumerador que procesa una iteración en la colección.
        /// </summary>
        /// <returns>
        /// Enumerador que se puede utilizar para recorrer en iteración la colección.
        /// </returns>
        public IEnumerator<BordeoPanel> GetEnumerator()
        {
            return this.Panels.GetEnumerator();
        }
        /// <summary>
        /// Devuelve un enumerador que recorre en iteración una colección.
        /// </summary>
        /// <returns>
        /// Objeto <see cref="T:System.Collections.IEnumerator" /> que puede usarse para recorrer en iteración la colección.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Panels.GetEnumerator();
        }
    }
}
