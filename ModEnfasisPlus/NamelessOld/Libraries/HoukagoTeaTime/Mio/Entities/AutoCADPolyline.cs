using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using NamelessOld.Libraries.HoukagoTeaTime.Assets;
using NamelessOld.Libraries.HoukagoTeaTime.Ritsu.Shapes2D;
using NamelessOld.Libraries.HoukagoTeaTime.Runtime;
using NamelessOld.Libraries.Yggdrasil.Lilith;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessOld.Libraries.HoukagoTeaTime.Mio.Entities
{
    public class AutoCADPolyline : NamelessObject
    {
        /// <summary>
        /// The geometry of the polyline
        /// </summary>
        public readonly Geometry2D Geometry;
        /// <summary>
        /// The name of the block where the polyline is going to be drew
        /// </summary>
        public readonly String Blockname;
        /// <summary>
        /// The polyline Object Id
        /// </summary>
        public readonly new ObjectId Id;
        /// <summary>
        /// Create a new autocad block polyline
        /// </summary>
        /// <param name="plGeometry">The polyline geometry</param>
        /// <param name="blockname">The name of the block table record to draw the polyline, 
        /// if empty the polyline is draw on the current space</param>
        public AutoCADPolyline(Geometry2D plGeometry, String blockname = "")
        {
            try
            {
                this.Blockname = blockname;
                this.Id = Drawer.Geometry2D(plGeometry, blockname, true);
            }
            catch (Exception exc)
            {
                throw new RomioException(String.Format("{0}: {1}", Errors.CreatingPolylineGeometry, exc.Message), exc);
            }
        }
        /// <summary>
        /// Create a new autocad block polyline
        /// </summary>
        /// <param name="plGeometry">The polyline geometry</param>
        /// <param name="blockname">The name of the block table record to draw the polyline, 
        /// <param name="tr">An active transaction</param>
        public AutoCADPolyline(Geometry2D plGeometry, string blockname, Transaction tr)
        {
            try
            {
                this.Blockname = blockname;
                this.Id = Drawer.Geometry2D(plGeometry, tr, blockname, true);
            }
            catch (Exception exc)
            {
                throw new RomioException(String.Format("{0}: {1}", Errors.CreatingPolylineGeometry, exc.Message), exc);
            }
        }

    }
}
