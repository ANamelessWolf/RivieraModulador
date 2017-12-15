using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Autodesk.AutoCAD.Geometry;
using static DaSoft.Riviera.Modulador.Bordeo.Assets.Codes;
namespace DaSoft.Riviera.Modulador.Bordeo.Model.Enities
{
    public class BordeoL90Panel : BordeoLPanel
    {
        /// <summary>
        /// Gets the panel code for a "panel en L a 90°"
        /// </summary>
        /// <value>
        /// The panel code.
        /// </value>
        protected override string PanelCode => CODE_PANEL_90;
        /// <summary>
        /// The panel rotation angle 45°
        /// </summary>
        protected override double RotationAngle => Math.PI/4;
        /// <summary>
        /// The panel angle 135°
        /// </summary>
        protected override double LAngle => Math.PI / 2;
        /// <summary>
        /// The panel union length
        /// </summary>
        protected override double UnionLength => 0.14170420d;
        /// <summary>
        /// Initializes a new instance of the <see cref="BordeoLPanel"/> class.
        /// </summary>
        /// <param name="direction">The panel direction.</param>
        /// <param name="start">The panel start point.</param>
        /// <param name="end">The end point direction.</param>
        /// <param name="size">The panel size.</param>
        public BordeoL90Panel(SweepDirection direction, Point3d start, Point3d end, LPanelMeasure size)
            : base(direction, start, end, size, CODE_PANEL_90)
        {
        }

    }
}
