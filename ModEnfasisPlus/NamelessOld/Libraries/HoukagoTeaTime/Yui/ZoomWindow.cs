using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using NamelessOld.Libraries.HoukagoTeaTime.Ritsu;
using NamelessOld.Libraries.Yggdrasil.Lilith;
using System;

namespace NamelessOld.Libraries.HoukagoTeaTime.Yui
{
    public class ZoomWindow : NamelessObject
    {
        /// <summary>
        /// The min point of the zoom window
        /// </summary>
        public Point2d MinPoint;
        /// <summary>
        /// The max point of the zoom window
        /// </summary>
        public Point2d MaxPoint;
        /// <summary>
        /// Creates a new zoom window from two 3d points
        /// </summary>
        /// <param name="center">The view window center</param>
        /// <param name="height">The view window height</param>
        /// <param name="width">The view window width</param>
        public ZoomWindow(Point3d center, Double width, Double height) :
            this(new Point2d(center.X - width, center.Y - height), new Point2d(center.X + width, center.Y + height))
        {
        }
        /// <summary>
        /// Creates a new zoom window from two 3d points
        /// </summary>
        /// <param name="ptMin">The min point</param>
        /// <param name="ptMax">The max point</param>
        public ZoomWindow(Point3d ptMin, Point3d ptMax)
        {
            MinPoint = new Point2d(ptMin.X, ptMin.Y);
            MaxPoint = new Point2d(ptMax.X, ptMax.Y);
        }
        /// <summary>
        /// Creates a new zoom window from two 3d points
        /// </summary>
        /// <param name="ptMin">The min point</param>
        /// <param name="ptMax">The max point</param>
        public ZoomWindow(Point2d ptMin, Point2d ptMax)
        {
            MinPoint = ptMin;
            MaxPoint = ptMax;
        }
        /// <summary>
        /// Set the current window as the current view
        /// a zoom factor help to scale the window
        /// </summary>
        /// <param name="factor">The scale factor</param>
        public void SetView(double factor = 1)
        {
            ViewTableRecord view = new ViewTableRecord();
            view.CenterPoint = this.MinPoint.MiddlePointTo(this.MaxPoint);
            view.Height = this.MaxPoint.Y - this.MinPoint.Y;
            view.Width = this.MaxPoint.X - this.MinPoint.X;
            view.Height *= factor;
            view.Width *= factor;
            Selector.Ed.SetCurrentView(view);
        }
        /// <summary>
        /// Updates the currrent window view
        /// </summary>
        /// <param name="height">The view window height</param>
        /// <param name="width">The view window width</param>
        public void UpdateView(Double width, Double height)
        {
            Point2d center = this.MinPoint.MiddlePointTo(this.MaxPoint);
            this.MinPoint = new Point2d(center.X - width, center.Y - height);
            this.MaxPoint = new Point2d(center.X - width, center.Y - height);
        }
        /// <summary>
        /// Print the window format
        /// </summary>
        /// <returns>The window formated</returns>
        public override string ToString()
        {
            return String.Format("[{0},{1}]", this.MinPoint.ToFormat(2), this.MaxPoint.ToFormat(2));
        }
    }
}
