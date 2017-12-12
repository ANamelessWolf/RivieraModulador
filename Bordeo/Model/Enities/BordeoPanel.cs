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
        /// El contenido del bloque a insertar
        /// </summary>
        public RivieraBlock Block { get { return new RivieraBlock(this.BlockName, BlockDirectoryPath); } }
        /// <summary>
        /// Gets the name of the block.
        /// </summary>
        public string BlockName => String.Format("{0}{1}{2}T", this.Code.Block, this.PanelSize.Frente.Nominal, this.PanelSize.Alto.Nominal);
        /// <summary>
        /// The Riviera object start point
        /// </summary>
        public override Point2d Start => this.PanelGeometry.StartPoint.ToPoint2d();
        /// <summary>
        /// The Riviera object end point
        /// </summary>
        public override Point2d End => this.PanelGeometry.GetEndPoint(this.PanelSize);
        /// <summary>
        /// Define la dirección del rectangulo.
        /// </summary>
        public override Vector2d Direction
        {
            get
            {
                double x = this.PanelGeometry.EndPoint.X - this.Start.X,
                       y = this.PanelGeometry.EndPoint.Y - this.Start.Y;
                return new Vector2d(x, y);
            }
        }
        /// <summary>
        /// Defines the starting Bordeo Panel elevation
        /// </summary>
        public Double Elevation;
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
        protected override Entity CADGeometry => PanelGeometry;
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
            Boolean is2DBlock = !App.Riviera.Is3DEnabled;
            ObjectId first = this.Ids.OfType<ObjectId>().FirstOrDefault();
            RivieraBlock block = this.Block;
            var doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            BlockReference blkRef;
            if (first.IsValid)
            {
                block.SetContent(is2DBlock, doc, tr);
                blkRef = first.GetObject(OpenMode.ForWrite) as BlockReference;
            }
            else
                blkRef = block.Insert(doc, tr, this.Start.ToPoint3d(), this.Direction.Angle);
            if (!is2DBlock)
                this.UpdateBlockPosition(tr, blkRef);
        }
        /// <summary>
        /// Updates the block position.
        /// </summary>
        /// <param name="tr">The active transaction.</param>
        /// <param name="blkRef">The block reference.</param>
        public void UpdateBlockPosition(Transaction tr, BlockReference blkRef)
        {
            blkRef.Position = new Point3d(blkRef.Position.X, blkRef.Position.Y, this.Elevation);
            //Se rota el bloque para la vista 3D
            Point3d insPoint = blkRef.Position;
            Vector3d v = insPoint.GetVectorTo(End.ToPoint3d(this.Elevation));
            blkRef.TransformBy(Matrix3d.Rotation(Math.PI / 2, v, blkRef.Position));
        }
    }
}
