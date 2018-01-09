using DaSoft.Riviera.Modulador.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace DaSoft.Riviera.Modulador.Core.Controller
{
    /// <summary>
    /// Adds the functionality to pick up arrows
    /// </summary>
    public interface ISowable
    {
        /// <summary>
        /// Gets the available directions.
        /// </summary>
        /// <returns>The arrow direction</returns>
        IEnumerable<ArrowDirection> GetAvailableDirections();
        /// <summary>
        /// Draws the arrow on the given direction
        /// </summary>
        /// <param name="arrow">The arrow to be drawn.</param>
        /// <param name="tr">The Active transaction.</param>
        /// <param name="insertionPt">The insertion point.</param>
        /// <param name="rotation">The arrow rotation rotation.</param>
        /// <returns>The drew arrow object id</returns>
        ObjectId DrawArrow(ArrowDirection arrow, Point3d insertionPt, Double rotation, Transaction tr);
        /// <summary>
        /// Draws all available arrows
        /// </summary>
        /// <param name="tr">The Active transaction.</param>
        /// <param name="filter">The arrow selection filter</param>
        /// <param name="insertionPt">The insertion point.</param>
        /// <param name="rotation">The arrow rotation rotation.</param>
        /// <returns>The drew arrows object ids</returns>
        ObjectIdCollection DrawArrows(Func<ArrowDirection, Boolean> filter, Point3d insertionPt, Double rotation, Transaction tr);
        /// <summary>
        /// Picks an arrow direction.
        /// </summary>
        /// <returns>The arrow direction</returns>
        ArrowDirection PickDirection(Transaction tr);
    }
}
