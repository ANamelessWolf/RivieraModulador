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
{ }
    /// <summary>
    /// Defines a bordeo "panel L recto"
    /// </summary>
    /// <seealso cref="DaSoft.Riviera.Modulador.Core.Model.RivieraObject" />
    //public class BordeoLPanel : RivieraObject, IBlockObject
    //{
    //    /// <summary>
    //    /// Gets the name of the block.
    //    /// </summary>
    //    public string BlockName => String.Format("{0}{1}{2}{3}T", this.Code.Block, this.PanelLSize.FrenteStart.Nominal, this.PanelLSize.FrenteEnd.Nominal, this.PanelLSize.Alto.Nominal);
    //    /// <summary>
    //    /// Gets or sets the block manager.
    //    /// </summary>
    //    /// <value>
    //    /// The block manager.
    //    /// </value>
    //    public RivieraBlock Block => new RivieraBlock(this.BlockName, BlockDirectoryPath);
    //    /// <summary>
    //    /// Gets the geometry that defines the riviera object data.
    //    /// </summary>
    //    /// <value>
    //    /// The geometry.
    //    /// </value>
    //    protected override Entity CADGeometry => PanelGeometry;
    //    /// <summary>
    //    /// The panel geometry
    //    /// </summary>
    //    public Polyline PanelGeometry;
    //    /// <summary>
    //    /// The measure size
    //    /// </summary>
    //    protected override RivieraMeasure Size => PanelLSize;
    //    /// <summary>
    //    /// The measure panel size
    //    /// </summary>
    //    public LPanelMeasure PanelLSize;
    //    /// <summary>
    //    /// The Riviera object start point
    //    /// </summary>
    //    public override Point2d Start => this.PanelGeometry.StartPoint.ToPoint2d();
    //    /// <summary>
    //    /// The Riviera object end point
    //    /// </summary>
    //    public override Point2d End => this.PanelGeometry.EndPoint.ToPoint2d();
    //    /// <summary>
    //    /// Gets the riviera object available direction keys.
    //    /// </summary>
    //    /// <value>
    //    /// The dictionary keys.
    //    /// </value>
    //    public override string[] Keys => throw new NotImplementedException();
    //    /// <summary>
    //    /// Defines the direction
    //    /// </summary>
    //    public override Vector2d Direction
    //    {
    //        get
    //        {
    //            Point3d end = this.PanelGeometry.GetPoint3dAt(1);
    //            double x = end.X - this.PanelGeometry.StartPoint.X,
    //                   y = end.Y - this.PanelGeometry.StartPoint.Y;
    //            return new Vector2d(x, y);
    //        }
    //    }

    //    public RivieraMeasure PanelSize { get; internal set; }

    //    public BordeoLPanel(RivieraCode code, Point3d start, Point3d end) :
    //        base(GetRivieraCode(CODE_PANEL_RECTO))
    //    {
    //    }

    //    public void UpdateBlockPosition(Transaction tr, BlockReference blockRef)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override void Draw(Transaction tr)
    //    {
    //        throw new NotImplementedException();
    //    }
//    }
//}
