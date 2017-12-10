using Autodesk.AutoCAD.Runtime;
using DaSoft.Riviera.Modulador.Controller;
using DaSoft.Riviera.Modulador.Core.Controller;
using DaSoft.Riviera.Modulador.Core.Runtime;
using DaSoft.Riviera.Modulador.Core.UI;
using Nameless.Libraries.HoukagoTeaTime.Mio.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DaSoft.Riviera.Modulador.Core.Assets.CMDS;
using System.Windows.Controls;

namespace DaSoft.Riviera.Modulador
{
    /// <summary>
    /// Defines the application commands
    /// </summary>
    public partial class ModuladorCommands : MioPlugin
    {
        /// <summary>
        /// Gets the riviera application.
        /// </summary>
        /// <value>
        /// The riviera application.
        /// </value>
        public RivieraApplication RivApp
        {
            get { return App.Riviera; }
        }
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public override string Name => "Modulador Riviera";

        public override string[] Tabs
        {
            get
            {
                if (App.Riviera.DeveloperMode)
                    return new String[]
                    {
                       "Mamparas",
                       "Developer"
                    };
                else
                    return new String[]
                    {
                        "Mamparas"
                    };
            }
        }

        public override UserControl[] Controls => throw new NotImplementedException();

        /// <summary>
        /// Initializes the delta application.
        /// </summary>
        [CommandMethod(START)]
        public void InitDeltaApplication()
        {
            if (App.Riviera == null)
                App.Riviera = new RivieraApplication();
            RivApp.Is3DEnabled = false;
            RivApp.Database.DatabaseLoaded =
            RivApp.InitDatabase();
        }
        /// <summary>
        /// Configurations this instance.
        /// </summary>
        [CommandMethod(CONFIG_UI)]
        public void Configuration()
        {
            if (App.Riviera == null)
                App.Riviera = new RivieraApplication();
            WinAppSettings win = new WinAppSettings();
            win.ShowDialog();
        }
    }
}
