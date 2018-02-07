using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using NamelessOld.Libraries.HoukagoTeaTime.Ritsu;
using NamelessOld.Libraries.HoukagoTeaTime.Ritsu.Shapes2D;
using System;
namespace NamelessOld.Libraries.HoukagoTeaTime.Yui
{
    public class PolygonSelector : AreaSelector
    {
        /// <summary>
        /// Creates a selection area
        /// The options are defined on the class methods
        /// </summary>
        /// <param name="pts">The area vertices</param>
        public PolygonSelector(Point3dCollection pts) :
            base(pts)
        {
        }
        /// <summary>
        /// Creates a selection area from a circular area
        /// The options are defined on the class methods
        /// </summary>
        /// <param name="center">The search area center</param>
        /// <param name="rad">The search area radius</param>
        public PolygonSelector(Point2d center, Double rad) :
            this(new Polygon2D(18, center, rad))
        {
        }
        /// <summary>
        /// Creates a selection area
        /// The options are defined on the class methods
        /// </summary>
        /// <param name="geometry">The geometry that defines the area</param>
        public PolygonSelector(Geometry2D geometry) :
            this(geometry.Vertices.ToPoint3d())
        {
        }
        /// <summary>
        /// Select a group of objectIds contained or crossed by a polygon.
        /// </summary>
        /// <param name="selType">The selection type window polygon or crossed polygon</param>
        /// <param name="filter">The selection filter.</param>
        /// <returns>True if the polygon selects more than one entity.</returns>
        public Boolean Search(SelectType selType, SelectionFilter filter)
        {
            if (filter != null)
                this.Result = selType == SelectType.Crossing ? this.Ed.SelectCrossingPolygon(this.Vertices, filter) : this.Ed.SelectWindowPolygon(this.Vertices, filter);
            else
                this.Result = selType == SelectType.Crossing ? this.Ed.SelectCrossingPolygon(this.Vertices) : this.Ed.SelectWindowPolygon(this.Vertices);

            return this.Status == PromptStatus.OK && this.Result.Value.Count > 0;
        }
    }
}
