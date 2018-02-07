using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using NamelessOld.Libraries.HoukagoTeaTime.Ritsu;
using NamelessOld.Libraries.HoukagoTeaTime.Ritsu.Shapes2D;
using System;

namespace NamelessOld.Libraries.HoukagoTeaTime.Yui
{
    public class FenceSelector : AreaSelector
    {
        /// <summary>
        /// Creates a selection fence geometry
        /// The options are defined on the class methods
        /// </summary>
        /// <param name="pts">The area vertices</param>
        public FenceSelector(Point3dCollection pts) :
            base(pts)
        {
        }
        /// <summary>
        /// Creates a selection area from a circular area
        /// The options are defined on the class methods
        /// </summary>
        /// <param name="center">The search area center</param>
        /// <param name="rad">The search area radius</param>
        public FenceSelector(Point2d center, Double rad) :
            this(new Polygon2D(18, center, rad))
        {
        }
        /// <summary>
        /// Creates a selection fence, defined by a line segement
        /// The options are defined on the class methods
        /// </summary>
        /// <param name="pts">The area vertices</param>
        public FenceSelector(Point3d start, Point3d end) :
            base(new Point3dCollection(new Point3d[] { start, end }))
        {
        }
        /// <summary>
        /// Creates a selection fence geometry
        /// The options are defined on the class methods
        /// </summary>
        /// <param name="geometry">The geometry that defines the area</param>
        public FenceSelector(Geometry2D geometry) :
            this(geometry.Vertices.ToPoint3d())
        {
        }
        /// <summary>
        /// Select a group of objectIds contained or crossed by a polygon.
        /// </summary>
        /// <param name="selType">The selection type window polygon or crossed polygon</param>
        /// <param name="filter">The selection filter.</param>
        /// <returns>True if the polygon selects more than one entity.</returns>
        public Boolean Search(SelectionFilter filter)
        {
            if (filter != null)
                this.Result = this.Ed.SelectFence(this.Vertices, filter);
            else
                this.Result = this.Ed.SelectFence(this.Vertices);
            return this.Status == PromptStatus.OK && this.Result.Value.Count > 0;
        }
    }
}
