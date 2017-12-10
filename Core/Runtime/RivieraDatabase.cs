using DaSoft.Riviera.Modulador.Core.Controller.Transactions;
using DaSoft.Riviera.Modulador.Core.Model;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using static DaSoft.Riviera.Modulador.Core.Controller.UI.DialogUtils;
using System.ComponentModel;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.ApplicationServices;
using static DaSoft.Riviera.Modulador.Core.Assets.Strings;
using MahApps.Metro.Controls.Dialogs;

namespace DaSoft.Riviera.Modulador.Core.Runtime
{
    /// <summary>
    /// Defines the Riviera Database
    /// </summary>
    public class RivieraDatabase
    {
        /// <summary>
        /// The database information sorted by design line
        /// </summary>
        public Dictionary<DesignLine, RivieraDesignDatabase> LineDB;
        /// <summary>
        /// The action excecuted ones the database loaded
        /// </summary>
        public Action DatabaseLoaded;
        /// <summary>
        /// Initializes a new instance of the <see cref="RivieraDatabase"/> class.
        /// </summary>
        public RivieraDatabase()
        {
            this.LineDB = new Dictionary<DesignLine, RivieraDesignDatabase>();

        }

        /// <summary>
        /// Initializes the database
        /// </summary>
        /// <param name="win">The windwow that calls this task.</param>
        public async void Init(MetroWindow win)
        {
            ActiveWindow = win;
            await ShowProgressDialog(CAP_LOADING, MSG_LOADING_DB);
            OracleTransactions.InitDatabase(
               async (BackgroundWorker worker, Object result) =>
            {
                string msg;
                if (result is Exception)
                    msg = (result as Exception).Message;
                else
                {
                    this.InitCompleted(result);
                    msg = MSG_MEMORY_LOADED;
                }
                await CloseProgressDialog();
                await win.ShowMessageAsync(String.Empty, msg, MessageDialogStyle.Affirmative);
                win.Close();
                if (!(result is Exception) && DatabaseLoaded != null)
                    this.DatabaseLoaded();
            });
        }
        /// <summary>
        /// Initializes the database
        /// </summary>
        /// <param name="win">The windwow that calls this task.</param>
        public void Init()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            ed.WriteMessage(MSG_LOADING_DB);
            OracleTransactions.InitDatabase(
               (BackgroundWorker worker, Object result) =>
               {
                   this.InitCompleted(result);
                   ed.WriteMessage("\n{0}", MSG_MEMORY_LOADED);
               });
        }
        /// <summary>
        /// The action taken once the Initializer process has completed.
        /// </summary>
        /// <param name="result">The Initialize task result.</param>
        private void InitCompleted(object result)
        {
            RivieraDatabaseResult dbResult = (RivieraDatabaseResult)result;
            foreach (var db in LineDB.Values)
                db.LoadDesignModelData(dbResult);
        }
    }
}
