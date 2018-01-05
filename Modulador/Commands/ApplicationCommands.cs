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
using DaSoft.Riviera.Modulador.Enfasis.UI;
using DaSoft.Riviera.Modulador.Bordeo.UI;
using Nameless.Libraries.HoukagoTeaTime.Yui;

namespace DaSoft.Riviera.Modulador.Commands
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
        /// <summary>
        /// Gets the tabs.
        /// </summary>
        /// <value>
        /// The tabs.
        /// </value>
        public override string[] Tabs
        {
            get
            {
                if (App.Riviera.DeveloperMode)
                    return new String[]
                    {
                       "Bordeo",
                       "Enfasis Plus",
                       "Developer"
                    };
                else
                    return new String[]
                    {
                       "Bordeo",
                       "Enfasis Plus",
                    };
            }
        }
        /// <summary>
        /// Gets the controls.
        /// </summary>
        /// <value>
        /// The controls.
        /// </value>
        public override UserControl[] Controls
        {
            get
            {
                if (App.Riviera.DeveloperMode && _Controls == null)
                    _Controls = new UserControl[]
                {
                    new TabBordeoMenu(),
                    new TabEnfasisMenu(),
                    new TabDevMenu()
                };
                else if (_Controls == null)
                    _Controls = new UserControl[]
                {
                    new TabBordeoMenu(),
                    new TabEnfasisMenu()
                };
                return _Controls;
            }
        }
        /// <summary>
        /// The controls
        /// </summary>
        private UserControl[] _Controls;
        /// <summary>
        /// Initializes the delta application.
        /// </summary>
        [CommandMethod(START)]
        public void InitDeltaApplication()
        {
            if (App.Riviera == null)
                App.Riviera = new RivieraApplication();
            RivApp.Is3DEnabled = false;
            RivApp.Database.DatabaseLoaded = InitCommand;
            RivApp.InitDatabase();
        }
        /// <summary>
        /// Initialize the delta
        /// </summary>
        [CommandMethod(INIT_APP_UI)]
        public override void InitCommand()
        {
            App.RunCommand(
                delegate ()
                {
                    base.InitCommand();
                    try
                    {
                    }
                    catch (System.Exception exc)
                    {
                        Selector.Ed.WriteMessage(exc.Message);
                    }
                });
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
