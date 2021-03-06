﻿using DaSoft.Riviera.Modulador.Core.Controller.Transactions;
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
        /// La lista de objetos cargados en la aplicación.
        /// </summary>
        public List<RivieraObject> Objects;
        /// <summary>
        /// La lista de objetos en la aplicación que son validos y no han sido borrados.
        /// </summary>
        public IEnumerable<RivieraObject> ValidObjects => Objects.Where(x => x.Id.IsValid && !x.Id.IsErased);
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
            this.Objects = new List<RivieraObject>();
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
        /// <summary>
        /// Gets the size.
        /// </summary>
        /// <param name="line">The design line.</param>
        /// <param name="code">The riviera design code</param>
        /// <param name="values">The nominal values.</param>
        public RivieraMeasure GetSize(DesignLine line, String code, KeyValuePair<string, double>[] values)
        {
            try
            {
                RivieraDesignDatabase db = this.LineDB[line];
                var sizes = db?.Sizes.FirstOrDefault(x => x.Key == code).Value.Sizes;
                return sizes.FirstOrDefault(x => x.HasSize(values));
            }
            catch (Exception)
            {
                throw new RivieraException(ERR_SIZE_NOT_EXIST);
            }

        }
        /// <summary>
        /// Cleans this instance, removing invalid objects from the database
        /// </summary>
        public void Clean()
        {
            var invalidObjs = Objects.Where(x => !(x.Id.IsValid && !x.Id.IsErased));
            foreach (var obj in this.Objects)
                this.Objects.Remove(obj);
        }
    }
}
