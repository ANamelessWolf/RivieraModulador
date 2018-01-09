﻿using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using DaSoft.Riviera.Modulador.Bordeo.Controller;
using DaSoft.Riviera.Modulador.Core.Controller;
using DaSoft.Riviera.Modulador.Core.Model;
using DaSoft.Riviera.Modulador.Core.Runtime;
using Nameless.Libraries.HoukagoTeaTime.Mio.Utils;
using Nameless.Libraries.HoukagoTeaTime.Ritsu.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media;
using static DaSoft.Riviera.Modulador.Bordeo.Assets.Codes;
using static DaSoft.Riviera.Modulador.Bordeo.Assets.Constants;

namespace DaSoft.Riviera.Modulador.Bordeo.Model.Enities
{
    public class BordeoPanelStack : RivieraObject, IEnumerable<BordeoPanel>, ISowable
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
            BlockTableRecord model = tr.GetModelSpace(OpenMode.ForWrite);
            //Se dibujan todos los paneles en la vista 3D
            if (App.Riviera.Is3DEnabled)
            {
                foreach (var panel in this.Panels)
                    panel.Draw(tr);
            }
            else //Solo se dibuja el primer panel
            {
                //Si estan dibujados los otros paneles los borramos.
                foreach (var panel in this.Panels.Where(x => x.Elevation > 0))
                    panel.Erase(tr);
                //Se dibuja el primer panel
                firstPanel.Draw(tr);
            }
            //Se dibuja o actualizá la línea
            if (this.Id.IsValid)
            {
                this.PanelGeometry.UpgradeOpen();
                this.Regen();
            }
            else
                this.PanelGeometry.Draw(model, tr);
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
    }
}
