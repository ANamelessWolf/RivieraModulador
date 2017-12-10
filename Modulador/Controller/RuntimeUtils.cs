using DaSoft.Riviera.Modulador.Bordeo.Model;
using DaSoft.Riviera.Modulador.Core.Model;
using DaSoft.Riviera.Modulador.Core.Runtime;
using DaSoft.Riviera.Modulador.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaSoft.Riviera.Modulador.Controller
{
    /// <summary>
    /// Defines the application utils class
    /// </summary>
    public static partial class RuntimeUtils
    {
        /// <summary>
        /// Initializes the database.
        /// </summary>
        /// <param name="app">The riviera application.</param>
        /// <param name="database">The database to initialize on the application.</param>
        public static void InitDatabase(this RivieraApplication app, params RivieraDatabase[] database)
        {
            DesignLine[] spLines = new DesignLine[] { DesignLine.Bordeo };
            RivieraDesignDatabase[] dbs = new RivieraDesignDatabase[] { new BordeoDesignDatabase() };
            WinAppInitializer win = new WinAppInitializer();
            win.Show();
            app.Database.Init(win);
        }
    }
}
