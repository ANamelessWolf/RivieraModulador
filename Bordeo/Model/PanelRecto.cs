using Autodesk.AutoCAD.DatabaseServices;
using DaSoft.Riviera.Modulador.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.Geometry;
using Nameless.Libraries.HoukagoTeaTime.Ritsu.Utils;
using DaSoft.Riviera.Modulador.Bordeo.Controller;

namespace DaSoft.Riviera.Modulador.Bordeo.Model
{
    public class PanelRecto : RivieraObject
    {
        /// <summary>
        /// Gets the geometry that defines the riviera object data.
        /// </summary>
        /// <value>
        /// The geometry.
        /// </value>
        protected override Entity Geometry => LineGeometry;
        /// <summary>
        /// The line geometry data
        /// </summary>
        public Line LineGeometry;
        /// <summary>
        /// The measure size
        /// </summary>
        protected override RivieraMeasure Size => PanelSize;
        /// <summary>
        /// The panel size
        /// </summary>
        public PanelMeasure PanelSize;
        /// <summary>
        /// The Riviera object start point
        /// </summary>
        public override Point2d Start => this.LineGeometry.StartPoint.ToPoint2d();
        /// <summary>
        /// The Riviera object end point
        /// </summary>
        public override Point2d End => this.LineGeometry.EndPoint.ToPoint2d();
        /// <summary>
        /// Gets the riviera object directions supported keys.
        /// </summary>
        /// <value>
        /// The dictionary keys.
        /// </value>
        public override string[] DirectionKeys => BordeoUtils.BordeoDirectionKeys();

    }
}