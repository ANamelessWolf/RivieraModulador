using DaSoft.Riviera.Modulador.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using DaSoft.Riviera.Modulador.Core.Runtime;
using static DaSoft.Riviera.Modulador.Bordeo.Controller.BordeoUtils;
using static DaSoft.Riviera.Modulador.Bordeo.Assets.Codes;
using static DaSoft.Riviera.Modulador.Core.Assets.CONST;
using DaSoft.Riviera.Modulador.Bordeo.Controller;
using Nameless.Libraries.HoukagoTeaTime.Ritsu.Utils;
using System.IO;
using Nameless.Libraries.HoukagoTeaTime.Mio.Entities;
using DaSoft.Riviera.Modulador.Core.Controller;
using Nameless.Libraries.HoukagoTeaTime.Mio.Model;
using Nameless.Libraries.HoukagoTeaTime.Mio.Utils;

namespace DaSoft.Riviera.Modulador.Bordeo.Model.Enities
{
    /// <summary>
    /// Defines a bordeo "panel recto"
    /// </summary>
    /// <seealso cref="DaSoft.Riviera.Modulador.Core.Model.RivieraObject" />
    public class BordeoPanel : RivieraObject, IBlockObject
    {
        /// <summary>
        /// The 2D Block file
        /// </summary>
        public FileInfo BlockFile2d => this.Code.GetBordeo2DBlock();
        /// <summary>
        /// The 3D Block file
        /// </summary>
        public FileInfo BlockFile3d => this.Code.GetBordeo3DBlock();
        /// <summary>
        /// Gets the spacename.
        /// </summary>
        /// <value>
        /// The spacename.
        /// </value>
        public String Spacename => String.Format(PREFIX_BLOCK, this.Code.Code);
        /// <summary>
        /// El contenido del bloque a insertar
        /// </summary>
        public AutoCADBlock Block { get; set; }
        /// <summary>
        /// The Riviera object start point
        /// </summary>
        public override Point2d Start => this.PanelGeometry.StartPoint.ToPoint2d();
        /// <summary>
        /// The Riviera object end point
        /// </summary>
        public override Point2d End => this.PanelGeometry.EndPoint.ToPoint2d();
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
        protected override Entity Geometry => PanelGeometry;
        /// <summary>
        /// The panel geometry
        /// </summary>
        public Line PanelGeometry;
        /// <summary>
        /// The measure size
        /// </summary>
        protected override RivieraMeasure Size => PanelSize;
        /// <summary>
        /// The measure panel size
        /// </summary>
        public PanelMeasure PanelSize;
        /// <summary>
        /// Initializes a new instance of the <see cref="BordeoPanel"/> class.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="measure">The measure.</param>
        public BordeoPanel(Point3d start, Point3d end, PanelMeasure measure) :
            base(GetRivieraCode(CODE_PANEL_RECTO))
        {
            this.PanelSize = measure;
            this.PanelGeometry = new Line(start, end);
        }
        /// <summary>
        /// Draws the specified Riviera object.
        /// The BordeoPanel is only visible when the mode is 3D
        /// </summary>
        /// <param name="tr">The Active transaction.</param>
        public override void Draw(Transaction tr)
        {
            AutoCADBlock space;
            if (App.Riviera.Is3DEnabled && this.DrawBlockContent(this.Code.Code, tr, out space))
            {
                AutoCADLayer lay = new AutoCADLayer(LAYER_RIVIERA_GEOMETRY, tr);
                lay.SetStatus(LayerStatus.EnableStatus, tr);
                BlockTableRecord model = tr.GetModelSpace();
                this.Ids.Add(space.CreateReference(this.Start.ToPoint3d(), this.Angle).Draw(model, tr));
                lay.AddToLayer(this.Ids, tr);
            }
            else
                this.Ids.Erase(tr);
        }
    }
}
