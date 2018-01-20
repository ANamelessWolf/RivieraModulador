using DaSoft.Riviera.Modulador.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Nameless.Libraries.DB.Misa;
using DaSoft.Riviera.Modulador.Core.Runtime;
using Nameless.Libraries.Yggdrasil.Lilith;
using static DaSoft.Riviera.Modulador.Core.Assets.Strings;
using static DaSoft.Riviera.Modulador.Core.Controller.UI.DialogUtils;
using Nameless.Libraries.DB.Mikasa;
using DaSoft.Riviera.Modulador.Core.Model.DB;
using System.ComponentModel;
using DaSoft.Riviera.Modulador.Core.UI;
using System.Collections;

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
        /// <returns></returns>
        public static String GetProjectName(this UserCredential userCred)
        {
            try
            {
                var tr = new Oracle_Transaction<Object, String>(App.Riviera.OracleConnection);
                tr.Transaction =
                    (DB_Connector conn, Object[] input) =>
                    {
                        UserCredential cred = (UserCredential)input[0];
                        int ejId = Ejecutivo.GetEjecutivoId(cred, conn);
                        RivieraProject p = RivieraProject.SelectProject(ejId, cred.ProjectId, conn);
                        return p.ProjectName;
                    };
                return tr.Run(userCred);
            }
            catch (Exception exc)
            {
                throw exc.CreateNamelessException<RivieraException>(ERR_GET_PRJ_NAME);
            }
        }
        /// <summary>
        /// Logins this instance.
        /// </summary>
        public static void Login(this CtrlOracleLogin login, UserCredential userCred)
        {
            var tr = new Oracle_BackgroundTransaction<Object, Object>();
            //Background Task
            tr.Transaction = (DB_Connector conn, Object[] input) =>
            {
                RivieraProject project;
                UserCredential cred = input[0] as UserCredential;
                String projectName = input[1] as String;
                int exId = Ejecutivo.GetEjecutivoId(cred, conn);
                if (exId == -1)
                    throw new RivieraException(ERR_INVALID_USER_PASS);
                project = RivieraProject.SelectProject(exId, projectName, conn);
                return project;
            };
            //Task Completed action
            tr.TaskCompleted = async (BackgroundWorker worker, object result) =>
            {
                String msg;
                RivieraProject prj = null;
                if (result is Exception)
                    msg = (result as Exception).Message;
                else if (result != null)
                {
                    prj = (RivieraProject)result;
                    login.ProjectId = prj.Id;
                    userCred.ProjectId = login.ProjectId;
                    App.Riviera.Credentials = userCred;
                    msg = String.Format(MSG_SESS_INIT, userCred.Username, prj.ProjectName);
                }
                else
                    msg = String.Format(ASK_NEW_PROJECT, login.tboProject.Text);
                if (!(result is Exception) && (prj == null && await login.ShowQuestionDialog(msg, String.Empty)))
                    login.Login_CreateProject(userCred);
                else
                {
                    await login.ShowDialog((result is Exception) ? ERR_LOGIN : CAPTION_LOG_IN, msg);
                    await CloseProgressDialog();
                }
            };
            tr.Run(App.Riviera.OracleConnection, userCred, login.tboProject.Text);
        }
        /// <summary>
        /// Obtiene la lista de proyectos disponibles
        /// </summary>
        /// <returns>La colección de proyectos disponibles</returns>
        internal static IEnumerable<RivieraProject> GetProjects()
        {
            if (App.Riviera.OracleConnection != null)
            {
                var tr = new Oracle_Transaction<object, IEnumerable<RivieraProject>>(App.Riviera.OracleConnection);
                tr.Transaction = (DB_Connector conn, object[] trParameters) =>
                {
                    return RivieraProject.SelectProjects(conn);
                };
                return tr.Run();
            }
            else
                return new RivieraProject[0];
        }

        /// <summary>
        /// Login to the database creating a new project.
        /// </summary>
        /// <param name="login">The UI login.</param>
        /// <param name="userCred">The user credentials.</param>
        public static void Login_CreateProject(this CtrlOracleLogin login, UserCredential userCred)
        {
            var tr = new Oracle_BackgroundTransaction<Object, Object>();
            //Background Task
            tr.Transaction = (DB_Connector conn, Object[] input) =>
            {
                UserCredential cred = input[0] as UserCredential;
                String projectName = input[1] as String;
                int exId = Ejecutivo.GetEjecutivoId(cred, conn);
                RivieraProject pData = new RivieraProject(projectName, exId),
                               newProject;//Nuevo proyecto
                if (pData.Insert<RivieraProject>(conn, out newProject))
                    return newProject;
                else
                    throw new RivieraException(String.Format(ERR_INS_PRJ, projectName, conn.Errors.Last().Message));
            };
            tr.TaskCompleted = async (BackgroundWorker worker, object result) =>
            {
                String msg;
                RivieraProject prj = null;
                if (result is Exception)
                    msg = (result as Exception).Message;
                else
                {
                    prj = (RivieraProject)result;
                    login.ProjectId = prj.Id;
                    App.Riviera.Credentials = userCred;
                    msg = String.Format(MSG_SESS_INIT, userCred.Username, prj.ProjectName);
                }
                await login.ShowDialog((result is Exception) ? ERR_LOGIN : CAPTION_LOG_IN, msg);
                await CloseProgressDialog();
            };
            tr.Run(App.Riviera.OracleConnection, userCred, login.tboProject.Text);
        }
    }
}
