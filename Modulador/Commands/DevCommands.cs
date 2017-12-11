using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using DaSoft.Riviera.Modulador.Bordeo.Testing;
using DaSoft.Riviera.Modulador.Core.Model;
using DaSoft.Riviera.Modulador.Core.Runtime;
using Nameless.Libraries.HoukagoTeaTime.Mio.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DaSoft.Riviera.Modulador.Core.Assets.CMDS;
namespace DaSoft.Riviera.Modulador.Commands
{
    /// <summary>
    /// Defines the application commands
    /// </summary>
    public partial class DevCommands
    {
        /// <summary>
        /// Initializes the delta application.
        /// </summary>
        [CommandMethod("LoadMockingDatabase")]
        public void InitDeltaApplication()
        {
            if (App.Riviera != null)
                App.Riviera = new RivieraApplication();
            App.Riviera.Is3DEnabled = false;
            App.Riviera.Database = new RivieraDatabase();
            App.Riviera.Database.LineDB.Add(DesignLine.Bordeo, new BordeoMockingDesignDatabase());
            var doc = Application.DocumentManager.MdiActiveDocument;
            doc.SendStringToExecute(INIT_APP_UI, true, false, false);
        }
    }
}
