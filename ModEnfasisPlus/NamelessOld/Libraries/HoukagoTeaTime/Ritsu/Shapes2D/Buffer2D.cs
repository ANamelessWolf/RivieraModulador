using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessOld.Libraries.HoukagoTeaTime.Ritsu.Shapes2D
{
    public class Buffer2D : Geometry2D
    {
        const double HALF_PI = Math.PI / 2d;
        /// <summary>
        /// The buffer start point
        /// </summary>
        public Point2d Start;
        /// <summary>
        /// The buffer end point
        /// </summary>
        public Point2d End;
        /// <summary>
        /// The buffer vector direction
        /// </summary>
        public Vector2d Direction
        {
            get
            {
                return this.Start.GetVectorTo(this.End);
            }
        }
        /// <summary>
        /// The buffer angle
        /// </summary>
        public Double Angle
        {
            get
            {
                return this.Direction.Angle;
            }
        }
        /// <summary>
        /// The buffer perimeter
        /// </summary>
        public override double Perimeter
        {
            get
            {
                return base.Length * 2 + base.Width * 2;
            }
        }
        /// <summary>
        /// The buffer length
        /// </summary>
        public override double Length
        {
            get
            {
                return this.Direction.Length;
            }
        }
        /// <summary>
        /// The buffer size works as the width
        /// </summary>
        public override double Width
        {
            get
            {
                return width;
            }
        }
        /// <summary>
        /// Buffer size
        /// </summary>
        Double width;

        /// <summary>
        /// Creates a buffer area defined on two points
        /// </summary>
        /// <param name="start">The buffer starting point</param>
        /// <param name="end">The buffer ending point</param>
        /// <param name="size">The size of the buffer</param>
        public Buffer2D(Point2d start, Point2d end, double size)
        {
            this.Start = start;
            this.End = end;
            this.SetWidth(size);
            this.Create();
        }
        /// <summary>
        /// Create the buffer geometry
        /// </summary>
        public void Create()
        {
            Point2d[] vertices = new Point2d[]
            {
                this.Start.ToPoint2dByPolar(this.Width, this.Angle + HALF_PI),
                this.End.ToPoint2dByPolar(this.Width, this.Angle + HALF_PI),
                this.End.ToPoint2dByPolar(this.Width, this.Angle - HALF_PI),
                this.Start.ToPoint2dByPolar(this.Width, this.Angle - HALF_PI),
            };
            this.RefreshVertices(new Point2dCollection(vertices));
        }

        /// <summary>
        /// Changes the buffer width
        /// </summary>
        /// <param name="size">The size of the buffer</param>
        public void SetWidth(double size)
        {
            this.width = size;
        }
    }
}
