using NamelessOld.Libraries.DB.Misa.Model;
using Oracle.DataAccess.Client;
using System;

namespace DaSoft.Riviera.OldModulador.Query
{
    public class Query_Revit_Project : OracleQuery
    {
        #region Constants
        const string TABLENAME = "REVIT_PROYECTO";
        const string FIELD_ID = "ID_REVIT";
        const string FIELD_USERID = "ID_USER";
        const string FIELD_NAME = "ARCHIVO_REVIT";
        const string FIELD_LAST_ACCESS = "FECHA_ULTIMO_ACCESO";
        #endregion

        /// <summary>
        /// Creates a query to selects a project Id
        /// </summary>
        /// <param name="userId">The id of the user selected</param>
        /// <param name="projectName">The name of the project</param>
        /// <returns>The Selection Query</returns>
        public String SelectProjectId(String userId, String projectName)
        {
            return "SELECT " + FIELD_ID + " " +
                          "FROM " + TABLENAME + " " +
                          "WHERE" + " " +
                                   FIELD_USERID + " = " + userId + " " +
                                   "AND " +
                                   FIELD_NAME + " = '" + projectName + "' ";
        }
        /// <summary>
        /// Creates a query to selects a project name
        /// </summary>
        /// <param name="credentials">Las credenciales del usuario</param>
        /// <returns>The Selection Query</returns>
        public string SelectProjectName(String userId, int projectId)
        {
            return "SELECT " + FIELD_NAME + " " +
                          "FROM " + TABLENAME + " " +
                          "WHERE" + " " +
                                   FIELD_USERID + " = " + userId + " " +
                                   "AND " +
                                   FIELD_ID + " = " + projectId.ToString() + " ";
        }
        /// <summary>
        /// Creates a command to create a project
        /// </summary>
        /// <param name="userId">The id of the user selected</param>
        /// <param name="projectName">The name of the project</param>
        /// <param name="dateTime">The creation date</param>
        /// <returns>The Selection Query</returns>
        public OracleCommand CreateProject(string userId, string projectname)
        {
            OracleCommand cmd = new OracleCommand();
            string[] fields = new string[] { ":" + FIELD_USERID, ":" + FIELD_NAME };
            string[] values = new string[] { userId, projectname };
            string text = "INSERT INTO " + TABLENAME + " " +
                "VALUES " +
                "(" +
                    ":" + FIELD_USERID + "," +
                    "NULL," +
                    ":" + FIELD_NAME + "," +
                    "NULL" +
                ") ";
            cmd.CommandText = text;
            for (int i = 0; i < fields.Length; i++)
                cmd.Parameters.Add(new OracleParameter(fields[i], values[i]));
            return cmd;
        }

        /// <summary>
        /// Update the Last Access for a table
        /// </summary>
        /// <param name="projectId">The id of the project</param>
        /// <returns>The Selection Query</returns>
        public OracleCommand UpdateLastAccess(string projectID)
        {
            OracleCommand cmd = new OracleCommand();
            string[] fields = new string[] { ":" + FIELD_LAST_ACCESS };
            string[] values = new string[] { "SYSDATE" };
            string text = "UPDATE " + TABLENAME + " " +
                                 "SET" + " " +
                                 FIELD_LAST_ACCESS + " = " + ":" + FIELD_LAST_ACCESS + " " +
                                 "WHERE " + FIELD_ID + " = " + projectID;
            cmd.CommandText = text;
            for (int i = 0; i < fields.Length; i++)
                cmd.Parameters.Add(new OracleParameter(fields[i], values[i]));
            return cmd;
        }


    }

}
