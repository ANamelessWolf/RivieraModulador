using System;

namespace DaSoft.Riviera.OldModulador.Query
{
    public class Query_Asociados
    {
        #region Constants
        const string TABLENAME_ORACLE = "DTRK_Dir_Asociados";
        const string TABLENAME_ACCESS = "[DIRECTORIO DE ASOCIADOS]";
        const string FIELD_MASK = "MASCARA";
        const string FIELD_ITEM = "AGREGAR";
        const string FIELD_COUNT = "CANTIDAD";
        #endregion


        public static string FormatAsociadosTable
        {
            get
            {
                return "SELECT " +
                          String.Format("Replace([{0}],\"{1}\",\"{2}\") as [MASK], ", FIELD_MASK, "*", "%") +
                          "[AGREGAR], " +
                          "[CANTIDAD] " +
                          "FROM " + TABLENAME_ACCESS;
            }
        }

        /// <summary>
        /// Gets the query for selecting a list of asociados
        /// </summary>
        /// <param name="key">The asociados key</param>
        /// <returns>The selection query</returns>
        internal static string SelectAsociadosAccess(string key)
        {
            return "SELECT" + " * " +
                    "FROM" + " (" + FormatAsociadosTable + ") " +
                    "WHERE" + " " +
                    "'" + key + "' like " + "[MASK]";
        }
        /// <summary>
        /// Gets the query for selecting a list of asociados
        /// </summary>
        /// <param name="key">The asociados key</param>
        /// <returns>The selection query</returns>
        internal static string SelectAsociadosOracle(string key)
        {
            return "SELECT" + " * " +
                    "FROM" + " " + TABLENAME_ACCESS + " " +
                    "WHERE" + " " +
                            FIELD_MASK + " = '" + key + "' ";
        }
    }
}
