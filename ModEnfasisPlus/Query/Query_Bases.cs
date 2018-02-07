using DaSoft.Riviera.OldModulador.Model;
using DaSoft.Riviera.OldModulador.Runtime.DaNTe;
using Oracle.DataAccess.Client;
using System;
using System.Data.Common;

namespace DaSoft.Riviera.OldModulador.Query
{
    public class Query_Bases
    {
        #region Constants
        const string TABLENAME = "BASES";
        const string TABLENAME_MODULO = "BASES_MODULOS";
        const string FIELD_REVIT_ID = "ID_REVIT";
        const string FIELD_USER_ID = "ID_USER";
        const string FIELD_ZONE = "AREA";
        const string FIELD_CLAVE = "CLAVE";
        const string FIELD_MODULE = "MODULO";
        const string FIELD_COUNT = "CANTIDAD";
        const string FIELD_COMPANY = "CIA";
        const string FIELD_HANDLE = "HAND";
        #endregion
        /// <summary>
        /// Creates a command to create a zone
        /// </summary>
        /// <param name="projectID">The id of the project</param>
        /// <param name="zoneName">The name of the zone</param>
        /// <returns>The insertion Command</returns>
        public OracleCommand CreateZone(string projectID, string zoneName)
        {
            OracleCommand cmd = new OracleCommand();
            string[] fields = new string[] { ":" + FIELD_REVIT_ID, ":" + FIELD_ZONE, };
            string[] values = new string[] { projectID, zoneName };
            string text = "INSERT INTO " + TABLENAME + " " +
                "( " +
                     FIELD_REVIT_ID + ", " +
                     FIELD_ZONE + ", " +
                     FIELD_CLAVE + " " +
                " ) " +
                "VALUES " +
                "( " +
                    ":" + FIELD_REVIT_ID + ", " +
                    ":" + FIELD_ZONE + ", " +
                    ":" + FIELD_ZONE + " " +
                " ) ";
            cmd.CommandText = text;
            for (int i = 0; i < fields.Length; i++)
                cmd.Parameters.Add(new OracleParameter(fields[i], values[i]));
            return cmd;
        }

        /// <summary>
        /// Gets the query for selecting a projectId
        /// </summary>
        /// <param name="projectID">The id of the project</param>
        /// <param name="zoneName">The name of the zone</param>
        /// <param name="clave">The current clave</param>
        /// <returns>The Selection Query</returns>
        public String SelectProjectId(String projectID, String zoneName, string clave)
        {
            return "SELECT " + FIELD_REVIT_ID + " " +
                          "FROM " + TABLENAME + " " +
                          "WHERE" + " " +
                                   FIELD_REVIT_ID + " = " + projectID + " " +
                                   "AND " +
                                   FIELD_CLAVE + " = '" + clave + "' " +
                                   "AND " +
                                   FIELD_ZONE + " = '" + zoneName + "' ";
        }


        public OracleCommand SetQuantification(UserCredential cred, string zonename, string clave, string isModule, int cantidad)
        {
            OracleCommand cmd = new OracleCommand();
            string[] fields = new string[] { ":" + FIELD_CLAVE, ":" + FIELD_MODULE, ":" + FIELD_COUNT, ":" + FIELD_USER_ID, ":" + FIELD_COMPANY };
            string[] values = new string[] { clave, isModule, cantidad.ToString(), cred.Username, ((int)cred.Company).ToString() };
            string text = "UPDATE " + TABLENAME + " " +
                                "SET " +
                                     FIELD_CLAVE + " = " + ":" + FIELD_CLAVE + ", " +
                                     FIELD_MODULE + " = " + ":" + FIELD_MODULE + ", " +
                                     FIELD_COUNT + " = " + ":" + FIELD_COUNT + ", " +
                                     FIELD_USER_ID + " = " + ":" + FIELD_USER_ID + " " +
                                     FIELD_COMPANY + " = " + ":" + FIELD_COMPANY + " " +
                                "WHERE" + " " +
                                        FIELD_REVIT_ID + " = " + cred.ProjectId + " " +
                                        "AND " +
                                        FIELD_ZONE + " = '" + zonename + "' ";
            cmd.CommandText = text;
            for (int i = 0; i < fields.Length; i++)
                cmd.Parameters.Add(new OracleParameter(fields[i], values[i]));
            return cmd;
        }
        public OracleCommand InsertQuantification(UserCredential cred, string zonename, string clave, string isModule, int cantidad)
        {
            OracleCommand cmd = new OracleCommand();
            string[] fields = new string[] { ":" + FIELD_REVIT_ID, ":" + FIELD_ZONE, ":" + FIELD_CLAVE, ":" + FIELD_MODULE, ":" + FIELD_COUNT, ":" + FIELD_USER_ID, ":" + FIELD_COMPANY };
            string[] values = new string[] { cred.ProjectId.ToString(), zonename, clave, isModule, cantidad.ToString(), cred.Username, ((int)cred.Company).ToString() };
            string text = "INSERT INTO " + TABLENAME + " " +
                "( " +
                     FIELD_REVIT_ID + ", " +
                     FIELD_ZONE + ", " +
                     FIELD_CLAVE + ", " +
                     FIELD_MODULE + ", " +
                     FIELD_COUNT + ", " +
                     FIELD_USER_ID + ", " +
                     FIELD_COMPANY + " " +
                " ) " +
                "VALUES " +
                "( " +
                    ":" + FIELD_REVIT_ID + ", " +
                    ":" + FIELD_ZONE + ", " +
                    ":" + FIELD_CLAVE + ", " +
                    ":" + FIELD_MODULE + ", " +
                    ":" + FIELD_COUNT + ", " +
                    ":" + FIELD_USER_ID + ", " +
                    ":" + FIELD_COMPANY + " " +
                " ) ";
            cmd.CommandText = text;
            for (int i = 0; i < fields.Length; i++)
                cmd.Parameters.Add(new OracleParameter(fields[i], values[i]));
            return cmd;
        }
        public OracleCommand InsertDaNTeRow(UserCredential cred, string zonename, DaNTeRow row)
        {
            OracleCommand cmd = new OracleCommand();
            string[] fields = new string[] { ":" + FIELD_REVIT_ID, ":" + FIELD_ZONE, ":" + FIELD_CLAVE, ":" + FIELD_COUNT, ":" + FIELD_USER_ID, ":" + FIELD_COMPANY, ":" + FIELD_HANDLE };
            string[] values = new string[] { cred.ProjectId.ToString(), zonename, row.Clave, row.Value.ToString(), cred.Username, ((int)cred.Company).ToString(), row.Handle.ToString() };
            string text = "INSERT INTO " + TABLENAME + " " +
                "( " +
                     FIELD_REVIT_ID + ", " +
                     FIELD_ZONE + ", " +
                     FIELD_CLAVE + ", " +
                     FIELD_COUNT + ", " +
                     FIELD_USER_ID + ", " +
                     FIELD_COMPANY + ", " +
                     FIELD_HANDLE + " " +
                " ) " +
                "VALUES " +
                "( " +
                    ":" + FIELD_REVIT_ID + ", " +
                    ":" + FIELD_ZONE + ", " +
                    ":" + FIELD_CLAVE + ", " +
                    ":" + FIELD_COUNT + ", " +
                    ":" + FIELD_USER_ID + ", " +
                    ":" + FIELD_COMPANY + ", " +
                    ":" + FIELD_HANDLE + " " +
                " ) ";
            cmd.CommandText = text;
            for (int i = 0; i < fields.Length; i++)
                cmd.Parameters.Add(new OracleParameter(fields[i], values[i]));
            return cmd;
        }

        public string SelectModuleId(string projectId, string module_key, string asociado_key)
        {
            return "SELECT " + FIELD_REVIT_ID + " " +
                          "FROM " + "\"" + TABLENAME_MODULO + " " +
                          "WHERE" + " " +
                                   FIELD_REVIT_ID + " = " + projectId + " " +
                                   "AND " +
                                   FIELD_MODULE + " = '" + module_key + "' " +
                                   "AND " +
                                   FIELD_CLAVE + " = '" + asociado_key + "' ";
        }

        public OracleCommand SetModule(UserCredential cred, string module_key, string asociado_key, int cantidad)
        {
            OracleCommand cmd = new OracleCommand();
            string[] fields = new string[] { ":" + FIELD_REVIT_ID, ":" + FIELD_MODULE, ":" + FIELD_CLAVE, ":" + FIELD_COUNT, ":" + FIELD_USER_ID, ":" + FIELD_COMPANY };
            string[] values = new string[] { cred.ProjectId.ToString(), module_key, asociado_key, cantidad.ToString(), cred.Username, ((int)cred.Company).ToString() };
            string text = "INSERT INTO " + TABLENAME_MODULO + " " +
                "( " +
                     FIELD_REVIT_ID + ", " +
                     FIELD_MODULE + ", " +
                     FIELD_CLAVE + ", " +
                     FIELD_COUNT + ", " +
                     FIELD_USER_ID + ", " +
                     FIELD_COMPANY + " " +
                " ) " +
                "VALUES " +
                "( " +
                    ":" + FIELD_REVIT_ID + ", " +
                    ":" + FIELD_MODULE + ", " +
                    ":" + FIELD_CLAVE + ", " +
                    ":" + FIELD_COUNT + ", " +
                    ":" + FIELD_USER_ID + ", " +
                    ":" + FIELD_COMPANY + " " +
                " ) ";
            cmd.CommandText = text;
            for (int i = 0; i < fields.Length; i++)
                cmd.Parameters.Add(new OracleParameter(fields[i], values[i]));
            return cmd;
        }





        public String CleanBases(string projectId, string zonename)
        {
            return "DELETE "
                    + "\"" + TABLENAME + " " +
                    "WHERE" + " " +
                        FIELD_REVIT_ID + " = " + projectId + " " +
                        "AND " +
                        FIELD_ZONE + " = '" + zonename + "' " +
                        "AND " +
                        FIELD_CLAVE + " != '" + zonename + "' ";
        }

        public DbCommand InsertAsociadoRow(UserCredential cred, string zonename, AsociadoRow row, string handle)
        {
            OracleCommand cmd = new OracleCommand();
            string[] fields = new string[] { ":" + FIELD_REVIT_ID, ":" + FIELD_ZONE, ":" + FIELD_CLAVE, ":" + FIELD_COUNT, ":" + FIELD_USER_ID, ":" + FIELD_COMPANY, ":" + FIELD_HANDLE };
            string[] values = new string[] { cred.ProjectId.ToString(), zonename, row.ItemKey, row.Count.ToString(), cred.Username, ((int)cred.Company).ToString(), handle.ToString() };
            string text = "INSERT INTO " + TABLENAME + " " +
                "( " +
                     FIELD_REVIT_ID + ", " +
                     FIELD_ZONE + ", " +
                     FIELD_CLAVE + ", " +
                     FIELD_COUNT + ", " +
                     FIELD_USER_ID + ", " +
                     FIELD_COMPANY + ", " +
                     FIELD_HANDLE + " " +
                " ) " +
                "VALUES " +
                "( " +
                    ":" + FIELD_REVIT_ID + ", " +
                    ":" + FIELD_ZONE + ", " +
                    ":" + FIELD_CLAVE + ", " +
                    ":" + FIELD_COUNT + ", " +
                    ":" + FIELD_USER_ID + ", " +
                    ":" + FIELD_COMPANY + ", " +
                    ":" + FIELD_HANDLE + " " +
                " ) ";
            cmd.CommandText = text;
            for (int i = 0; i < fields.Length; i++)
                cmd.Parameters.Add(new OracleParameter(fields[i], values[i]));
            return cmd;
        }
    }
}
