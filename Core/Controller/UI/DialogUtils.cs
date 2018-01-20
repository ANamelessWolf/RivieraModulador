using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WinApp = System.Windows.Application;
namespace DaSoft.Riviera.Modulador.Core.Controller.UI
{
    /// <summary>
    /// Defines the user interface utils
    /// </summary>
    public static partial class DialogUtils
    {
        /// <summary>
        /// The active window
        /// </summary>
        public static MetroWindow ActiveWindow;
        /// <summary>
        /// Progress dialog controller
        /// </summary>
        static ProgressDialogController ProgressController;
        /// <summary>
        /// Shows a pop up dialog
        /// </summary>
        /// <param name="control">The user control.</param>
        /// <param name="title">The message title.</param>
        /// <param name="msg">The message content.</param>
        public static async Task ShowDialog(this UserControl control, String title, String msg)
        {
            var metroWindow = MetroWindow.GetWindow(control) as MetroWindow;
            MessageDialogResult res = await metroWindow.ShowMessageAsync(title, msg, MessageDialogStyle.Affirmative);
        }
        /// <summary>
        /// Shows the question dialog.
        /// </summary>
        /// <param name="control">The user control.</param>
        /// <param name="title">The message title.</param>
        /// <param name="msg">The message content.</param>
        /// <returns>True if the user agree</returns>
        public static async Task<Boolean> ShowQuestionDialog(this UserControl control, String title, String msg)
        {
            var metroWindow = MetroWindow.GetWindow(control) as MetroWindow;
            MessageDialogResult res = await metroWindow.ShowMessageAsync(title, msg, MessageDialogStyle.AffirmativeAndNegative);
            return res == MessageDialogResult.Affirmative;
        }
        /// <summary>
        /// Start Progeess dialog using the Active Window <see cref="ActiveWindow"/>
        /// </summary>
        /// <param name="title">The progress dialog title.</param>
        /// <param name="msg">The progress dialog message.</param>
        /// <returns></returns>
        public static async Task ShowProgressDialog(String title, String msg)
        {
            ProgressController = await ActiveWindow.ShowProgressAsync(title, msg);
            ProgressController.SetCancelable(false);
            ProgressController.SetIndeterminate();
        }
        /// <summary>
         /// 
         /// </summary>
         /// <param name="title"></param>
         /// <param name="msg"></param>
         /// <returns></returns>
        public static async Task CloseProgressDialog()
        {
            await ProgressController.CloseAsync();
            ProgressController = null;
        }
        /// <summary>
        /// Gets the window.
        /// </summary>
        /// <param name="connector">The Oracle connector.</param>
        /// <returns>The Active Window</returns>
        public static MetroWindow GetWindow(this IOracleUIConnector connector)
        {
            return connector.Sender.GetWindow();
        }
        /// <summary>
        /// Gets the window.
        /// </summary>
        /// <param name="connector">The Oracle connector.</param>
        /// <returns>The Active Window</returns>
        public static MetroWindow GetWindow(this UserControl control)
        {
            return MetroWindow.GetWindow(control) as MetroWindow;
        }
    }
}
