using DaSoft.Riviera.Modulador.Bordeo.Model;
using DaSoft.Riviera.Modulador.Bordeo.Runtime;
using DaSoft.Riviera.Modulador.Core.Model;
using DaSoft.Riviera.Modulador.Core.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DaSoft.Riviera.Modulador.Bordeo.Assets.Constants;
using static DaSoft.Riviera.Modulador.Bordeo.Assets.Strings;
using Nameless.Libraries.Yggdrasil.Lilith;
using System.IO;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.DatabaseServices;
using Nameless.Libraries.HoukagoTeaTime.Ritsu.Utils;

namespace DaSoft.Riviera.Modulador.Bordeo.Controller
{
    public static class BordeoUtils
    {
        /// <summary>
        /// Gets the block directory path.
        /// </summary>
        /// <value>
        /// The block directory path.
        /// </value>
        public static String BlockDirectoryPath => Path.Combine(App.Riviera.AppDirectory.FullName, FOLDER_NAME_BLOCKS_BORDEO);
        /// <summary>
        /// Bordeoes the direction keys.
        /// </summary>
        /// <returns></returns>
        public static String[] BordeoDirectionKeys()
        {
            return new String[]
            {
               // KEY_BACK, KEY_FRONT, KEY_LEFT_135, KEY_LEFT_90, KEY_RIGHT_135, KEY_RIGHT_90, KEY_EXTRA
            };
        }
        /// <summary>
        /// Gets the database for the design line bordeo.
        /// </summary>
        /// <returns>The bordeo database</returns>
        public static BordeoDesignDatabase GetDatabase()
        {
            var rivApp = App.Riviera;
            if (rivApp != null && rivApp.Database != null && rivApp.Database.LineDB.ContainsKey(DesignLine.Bordeo))
                return rivApp.Database.LineDB[DesignLine.Bordeo] as BordeoDesignDatabase;
            else
                throw new BordeoException(ERR_DB_NOT_READY);
        }
        /// <summary>
        /// Gets the database for the design line bordeo.
        /// </summary>
        /// <param name="code">The riviera code</param>
        /// <returns>The bordeo database</returns>
        public static RivieraCode GetRivieraCode(String code)
        {
            try
            {
                var db = GetDatabase();
                var rivCode = db.Codes.FirstOrDefault(x => x.Code == code);
                if (rivCode == null)
                    throw new BordeoException(String.Format(ERR_CODE_NOT_FOUND, code));
                return rivCode;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        /// <summary>
        /// Gets the end point.
        /// </summary>
        /// <param name="geometry">The end point of the geometry.</param>
        /// <returns>The end point of the geometry</returns>
        public static Point2d GetEndPoint(this Line geometry, PanelMeasure size)
        {
            Point2d start = geometry.StartPoint.ToPoint2d(),
                end = geometry.EndPoint.ToPoint2d();
            Double angle = start.GetVectorTo(end).Angle;
            Double distance = size.Frente.Real;
            return start.ToPoint2dByPolar(distance, angle);
        }
    }
}
