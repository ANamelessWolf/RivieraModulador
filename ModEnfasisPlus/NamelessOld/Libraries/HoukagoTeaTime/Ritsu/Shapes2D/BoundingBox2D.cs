using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using NamelessOld.Libraries.HoukagoTeaTime.Mio;
using System;
using System.Collections.Generic;
using System.Linq;
namespace NamelessOld.Libraries.HoukagoTeaTime.Ritsu.Shapes2D
{
    public class BoundingBox2D : Geometry2D
    {
        /// <summary>
        /// The mid point of the bounding box
        /// </summary>
        public Point2d MidPoint { get { return Min.MiddlePointTo(Max); } }
        /// <summary>
        /// The Bounding box diagonal Vector
        /// </summary>
        public Vector2d Vector { get { return Min.GetVectorTo(Max); } }
        /// <summary>
        /// Gets the Area of the bounding box
        /// </summary>
        public override double Area
        {
            get
            {
                return this.Width * this.Length;
            }
        }
        /// <summary>
        /// Gets the Perimeter of the bounding box
        /// </summary>
        public override double Perimeter
        {
            get
            {
                return 2 * (this.Width + this.Length);
            }
        }
        /// <summary>
        /// Creates a new bounding box
        /// </summary>
        /// <param name="minPt">The minimum point of the bounding box</param>
        /// <param name="maxPt">The maximun point of the bounding box</param>
        public BoundingBox2D(Point2d minPt, Point2d maxPt)
            : base(BoundingBox2D.GetPoints(minPt, maxPt))
        {

        }
        /// <summary>
        /// Creates a new bounding box
        /// </summary>
        /// <param name="minPt">The entity used as base to create a bounding box</param>
        public BoundingBox2D(Entity ent)
            : base(BoundingBox2D.GetPoints(ent.GeometricExtents.MinPoint.ToPoint2d(), ent.GeometricExtents.MaxPoint.ToPoint2d()))
        {

        }

        /// <summary>
        /// Scales the bounding box by a given factor
        /// </summary>
        /// <param name="factor">The factor used to scale the bounding box.</param>
        public BoundingBox2D Scale(double factor)
        {
            double dX = this.MidPoint.X;
            double dY = this.MidPoint.Y;
            Point2d minPoint = new Point2d(((this.Min.X - dX) * factor) + dX, ((this.Min.Y - dY) * factor) + dY),
                    maxPoint = new Point2d(((this.Max.X - dX) * factor) + dX, ((this.Max.Y - dY) * factor) + dY);
            return new BoundingBox2D(minPoint, maxPoint);
        }
        /// <summary>
        /// Moves the bounding box by a vector
        /// </summary>
        /// <param name="vector">The vector used to the bounding box.</param>
        public BoundingBox2D Move(Vector2d vector)
        {
            Point2d minPoint = this.Min.TransformBy(Matrix2d.Displacement(vector)),
                    maxPoint = this.Max.TransformBy(Matrix2d.Displacement(vector));
            return new BoundingBox2D(minPoint, maxPoint);
        }
        /// <summary>
        /// Get the collection of bounding boxes
        /// </summary>
        /// <param name="ents">The collection of entities to extract it bounding boxes</param>
        /// <returns>The collection of bounding boxes</returns>
        public static IEnumerable<BoundingBox2D> ExtractBoxes(params Entity[] ents)
        {
            return ents.Select<Entity, BoundingBox2D>(x => x.CreateBoundingBox());
        }

        /// <summary>
        /// Get the points of the bounding box
        /// </summary>
        /// <param name="minPt">The minimum point of the bounding box</param>
        /// <param name="maxPt">The maximun point of the bounding box</param>
        /// <returns>The bounding box point collection</returns>
        public static Point2dCollection GetPoints(Point2d minPt, Point2d maxPt)
        {
            Point2dCollection pts;
            Point2d[] ptArr = new Point2d[] { minPt, maxPt };
            Double minX = ptArr.Select<Point2d, Double>(c => c.X).Min(),
                   maxX = ptArr.Select<Point2d, Double>(c => c.X).Max(),
                   minY = ptArr.Select<Point2d, Double>(c => c.Y).Min(),
                   maxY = ptArr.Select<Point2d, Double>(c => c.Y).Max();
            pts = new Point2dCollection(
                new Point2d[]
                {
                    new Point2d(minX,minY),
                    new Point2d(maxX,minY),
                    new Point2d(maxX,maxY),
                    new Point2d(minX,maxY),
                });
            return pts;
        }
    }
}
