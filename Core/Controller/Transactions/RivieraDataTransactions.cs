using DaSoft.Riviera.Modulador.Core.Model;
using DaSoft.Riviera.Modulador.Core.Model.DB;
using DaSoft.Riviera.Modulador.Core.Runtime;
using Nameless.Libraries.DB.Mikasa;
using Nameless.Libraries.DB.Misa;
using Nameless.Libraries.Yggdrasil.Lilith;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DaSoft.Riviera.Modulador.Core.Assets.Strings;
namespace DaSoft.Riviera.Modulador.Core.Controller.Transactions
{
    /// <summary>
    /// Defines the transaction to ORACLE
    /// </summary>
    public static partial class OracleTransactions
    {
        /// <summary>
        /// Gets the name of the project.
        /// </summary>
        /// <param name="userCred">The user credentials.</param>
        public static void InitDatabase(DB_BackgroundTransaction<RivieraDesignDatabase, Object>.TaskFinishHandler taskCompleted)
        {
            try
            {
                var tr = new Oracle_BackgroundTransaction<RivieraDesignDatabase, Object>();
                tr.Transaction =
                    (DB_Connector conn, RivieraDesignDatabase[] rivDesignDB) =>
                    {
                        var dsgResult = new Dictionary<DesignLine, Object>();
                        //Tablas globales de la aplicación
                        RivieraDatabaseResult result = new RivieraDatabaseResult();
                        result.RivieraCodeRows = RivieraCodeRow.SelectAll(conn);
                        //Tablas exclusivas para una línea de diseño
                        foreach (RivieraDesignDatabase db in rivDesignDB)
                            dsgResult.Add(db.Line, db.InitDesignDatabase(conn));
                        result.DesignResult = dsgResult;
                        return result;
                    };
                tr.TaskCompleted = taskCompleted;
                tr.Run(App.Riviera.OracleConnection, App.Riviera.Database.LineDB.Values.ToArray());
            }
            catch (Exception exc)
            {
                throw exc.CreateNamelessException<RivieraException>(ERR_INIT_DB);
            }
        }
    }

}
