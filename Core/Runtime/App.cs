using Nameless.Libraries.HoukagoTeaTime.Yui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DaSoft.Riviera.Modulador.Core.Assets.Strings;
using static DaSoft.Riviera.Modulador.Core.Assets.CMDS;
namespace DaSoft.Riviera.Modulador.Core.Runtime
{
    /// <summary>
    /// Defines the application entry point
    /// </summary>
    public class App
    {

        /// <summary>
        /// The riviera application object
        /// </summary>
        public static RivieraApplication Riviera;
        /// <summary>
        /// The riviera database
        /// </summary>
        public static RivieraDatabase DB;
        /// <summary>
        /// Gets a value indicating whether this instance is ready.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is ready; otherwise, <c>false</c>.
        /// </value>
        public static bool IsReady
        {
            get { return Riviera != null && DB != null; }
        }
        /// <summary>
        /// Defines a command action
        /// </summary>
        public delegate void CommandHandler();
        /// <summary>
        /// Runs an AutoCAD command.
        /// </summary>
        /// <param name="cmd">The command to run.</param>
        public static void RunCommand(CommandHandler cmd)
        {
            if (App.IsReady)
            {
                try
                {
                    cmd();
                }
                catch (Exception exc)
                {

                    Selector.Ed.WriteMessage(exc.Message);
                }
            }
            else
                Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage(String.Format(WAR_START_CMD, START));
        }
    }
}
