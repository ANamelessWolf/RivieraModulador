using Autodesk.AutoCAD.Runtime;
using DaSoft.Riviera.Modulador.Core.Controller;
using DaSoft.Riviera.Modulador.Core.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DaSoft.Riviera.Modulador.Core.Assets.CMDS;
namespace DaSoft.Riviera.Modulador
{
    /// <summary>
    /// Defines the application commands
    /// </summary>
    public partial class ModuladorCommands
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
        /// Initializes the delta application.
        /// </summary>
        [CommandMethod(START)]
        public void InitDeltaApplication()
        {
            App.Riviera = new RivieraApplication();
            RivApp.Is3DEnabled = false;
            RivApp.InitDatabase();
        }
    }
}
