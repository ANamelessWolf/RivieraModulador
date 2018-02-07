using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using NamelessOld.Libraries.Yggdrasil.Lilith;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessOld.Libraries.HoukagoTeaTime.Ritsu.Shapes2D
{
    public abstract class Geometry2D : NamelessObject
    {
        /// <summary>
        /// The vertex collection of the Polygon.
        /// </summary>
        public Point2dCollection Vertices;
        /// <summary>
        /// Get the min point of the 2d point collection
        /// </summary>
        public Point2d Min { get { return Vertices.Sort(Model.PointCompareable.XY, SortOrder.Ascending)[0]; } }
        /// <summary>
        /// Get the max point of the 2d point collection
        /// </summary>
        public Point2d Max { get { return Vertices.Sort(Model.PointCompareable.XY, SortOrder.Descending)[0]; } }
        /// <summary>
        /// Get the geometry extends min point for the given geometry
        /// </summary>
        public Point2d MinExtends
        {
            get
            {
                Double ptX = this.Vertices.OfType<Point2d>().Select<Point2d, Double>(x => x.X).Min(),
                       ptY = this.Vertices.OfType<Point2d>().Select<Point2d, Double>(x => x.Y).Min();
                return new Point2d(ptX, ptY);
            }
        }
        /// <summary>
        /// Get the geometry extends max point for the given geometry
        /// </summary>
        public Point2d MaxExtends
        {
            get
            {
                Double ptX = this.Vertices.OfType<Point2d>().Select<Point2d, Double>(x => x.X).Max(),
                       ptY = this.Vertices.OfType<Point2d>().Select<Point2d, Double>(x => x.Y).Max();
                return new Point2d(ptX, ptY);
            }
        }
        /// <summary>
        /// The Length of the geometry, the length is always bigger or equal than the width.
        /// </summary>
        public virtual double Length
        {
            get
            {
                Double dy = Math.Abs(MinExtends.Y - MaxExtends.Y),
                       dx = Math.Abs(MinExtends.X - MaxExtends.X);
                if (dy >= dx)
                    return dy;
                else
                    return dx;
            }
        }
        /// <summary>
        /// The width of the geometry, the windth is always smaller or equal than the Length.
        /// </summary>
        public virtual double Width
        {
            get
            {
                Double dy = Math.Abs(MinExtends.Y - MaxExtends.Y),
                       dx = Math.Abs(MinExtends.X - MaxExtends.X);
                if (dy <= dx)
                    return dy;
                else
                    return dx;
            }
        }
        /// <summary>
        /// The perimeter of the geometry.
        /// </summary>
        public abstract double Perimeter { get; }
        /// <summary>
        /// The area of the geometry
        /// </summary>
        public virtual double Area { get { return 0; } }

        public void TransformBy(Matrix2d m)
        {
            Double[] coords;
            Double[] matrix = m.ToArray();
            for (int i = 0; i < this.Vertices.Count; i++)
            {
                coords = new Double[] { this.Vertices[i].X, this.Vertices[i].Y };
                coords[0] = matrix[0] * coords[0] + matrix[1] * coords[1] + matrix[2];
                coords[1] = matrix[2] * coords[0] + matrix[3] * coords[1] + matrix[4];
                this.Vertices[i] = new Point2d(coords[0], coords[1]);
            }
        }

        /// <summary>
        /// Creates a 2D Geometry
        /// </summary>
        /// <param name="vertexColl">The Vertex collection for the Geometry.</param>
        public Geometry2D(Point2dCollection vertexColl)
        {
            this.Vertices = vertexColl;
        }
        /// <summary>
        /// Creates a 2D Geometry
        /// </summary>
        /// <param name="pts">The geometry 2d point collection</param>
        public Geometry2D(params Point2d[] pts) :
            this(new Point2dCollection(pts))
        {

        }
        /// <summary>
        /// Update the collection of vertices
        /// </summary>
        /// <param name="pts">The new collection of vertices</param>
        public void RefreshVertices(Point2dCollection pts)
        {
            this.Vertices = pts;
        }
    }
}
