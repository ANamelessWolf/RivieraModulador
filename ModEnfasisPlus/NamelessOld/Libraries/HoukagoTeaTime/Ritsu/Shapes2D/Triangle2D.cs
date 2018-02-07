using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessOld.Libraries.HoukagoTeaTime.Ritsu.Shapes2D
{
    public class Triangle2D : Geometry2D
    {
        /// <summary>
        /// Triangle start point A
        /// </summary>
        public Point2d A { get { return this.Vertices[0]; } }
        /// <summary>
        /// Triangle second point B
        /// </summary>
        public Point2d B { get { return this.Vertices[1]; } }
        /// <summary>
        /// Triangle third point C
        /// </summary>
        public Point2d C { get { return this.Vertices[2]; } }
        /// <summary>
        /// Triangle third point C
        /// </summary>
        public Point2d Centroid { get { return new Point2d((this.A.X + this.B.X + this.C.X) / 3, (this.A.Y + this.B.Y + this.C.Y) / 3); } }
        /// <summary>
        /// The angle a is the angle between side BC and Side BA
        /// </summary>
        public Double a
        {
            get
            {
                Vector2d BC = new Vector2d(C.X - B.X, C.Y - B.Y),
                         BA = new Vector2d(A.X - B.X, A.Y - B.Y);
                return BC.GetAngleTo(BA);
            }
        }
        /// <summary>
        /// The angle b is the angle between side CA and Side CB
        /// </summary>
        public Double b
        {
            get
            {
                Vector2d CA = new Vector2d(A.X - C.X, A.Y - C.Y),
                         CB = new Vector2d(B.X - C.X, B.Y - C.Y);
                return CA.GetAngleTo(CB);
            }
        }
        /// <summary>
        /// The angle c is the angle between side AB and Side AC
        /// </summary>
        public Double c
        {
            get
            {
                Vector2d AB = new Vector2d(B.X - A.X, B.Y - A.Y),
                         AC = new Vector2d(C.X - A.X, C.Y - A.Y);
                return AB.GetAngleTo(AC);
            }
        }
        /// <summary>
        /// The Triangle first side AB
        /// </summary>
        public Double AB
        {
            get { return this.A.GetDistanceTo(this.B); }
        }
        /// <summary>
        /// The Triangle second side BC
        /// </summary>
        public Double BC
        {
            get { return this.B.GetDistanceTo(this.C); }
        }
        /// <summary>
        /// The Triangle third side CA
        /// </summary>
        public Double CA
        {
            get { return this.C.GetDistanceTo(this.A); }
        }
        /// <summary>
        /// The triangle perimeter
        /// </summary>
        public override double Perimeter
        {
            get
            {
                return AB + BC + CA;
            }
        }
        /// <summary>
        /// The triangle semiperimeter
        /// </summary>
        public double SemiPerimeter { get { return Perimeter / 2; } }
        /// <summary>
        /// Get the area of the triangle
        /// </summary>
        public override double Area
        {
            get
            {
                Double area = SemiPerimeter * (SemiPerimeter - AB) * (SemiPerimeter - BC) * (SemiPerimeter - CA);
                return Math.Sqrt(area);
            }
        }
        /// <summary>
        /// Create a trinagle from three points
        /// </summary>
        /// <param name="ptA">Triangle point A</param>
        /// <param name="ptB">Triangle point B</param>
        /// <param name="ptC">Triangle point C</param>
        public Triangle2D(Point2d ptA, Point2d ptB, Point2d ptC)
            : base(new Point2d[] { ptA, ptB, ptC })
        {
        }
        /// <summary>
        /// Print the triangle data
        /// </summary>
        /// <returns>The triangle data</returns>
        public override string ToString()
        {
            return String.Format("A({0:N3},{1:N3}), B({2:N3},{3:N3}), C({4:N3},{5:N3}), α: {6:N2}, β: {7:N2}, γ: {8:N2}, P:{9:N3}, A:{10:N2}",
                Math.Round(this.A.X, 3), Math.Round(this.A.Y, 3),
                Math.Round(this.B.X, 3), Math.Round(this.B.Y, 3),
                Math.Round(this.C.X, 3), Math.Round(this.C.Y, 3),
                Math.Round(this.a * 180 / Math.PI, 2),
                Math.Round(this.b * 180 / Math.PI, 2),
                Math.Round(this.c * 180 / Math.PI, 2),
                Math.Round(this.Perimeter, 3),
                Math.Round(this.Area, 2));
        }
    }
}
