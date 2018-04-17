﻿using DaSoft.Riviera.Modulador.Core.Controller;
using DaSoft.Riviera.Modulador.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using DaSoft.Riviera.Modulador.Bordeo.Controller;
using DaSoft.Riviera.Modulador.Core.Model.DB;
using Nameless.Libraries.HoukagoTeaTime.Ritsu.Utils;
using static DaSoft.Riviera.Modulador.Bordeo.Controller.BordeoUtils;
using DaSoft.Riviera.Modulador.Core.Runtime;
using Autodesk.AutoCAD.ApplicationServices;

namespace DaSoft.Riviera.Modulador.Bordeo.Model.Enities
{
    public class BordeoBridge : RivieraObject, IBlockObject
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
        public string BlockName => this.Code.Code;
        /// <summary>
        /// Defines the starting Bordeo Panel elevation
        /// </summary>
        public Double Elevation;
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
        /// Gets the riviera object available direction keys.
        /// </summary>
        /// <value>
        /// The dictionary keys.
        /// </value>
        public override string[] Keys => BordeoUtils.BordeoDirectionKeys();
        /// <summary>
        /// Initializes a new instance of the <see cref="BordeoBridge"/> class.
        /// </summary>
        /// <param name="groupCode">The group code.</param>
        /// <param name="acabadoPazo">The acabado pazo.</param>
        /// <param name="acabadoBridge">The acabado bridge.</param>
        /// <param name="size">The size.</param>
        /// <param name="start">The start point.</param>
        /// <param name="start">The end point.</param>
        public BordeoBridge(String groupCode, RivieraAcabado acabadoPazo, RivieraAcabado acabadoBridge, PanelMeasure size, Point3d start, Point3d end) :
            base(new BridgeGroupCode(groupCode, acabadoBridge, acabadoPazo), size, start)
        {
            this.Direction = start.ToPoint2d().GetVectorTo(end.ToPoint2d());
            this.Regen();
        }
        /// <summary>
        /// Regens this instance geometry <see cref="P:DaSoft.Riviera.Modulador.Core.Model.RivieraObject.CADGeometry" />.
        /// </summary>
        public override void Regen() => this.RegenAsLine(ref this.PanelGeometry);
        /// <summary>
        /// Draws this instance
        /// </summary>
        /// <param name="tr">The active transaction.</param>
        /// <returns></returns>
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
        /// Updates the block position.
        /// </summary>
        /// <param name="tr">The tr.</param>
        /// <param name="blockRef">The block reference.</param>
        public void UpdateBlockPosition(Transaction tr, BlockReference blkRef)
        {
            blkRef.Position = new Point3d(blkRef.Position.X, blkRef.Position.Y, this.Elevation);
        }
    }
}
