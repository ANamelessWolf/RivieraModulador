using Nameless.Libraries.Yggdrasil.Aerith;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Environment;

namespace DaSoft.Riviera.Modulador.Core.Controller.UI
{
    /// <summary>
    /// Defines the user interface utils
    /// </summary>
    public static partial class DialogUtils
    {
        /// <summary>
        /// Picks the file.
        /// </summary>
        public static void PickFile(this string startDirectory, string dialogTitle, string fileCat, Action<string> fileSelected_Action, params string[] allowedExt)
        {
            FilePicker fPck = new FilePicker(false, allowedExt);
            fPck.InitialDirectory = startDirectory;
            string pth;
            if (fPck.PickPath(fileCat, dialogTitle, out pth) && File.Exists(pth))
                fileSelected_Action(pth);
        }
        /// <summary>
        /// Picks the file.
        /// </summary>
        public static void PickDirectory(this SpecialFolder startDirectory, string dialogmsg, Action<string> directorySelected_Action)
        {
            DirectoryPicker pck = new DirectoryPicker();
            pck.InitialDirectory = startDirectory;
            string pth;
            if (pck.PickPath(dialogmsg, out pth) && Directory.Exists(pth))
                directorySelected_Action(pth);
        }
    }
}
