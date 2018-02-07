using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System.Collections.Generic;
using System.Linq;

namespace NamelessOld.Libraries.HoukagoTeaTime.Yui
{
    public abstract class AreaSelector : Selection
    {
        /// <summary>
        /// Area Selection geometry
        /// </summary>
        public Point3dCollection Vertices;
        /// <summary>
        /// Creates a selection area
        /// The options are defined on the class methods
        /// </summary>
        /// <param name="pts">The area vertices</param>
        public AreaSelector(Point3dCollection pts)
        {
            this.Vertices = pts;
        }
        /// <summary>
        /// Filter the current collection of selected ids
        /// </summary>
        /// <param name="allowedIds">The collection of not allowed ids</param>
        /// <returns>The filtered elements</returns>
        public IEnumerable<ObjectId> Filter(ObjectIdCollection ignoreIds)
        {
            return this.Ids.OfType<ObjectId>().Where(x => !ignoreIds.Contains(x));
        }
        /// <summary>
        /// Filter the current collection of selected ids
        /// </summary>
        /// <param name="allowedIds">The collection of not allowed ids</param>
        /// <returns>The filtered elements</returns>
        public IEnumerable<ObjectId> Filter(params ObjectId[] ignoreIds)
        {
            return this.Ids.OfType<ObjectId>().Where(x => !ignoreIds.Contains(x));
        }
    }
}
