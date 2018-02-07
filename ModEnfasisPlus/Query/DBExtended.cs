using DaSoft.Riviera.OldModulador.Model;
using DaSoft.Riviera.OldModulador.Runtime;
using DaSoft.Riviera.OldModulador.UI;
using NamelessOld.Libraries.DB.Misa;
using NamelessOld.Libraries.DB.Misa.Model;
using System;
using System.Windows;

namespace DaSoft.Riviera.OldModulador.Query
{
    public static class DBExtended
    {
        /// <summary>
        /// Loads the application ProjectId
        /// </summary>
        /// <param name="conn">The oracle connector object</param>
        /// <param name="userId">The user id</param>
        /// <param name="projectname">The name of the project</param>
        /// <param name="projectId">The output is the selected projectId</param>
        /// <returns>True if the projectId is loaded</returns>
        public static bool LoadProjectId(this Oracle_Connector conn, string userId, string projectname, out string projectId)
        {
            Boolean flag = false;
            Query_Revit_Project q = new Query_Revit_Project();
            if (userId != String.Empty && projectname != String.Empty)
            {
                projectId = conn.SelectOne(q.SelectProjectId(userId, projectname));
                if (projectId == String.Empty)
                {
                    conn.Update(q.CreateProject(userId, projectname));
                    projectId = conn.SelectOne(q.SelectProjectId(userId, projectname));
                    if (projectId != String.Empty)
                        flag = true;
                }
                else
                {
                    conn.Update(q.UpdateLastAccess(userId));
                    flag = true;
                }
            }
            else
                projectId = String.Empty;
            return flag;
        }

        public static String GetProjectName(this int projectId, UserCredential cred)
        {
            String projectName = String.Empty;
            try
            {
                projectName = new BlankOracle_Transaction<String>(App.Riviera.Connection,
                    delegate (Oracle_Connector conn)
                    {
                        Query_Ejecutivo qEj = new Query_Ejecutivo();
                        Query_Revit_Project qRev = new Query_Revit_Project();
                        string userId = conn.SelectOne(qEj.SelectUserID(cred));
                        return conn.SelectOne(qRev.SelectProjectName(userId, cred.ProjectId));
                    }).Run();

            }
            catch (Exception exc)
            {
                Dialog_MessageBox.Show(exc.Message, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            return projectName;
        }

    }
}
