using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessOld.Libraries.HoukagoTeaTime.Ritsu.Shapes2D
{
    public class Polygon2D : Geometry2D
    {
        /// <summary>
        /// The polygon center
        /// </summary>
        public Point2d Center { get { return cen; } }
        Point2d cen;
        /// <summary>
        /// The apothem of the polygon.
        /// </summary>
        public Double Apothem { get { return apo; } }
        Double apo;
        /// <summary>
        /// The number of sides of the polygon.
        /// </summary>
        public int Sides { get { return nsides; } }
        /// <summary>
        /// The polygon Length
        /// </summary>
        public override double Length
        {
            get
            {
                return this.apo * 2;
            }
        }
        /// <summary>
        /// The polygon Width
        /// </summary>
        public override double Width
        {
            get
            {
                return this.apo * 2;
            }
        }
        /// <summary>
        /// The polygon perimeter
        /// </summary>
        public override double Perimeter
        {
            get
            {
                return this.Vertices[0].GetDistanceTo(this.Vertices[1]) * this.Sides;
            }
        }
        int nsides;
        /// <summary>
        /// Creates a polygon geometry, specifying the number the sides
        /// and the apothem length.
        /// </summary>
        /// <param name="sides">The number of sides of the polygon</param>
        /// <param name="center">The polygon center.</param>
        /// <param name="apothem">The length of the apothem.</param>
        public Polygon2D(int sides, Point2d center, double apothem) :
            base(new Point2dCollection(sides))
        {
            double x, y, radian;
            double angle = 360 / sides;
            this.apo = apothem;
            this.cen = center;
            this.nsides = sides;
            radian = 180 / Math.PI;
            for (int v = 0; v < sides; v++)
            {
                x = apothem * Math.Cos((angle * (v + 1)) / radian);
                y = apothem * Math.Sin((angle * (v + 1)) / radian);
                x += center.X;
                y += center.Y;
                base.Vertices.Add(new Point2d(x, y));
            }
        }
    }
}
