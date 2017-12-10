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
namespace DaSoft.Riviera.Modulador.Bordeo.Controller
{
    public static class BordeoUtils
    {
        /// <summary>
        /// Bordeoes the direction keys.
        /// </summary>
        /// <returns></returns>
        public static String[] BordeoDirectionKeys()
        {
            return new String[] 
            {
                KEY_BACK, KEY_FRONT, KEY_LEFT_135, KEY_LEFT_90, KEY_RIGHT_135, KEY_RIGHT_90, KEY_EXTRA
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
    }
}
