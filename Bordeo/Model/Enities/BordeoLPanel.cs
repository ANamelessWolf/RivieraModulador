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
using System.Windows.Media;
using Autodesk.AutoCAD.ApplicationServices;
using static DaSoft.Riviera.Modulador.Core.Controller.AutoCADUtils;
using static DaSoft.Riviera.Modulador.Bordeo.Assets.Constants;
using Nameless.Libraries.HoukagoTeaTime.Tsumugi;

namespace DaSoft.Riviera.Modulador.Bordeo.Model.Enities
{
    /// <summary>
    /// Defines a bordeo "panel L recto"
    /// </summary>
    /// <seealso cref="DaSoft.Riviera.Modulador.Core.Model.RivieraObject" />
    public abstract class BordeoLPanel : RivieraObject, IBlockObject
    {
        /// <summary>
        /// Defines the starting Bordeo Panel elevation
        /// </summary>
        public Double Elevation;
        /// <summary>
        /// Gets or sets the block manager.
        /// </summary>
        /// <value>
        /// The block manager.
        /// </value>
        public RivieraBlock Block => new RivieraLBlock(this.BlockName, BlockDirectoryPath);
        /// <summary>
        /// Gets the name of the block.
        /// </summary>
        public string BlockName
        {
            get
            {
                double f1, f2;
                f1 = this.PanelSize.FrenteStart.Nominal;
                f2 = this.PanelSize.FrenteEnd.Nominal;
                return String.Format("{0}{1}{2}{3}T", this.Code.Block, f1, f2, this.PanelSize.Alto.Nominal);
            }
        }
        /// <summary>
        /// The panel sweep direction
        /// </summary>
        public SweepDirection Rotation;
        /// <summary>
        /// The panel rotation angle
        /// </summary>
        protected abstract Double RotationAngle { get; }
        /// <summary>
        /// The panel angle
        /// </summary>
        public abstract Double LAngle { get; }
        /// <summary>
        /// The panel union length
        /// </summary>
        protected abstract Double UnionLength { get; }
        /// <summary>
        /// Gets the panel code.
        /// </summary>
        /// <value>
        /// The panel code.
        /// </value>
        protected abstract String PanelCode { get; }
        /// <summary>
        /// The measure panel size
        /// </summary>
        public LPanelMeasure PanelSize => this.Size as LPanelMeasure;
        /// <summary>
        /// Gets the geometry that defines the riviera object data.
        /// </summary>
        /// <value>
        /// The geometry.
        /// </value>
        public override Entity CADGeometry => PanelGeometry;
        /// <summary>
        /// Gets the arrow rotation.
        /// </summary>
        /// <param name="isFrontDirection">if set to <c>true</c> [is front direction].</param>
        /// <param name="initialRotation">The initial rotation.</param>
        /// <returns>The arrow rotation</returns>
        public abstract Double GetArrowRotation(Boolean isFrontDirection, Double initialRotation);
        /// <summary>
        /// The panel geometry
        /// </summary>
        public Polyline PanelGeometry;
        /// <summary>
        /// Gets the riviera object available record keys.
        /// </summary>
        /// <value>
        /// The dictionary XRecord Keys.
        /// </value>
        public override string[] Keys => BordeoUtils.BordeoDirectionKeys();
        /// <summary>
        /// Initializes a new instance of the <see cref="BordeoLPanel"/> class.
        /// </summary>
        /// <param name="direction">The panel direction.</param>
        /// <param name="start">The panel start point.</param>
        /// <param name="end">The end point direction.</param>
        /// <param name="size">The panel size.</param>
        /// <param name="code">The panel code.</param>
        public BordeoLPanel(SweepDirection rotation, Point3d start, Point3d end, LPanelMeasure size, String code)
            : base(BordeoUtils.GetRivieraCode(code), size, start)
        {
            this.Rotation = rotation;
            this.Direction = start.ToPoint2d().GetVectorTo(end.ToPoint2d());
            this.Rotation = rotation;
            this.Regen();
        }
        public override void Save(Transaction tr)
        {
            base.Save(tr);
            var dMan = new ExtensionDictionaryManager(this.CADGeometry.Id, tr);
            dMan.Set(tr, KEY_LOCATION, this.Start.ToFormat(5, false), this.End.ToFormat(5, false));
            dMan.Set(tr, KEY_CODE, this.Code.Code);
        }
        /// <summary>
        /// Draws this instance
        /// </summary>
        /// <param name="tr">The active transaction.</param>
        protected override ObjectIdCollection DrawContent(Transaction tr)
        {
            try
            {
                ObjectIdCollection ids = new ObjectIdCollection();
                Boolean is2DBlock = !App.Riviera.Is3DEnabled;
                ObjectId first = this.Ids.OfType<ObjectId>().FirstOrDefault();
                RivieraLBlock block = this.Block as RivieraLBlock;
                var doc = Application.DocumentManager.MdiActiveDocument;
                BlockReference blkRef;
                LBlockType blockType = this.GetBlockType();
                //Si ya se dibujo, el elemento tiene un id válido, solo se debe actualizar
                //el contenido.
                if (first.IsValid)
                {
                    block.SetContent(blockType, is2DBlock, doc, tr);
                    blkRef = first.GetObject(OpenMode.ForWrite) as BlockReference;
                }
                else
                {
                    blkRef = block.Insert(doc, tr, blockType, this.Start.ToPoint3d(), this.Direction.Angle);
                    if (!is2DBlock)
                        UpdateBlockPosition(tr, blkRef);
                }
                ids.Add(blkRef.Id);
                return ids;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        /// <summary>
        /// Draws the geometry.
        /// </summary>
        /// <param name="tr">The transaction.</param>
        public ObjectId DrawGeometry(Transaction tr)
        {
            BlockTableRecord currentSpace = Application.DocumentManager.MdiActiveDocument.Database.CurrentSpaceId.GetObject(OpenMode.ForWrite) as BlockTableRecord;
            return this.CADGeometry.Draw(currentSpace, tr);
        }

        /// <summary>
        /// Gets the type of the block to insert.
        /// </summary>
        /// <returns>The block to insert</returns>
        private LBlockType GetBlockType()
        {
            LPanelMeasure size = this.PanelSize;
            LBlockType blockType = LBlockType.NONE;
            if (this.Rotation == SweepDirection.Clockwise)
            {
                if (size.FrenteStart == size.FrenteEnd)
                    blockType = LBlockType.RIGHT_SAME_SIZE;
                else if (size.FrenteStart > size.FrenteEnd)
                    blockType = LBlockType.RIGHT_START_MAX_SIZE;
                else
                    blockType = LBlockType.RIGHT_START_MIN_SIZE;
            }
            else
            {
                if (size.FrenteStart == size.FrenteEnd)
                    blockType = LBlockType.LEFT_SAME_SIZE;
                else if (size.FrenteStart > size.FrenteEnd)
                    blockType = LBlockType.LEFT_START_MAX_SIZE;
                else
                    blockType = LBlockType.LEFT_START_MIN_SIZE;
            }
            return blockType;
        }
        /// <summary>
        /// Regens this instance geometry <see cref="P:DaSoft.Riviera.Modulador.Core.Model.RivieraObject.CADGeometry" />.
        /// </summary>
        public override void Regen()
        {
            if (this.CADGeometry == null || this.PanelGeometry.Id.IsNull)
                this.PanelGeometry = new Polyline();

            //Limpiamos la polilínea para comenzar a dibujarla
            while (this.PanelGeometry.NumberOfVertices > 1)
                this.PanelGeometry.RemoveVertexAt(this.PanelGeometry.NumberOfVertices - 1);
            Double f1 = this.LAngle == Math.PI / 2 ? this.PanelSize.FrenteStart.Nominal.GetPanel90DrawingSize() : this.PanelSize.FrenteStart.Nominal.GetPanel135DrawingSize(),
                   f2 = this.LAngle == Math.PI / 2 ? this.PanelSize.FrenteEnd.Nominal.GetPanel90DrawingSize() : this.PanelSize.FrenteEnd.Nominal.GetPanel135DrawingSize(),
                   dirAng = this.Direction.Angle,
                   rotAng = this.Direction.Angle + (this.Rotation == SweepDirection.Clockwise ? -1 * this.RotationAngle : this.RotationAngle),
                   panAng = this.Direction.Angle + (this.Rotation == SweepDirection.Clockwise ? -1 * this.LAngle : this.LAngle);
            if (this is BordeoL135Panel)
                panAng = this.Rotation == SweepDirection.Clockwise ? rotAng - RotationAngle : rotAng + RotationAngle;
            //Polyline vertices
            List<Point2d> vertices = new List<Point2d>();
            vertices.Add(this.Start);
            vertices.Add(vertices[0].ToPoint2dByPolar(f1, dirAng));
            vertices.Add(vertices[1].ToPoint2dByPolar(this.UnionLength, rotAng));
            vertices.Add(vertices[2].ToPoint2dByPolar(f2, panAng));
            Double bulge = this.LAngle == Math.PI / 2 ? 0.4142 : 0.1989;
            if (this.Rotation == SweepDirection.Clockwise)
                bulge *= -1;
            vertices.ForEach(x => this.PanelGeometry.AddVertexAt(vertices.IndexOf(x), x, vertices.IndexOf(x) == 1 ? bulge : 0, 0, 0));
            if (this.PanelGeometry.NumberOfVertices == 5)
                this.PanelGeometry.RemoveVertexAt(this.PanelGeometry.NumberOfVertices - 1);
        }
        /// <summary>
        /// Updates the block position.
        /// </summary>
        /// <param name="tr">The tr.</param>
        /// <param name="blockRef">The block reference.</param>
        public void UpdateBlockPosition(Transaction tr, BlockReference blkRef)
        {
            blkRef.Position = new Point3d(blkRef.Position.X, blkRef.Position.Y, this.Elevation);
        }
        /// <summary>
        /// Gets the riviera object end point.
        /// </summary>
        /// <returns>
        /// The riviera end point
        /// </returns>
        protected override Point2d GetEndPoint()
        {
            Double f1 = this.LAngle == Math.PI / 2 ? this.PanelSize.FrenteStart.Nominal.GetPanel90DrawingSize() : this.PanelSize.FrenteStart.Nominal.GetPanel135DrawingSize(),
                   f2 = this.LAngle == Math.PI / 2 ? this.PanelSize.FrenteEnd.Nominal.GetPanel90DrawingSize() : this.PanelSize.FrenteEnd.Nominal.GetPanel135DrawingSize(),
                  dirAng = this.Direction.Angle,
                  rotAng = this.Direction.Angle + (this.Rotation == SweepDirection.Clockwise ? -1 * this.RotationAngle : this.RotationAngle),
                  panAng = this.Direction.Angle + (this.Rotation == SweepDirection.Clockwise ? -1 * this.LAngle : this.LAngle);
            if (this is BordeoL135Panel)
                panAng = this.Rotation == SweepDirection.Clockwise ? rotAng - RotationAngle : rotAng + RotationAngle;
            return this.Start.ToPoint2dByPolar(f1, dirAng).ToPoint2dByPolar(this.UnionLength, rotAng).ToPoint2dByPolar(f2, panAng);
        }

        /// <summary>
        /// Updates the size.
        /// </summary>
        /// <param name="sizes">The sizes.</param>
        /// <param name="direction">The direction.</param>
        internal void UpdateSize(IEnumerable<LPanelMeasure> sizes)
        {
            this.Size = sizes.FirstOrDefault(x => x.Alto.Nominal == this.PanelSize.Alto.Nominal);
        }
    }
}