using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using NamelessOld.Libraries.HoukagoTeaTime.Ritsu;
using NamelessOld.Libraries.HoukagoTeaTime.Ritsu.Shapes2D;
using System;
using System.Linq;

namespace NamelessOld.Libraries.HoukagoTeaTime.Yui
{
    public class WindowSelector : AreaSelector
    {
        /// <summary>
        /// The window min point
        /// </summary>
        public Point3d Min
        {
            get { return this.Vertices.OfType<Point3d>().FirstOrDefault(); }
            set
            {
                this.Vertices.RemoveAt(0);
                this.Vertices.Insert(0, value);
            }
        }
        /// <summary>
        /// The window max point
        /// </summary>
        public Point3d Max
        {
            get { return this.Vertices.OfType<Point3d>().FirstOrDefault(); }
            set
            {
                this.Vertices.RemoveAt(1);
                this.Vertices.Insert(1, value);
            }
        }
        /// <summary>
        /// Creates a new window selector
        /// </summary>
        /// <param name="max">The window max point</param>
        /// <param name="min">The window min point</param>
        public WindowSelector(Point3d min, Point3d max) :
            base(new Point3dCollection(new Point3d[] { min, max }))
        {
        }
        /// <summary>
        /// Creates a new window selector from a bounding box
        /// </summary>
        /// <param name="enableCrossing">True if the selection is made by crossing</param>
        /// <param name="box">The bounding box</param>
        public WindowSelector(BoundingBox2D box) :
            this(box.Min.ToPoint3d(), box.Max.ToPoint3d())
        {
        }
        /// <summary>
        /// Select a group of objectIds contained or crossed by a window.
        /// </summary>
        /// <param name="selType">The selection type window or Crossing Window</param>
        /// <param name="filter">The selection filter.</param>
        /// <returns>True if the window selects more than one entity.</returns>
        public Boolean Search(SelectType selType, SelectionFilter filter)
        {
            if (filter != null)
                this.Result = selType == SelectType.Crossing ? this.Ed.SelectCrossingWindow(this.Min, this.Max, filter) : this.Ed.SelectWindow(this.Min, this.Max, filter);
            else
                this.Result = selType == SelectType.Crossing ? this.Ed.SelectCrossingWindow(this.Min, this.Max) : this.Ed.SelectWindow(this.Min, this.Max);

            return this.Status == PromptStatus.OK && this.Result.Value.Count > 0;
        }
    }
}
