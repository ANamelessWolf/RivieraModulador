using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using NamelessOld.Libraries.HoukagoTeaTime.Mio.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessOld.Libraries.HoukagoTeaTime.Ritsu
{
    public static class PointExtender
    {
        /// <summary>
        /// Converts a 2D point to a 3D Point, setting the z part by default to zero
        /// </summary>
        /// <param name="pt">The point to be converted</param>
        /// <param name="defaultZ">The default z value</param>
        /// <returns>The point 3D</returns>
        public static Point3d ToPoint3d(this Point2d pt, double defaultZ = 0)
        {
            return new Point3d(pt.X, pt.Y, defaultZ);
        }
        /// <summary>
        /// Converts a 2D point dollection to a 3D Point collection, setting the z part by default to zero
        /// </summary>
        /// <param name="pts">The point collection to be converted</param>
        /// <param name="defaultZ">The default z value</param>
        /// <returns>The point 3D</returns>
        public static Point3dCollection ToPoint3d(this Point2dCollection pts, double defaultZ = 0)
        {
            return new Point3dCollection(pts.OfType<Point2d>().Select<Point2d, Point3d>(x => x.ToPoint3d()).ToArray());
        }
        /// <summary>
        /// Returns the delta x between two points
        /// Δx=x1-x0
        /// </summary>
        /// <param name="pt0">The first point</param>
        /// <param name="pt1">The second point</param>
        /// <returns>The difference in X</returns>
        public static Double DeltaX(this Point2d pt0, Point2d pt1)
        {
            return pt1.X - pt0.X;
        }
        /// <summary>
        /// Returns the delta y between two points
        /// Δy=y1-y0
        /// </summary>
        /// <param name="pt0">The first point</param>
        /// <param name="pt1">The second point</param>
        /// <returns>The difference in Y</returns>
        public static Double DeltaY(this Point2d pt0, Point2d pt1)
        {
            return pt1.Y - pt0.Y;
        }
        /// <summary>
        /// Converts a 3D point to a 2D Point
        /// </summary>
        /// <param name="pt">The point to be converted</param>
        /// <returns>The point 2D</returns>
        public static Point2d ToPoint2d(this Point3d pt)
        {
            return pt.Convert2d(new Plane(new Point3d(), Vector3d.ZAxis));
        }
        /// <summary>
        /// Get a polar point using this point as base point
        /// </summary>
        /// <param name="pt">The base point</param>
        /// <param name="r">The distance to the next point</param>
        /// <param name="ang">The angle to the next point</param>
        /// <returns>The point 2D</returns>
        public static Point2d ToPoint2dByPolar(this Point2d pt, Double r, Double ang)
        {
            return new Point2d(pt.X + r * Math.Cos(ang), pt.Y + r * Math.Sin(ang));
        }

        /// <summary>
        /// Get the 3d Point collection of a polyline
        /// </summary>
        /// <param name="pl">The polyline to extract its vertices</param>
        /// <returns>The point 3D Collection</returns>
        public static Point3dCollection Vertices(this Polyline pl)
        {
            Point3dCollection pts = new Point3dCollection();
            for (int i = 0; i < pl.NumberOfVertices; i++)
                pts.Add(pl.GetPoint3dAt(i));
            return pts;
        }
        /// <summary>
        /// Gets the average of a collection of points 3d
        /// </summary>
        /// <param name="pts">The point collection</param>
        /// <param name="xAvg">The average on X</param>
        /// <param name="yAvg">The average on Y</param>
        /// <param name="zAvg">The average on Z</param>
        public static void Average(this Point3dCollection pts, out Double xAvg, out Double yAvg, out Double zAvg)
        {
            xAvg = 0;
            yAvg = 0;
            zAvg = 0;
            for (int i = 0; i < pts.Count; i++)
            {
                xAvg += pts[i].X;
                yAvg += pts[i].Y;
                zAvg += pts[i].Z;
            }
            xAvg /= pts.Count;
            yAvg /= pts.Count;
            zAvg /= pts.Count;
        }
        /// <summary>
        /// Gets the average of a collection of points 2d
        /// </summary>
        /// <param name="pts">The point collection</param>
        /// <param name="xAvg">The average on X</param>
        /// <param name="yAvg">The average on Y</param>
        public static void Average(this Point2dCollection pts, out Double xAvg, out Double yAvg)
        {
            xAvg = 0;
            yAvg = 0;
            for (int i = 0; i < pts.Count; i++)
            {
                xAvg += pts[i].X;
                yAvg += pts[i].Y;
            }
            xAvg /= pts.Count;
            yAvg /= pts.Count;
        }
        /// <summary>
        /// Gets the average of a collection of points 3d
        /// </summary>
        /// <param name="pts">The point collection</param>
        /// <returns>The average point</returns>
        public static Point3d Average(this Point3dCollection pts)
        {
            Double xAvg, yAvg, zAvg;
            pts.Average(out xAvg, out yAvg, out zAvg);
            return new Point3d(xAvg, yAvg, zAvg);
        }
        /// <summary>
        /// Gets the average of a collection of points 2d
        /// </summary>
        /// <param name="pts">The point collection</param>
        /// <returns>The average point</returns>
        public static Point2d Average(this Point2dCollection pts)
        {
            Double xAvg, yAvg;
            pts.Average(out xAvg, out yAvg);
            return new Point2d(xAvg, yAvg);
        }

        /// <summary>
        /// Get the geometric extents middle point from an entity. 
        /// </summary>
        /// <param name="entity">The entity to obtain its middle point</param>
        /// <returns>The middle point</returns>
        public static Point3d GeometricExtentsCenter(this Entity entity)
        {
            return entity.GeometricExtents.MinPoint.MiddlePointTo(entity.GeometricExtents.MaxPoint);
        }

        /// <summary>
        /// Get the geometric extents for a point collection
        /// </summary>
        /// <param name="coll">The collection of points</param>
        /// <param name="min">The minimum point</param>
        /// <param name="max">The maximum point</param>
        public static void GeometricExtents(this Point2dCollection coll, out Point2d min, out Point2d max)
        {
            if (coll.Count > 0)
            {
                IEnumerable<Double> xs = coll.OfType<Point2d>().Select<Point2d, Double>(x => x.X),
                                    ys = coll.OfType<Point2d>().Select<Point2d, Double>(x => x.Y);
                min = new Point2d(xs.Min(), ys.Min());
                max = new Point2d(xs.Max(), ys.Max());
            }
            else
            {
                min = new Point2d();
                max = new Point2d();
            }
        }
        /// <summary>
        /// Get the geometric extents from a collection of arrays
        /// </summary>
        /// <param name="coll">The collection of points on arrays</param>
        /// <param name="min">The minimum point</param>
        /// <param name="max">The maximum point</param>
        public static void GeometricExtents(out Point2d min, out Point2d max, params Point2d[][] pts)
        {
            Point2dCollection coll = new Point2dCollection();
            foreach (Point2d[] arr in pts)
                foreach (Point2d pt in arr)
                    coll.Add(pt);
            coll.GeometricExtents(out min, out max);
        }
        /// <summary>
        /// Applies a margin to a point
        /// </summary>
        /// <param name="pt">The point to apply the margin</param>
        /// <param name="rotation">The angle rotation for the point</param>
        /// <param name="margin">The margin rotation</param>
        /// <returns>The Point2d aligned to the margin</returns>
        public static Point2d SetMargin(this Point2d pt, Double rotation, Margin margin)
        {
            Double x = pt.X + (margin.Left - margin.Right) * Math.Cos(rotation),
                   y = pt.Y + (margin.Bottom - margin.Top) * Math.Sin(rotation);
            return new Point2d(x, y);
        }


        /// <summary>
        /// Get the Middle point from two points
        /// </summary>
        /// <param name="pt">The initial point</param>
        /// <param name="ptB">The end point</param>
        /// <returns>The middle point</returns>
        public static Point3d MiddlePointTo(this Point3d pt, Point3d ptB)
        {
            return new Point3d((pt.X + ptB.X) / 2, (pt.Y + ptB.Y) / 2, (pt.Z + ptB.Z) / 2);
        }

        /// <summary>
        /// Get the Middle point from two points
        /// </summary>
        /// <param name="pt">The initial point</param>
        /// <param name="ptB">The end point</param>
        /// <returns>The middle point</returns>
        public static Point2d MiddlePointTo(this Point2d pt, Point2d ptB)
        {
            return new Point2d((pt.X + ptB.X) / 2, (pt.Y + ptB.Y) / 2);
        }
        /// <summary>
        /// Extract the vertices from a polyline
        /// </summary>
        /// <param name="pl">The polyline to extract its vertices</param>
        /// <returns>The polyline vertices</returns>
        public static Point2dCollection ExtractVertices(this Polyline pl)
        {
            Point2dCollection pts = new Point2dCollection();
            for (int i = 0; i < pl.NumberOfVertices; i++)
                pts.Add(pl.GetPoint2dAt(i));
            return pts;
        }

        /// <summary>
        /// Format a 2D Point
        /// </summary>
        /// <param name="pt">The point to be formated</param>
        /// <param name="numDecimals">The number of decimals in the point</param>
        /// <param name="printParenthesis">If true the format use parenthesis</param>
        /// <returns>The point formatted</returns>
        public static string ToFormat(this Point2d pt, int numDecimals, bool printParenthesis = true)
        {
            const int COORDS = 2;
            StringBuilder sb = new StringBuilder();
            if (printParenthesis)
                sb.Append('(');
            for (int i = 0; i < COORDS; i++)
            {
                sb.Append('{'); sb.Append(i); sb.Append(":N"); sb.Append(numDecimals); sb.Append("}");
                if (i < COORDS - 1)
                    sb.Append(", ");
            }
            if (printParenthesis)
                sb.Append(')');
            return String.Format(sb.ToString(), pt.X, pt.Y);
        }
        /// <summary>
        /// Format a 3D Point
        /// </summary>
        /// <param name="pt">The point to be formated</param>
        /// <param name="numDecimals">The number of decimals in the point</param>
        /// <param name="printParenthesis">If true the format use parenthesis</param>
        /// <returns>The point formatted</returns>
        public static string ToFormat(this Point3d pt, int numDecimals, bool printParenthesis = true)
        {
            const int COORDS = 2;
            StringBuilder sb = new StringBuilder();
            if (printParenthesis)
                sb.Append('(');
            for (int i = 0; i < COORDS; i++)
            {
                sb.Append('{'); sb.Append(i); sb.Append(":N"); sb.Append(numDecimals); sb.Append("}");
                if (i < COORDS - 1)
                    sb.Append(", ");
            }
            if (printParenthesis)
                sb.Append(')');
            return String.Format(sb.ToString(), pt.X, pt.Y);
        }


    }
}
