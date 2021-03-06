﻿using DaSoft.Riviera.Modulador.Core.Model;
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
using Autodesk.AutoCAD.ApplicationServices;

namespace DaSoft.Riviera.Modulador.Bordeo.Model.Enities
{
    /// <summary>
    /// Defines a bordeo "panel recto"
    /// </summary>
    /// <seealso cref="DaSoft.Riviera.Modulador.Core.Model.RivieraObject" />
    public class BordeoPanel : RivieraObject, IBlockObject
    {
        /// <summary>
        /// Gets or sets the block manager.
        /// </summary>
        /// <value>
        /// The block manager.
        /// </value>
        public RivieraBlock Block => new RivieraLinearBlock(this.BlockName, BlockDirectoryPath);
        /// <summary>
        /// Gets the name of the block.
        /// </summary>
        public string BlockName => String.Format("{0}{1}{2}T", this.Code.Block, this.PanelSize.Frente.Nominal, this.PanelSize.Alto.Nominal);
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
        public override string[] Keys => BordeoUtils.BordeoDirectionKeys();
        /// <summary>
        /// Gets the geometry that defines the riviera object data.
        /// </summary>
        /// <value>
        /// The geometry.
        /// </value>
        public override Entity CADGeometry => PanelGeometry;
        /// <summary>
        /// The panel geometry
        /// </summary>
        public Line PanelGeometry;
        /// <summary>
        /// The measure panel size
        /// </summary>
        public PanelMeasure PanelSize => this.Size as PanelMeasure;
        /// <summary>
        /// Initializes a new instance of the <see cref="BordeoPanel"/> class.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="measure">The measure.</param>
        public BordeoPanel(Point3d start, Point3d end, PanelMeasure measure) :
            base(BordeoUtils.GetRivieraCode(CODE_PANEL_RECTO), measure, start)
        {
            this.Direction = start.ToPoint2d().GetVectorTo(end.ToPoint2d());
            this.Regen();
        }
        /// <summary>
        /// Draws the specified Riviera object.
        /// The BordeoPanel is only visible when the mode is 3D
        /// </summary>
        /// <param name="tr">The Active transaction.</param>
        protected override ObjectIdCollection DrawContent(Transaction tr)
        {
            Boolean is2DBlock = !App.Riviera.Is3DEnabled;
            ObjectId first = this.Ids.OfType<ObjectId>().FirstOrDefault();
            RivieraBlock block = this.Block;
            var doc = Application.DocumentManager.MdiActiveDocument;
            BlockReference blkRef, blockContent;
            ObjectIdCollection ids = new ObjectIdCollection();
            //Si ya se dibujo, el elemento tiene un id válido, solo se debe actualizar
            //el contenido.
            if (first.IsValid)
            {
                block.SetContent(is2DBlock, out blockContent, doc, tr);
                blkRef = first.GetObject(OpenMode.ForWrite) as BlockReference;
            }
            else
            {
                blkRef = block.Insert(doc, tr, this.Start.ToPoint3d(), this.Direction.Angle);
                if (!is2DBlock)
                    UpdateBlockPosition(tr, blkRef);
                ids.Add(blkRef.Id);
            }
            //Solo se actualizan los bloques insertados en la vista 3D

            return ids;
        }
        /// <summary>
        /// Updates the block position.
        /// </summary>
        /// <param name="tr">The active transaction.</param>
        /// <param name="blkRef">The block reference.</param>
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
            Double r = this.PanelSize.Frente.Real,
                   ang = this.Direction.Angle;
            return this.Start.ToPoint2dByPolar(r, ang);
        }
        /// <summary>
        /// Regens this instance geometry <see cref="P:DaSoft.Riviera.Modulador.Core.Model.RivieraObject.CADGeometry" />.
        /// </summary>
        public override void Regen() => this.RegenAsLine(ref this.PanelGeometry);
        /// <summary>
        /// Updates the size.
        /// </summary>
        /// <param name="size">The size.</param>
        internal void UpdateSize(IEnumerable<PanelMeasure> sizes)
        {
            this.Size = sizes.FirstOrDefault(x => x.Alto.Nominal == this.PanelSize.Alto.Nominal);

        }
    }
}