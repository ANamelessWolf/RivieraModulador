using DaSoft.Riviera.OldModulador.Runtime.DaNTe;
using NamelessOld.Libraries.DB.Tessa.Model;
using System;

namespace DaSoft.Riviera.OldModulador.Query
{
    public class Query_BasesMDB : Access_Query
    {
        /// <summary>
        /// The name of the main table
        /// COMENTARIOS DE LAS BASES
        /// </summary>
        public override string TableName { get { return "COMENTARIOS DE LAS BASES"; } }
        /// <summary>
        /// The name of the DaNTe Table
        /// </summary>
        public string TableName_DaNTeBase { get { return "CUANTIFICACION BASE"; } }

        const string FIELD_BASE = "BASE";
        const string FIELD_AREA = "AREA";
        const string FIELD_CLAVE = "CLAVE";
        const string FIELD_HAND = "HAND";
        const string FIELD_VALUE = "CANTIDAD";
        const string FIELD_MODULE = "MODULO";
        const string FIELD_COMENTARIO = "COMENTARIO";
        const string FIELD_ENFASIS_QUAN_TYPE = "ATSXN19";
        /// <summary>
        /// Gets the query for select all elementes
        /// </summary>
        /// <returns>The selection query</returns>
        public string SelectAll()
        {
            return "SELECT " +
                                FIELD_BASE + ", " +
                                FIELD_COMENTARIO + " " +
                    "FROM" + " " + TableName + ";";
        }
        /// <summary>
        /// Get the query for counting the elements that match a key
        /// </summary>
        /// <param name="tableName">The table name</param>
        /// <param name="key">The selected key</param>
        /// <returns>The count elements</returns>
        public string CountClave(string tableName, string key)
        {
            return "SELECT count(" + FIELD_CLAVE + ") " +
                        "FROM [" + tableName + "] " +
                        "WHERE " +
                            FIELD_CLAVE + " = '" + key + "'";
        }
        /// <summary>
        /// El query de selección de clave cantidad
        /// </summary>
        /// <param name="tableName">El nombre de la tabla</param>
        /// <returns>The count elements</returns>
        public string SelectClaveCantidad(string tableName)
        {
            return "SELECT " +
                        FIELD_CLAVE + ", " +
                        FIELD_VALUE + " " +
                        "FROM [" + tableName + "]";
        }
        /// <summary>
        /// Get the query for creating the comment table
        /// </summary>
        /// <returns>The comment table</returns>
        public string CreateTable()
        {
            return "CREATE TABLE" + " " +
                                 "[" + TableName + "] (" + " " +
                                                          FIELD_BASE + " Varchar(50)," + " " +
                                                          FIELD_AREA + " Varchar(255)," + " " +
                                                          FIELD_COMENTARIO + " Varchar(255)" + " " +
                                                        ")";
        }
        /// <summary>
        /// Get the query for creating a new DaNTe Table
        /// </summary>
        /// <param name="tableName">The Table name</param>
        /// <returns>The creation query</returns>
        public String CreateDaNTeTable(string tableName)
        {
            return "SELECT * INTO [" + tableName + "] FROM [" + TableName_DaNTeBase + "]";
        }
        /// <summary>
        /// Get the query for clear a new DaNTe Table
        /// </summary>
        /// <param name="tableName">The Table name</param>
        /// <returns>The deletion query</returns>
        public String ClearDaNTeTable(string tableName)
        {
            return "DELETE FROM [" + tableName + "] ";
        }
        /// <summary>
        /// Get the query for clear a new DaNTe Table
        /// </summary>
        /// <param name="tableName">The Table name</param>
        /// <returns>The deletion query</returns>
        public String ClearCurrentDaNTeTable(string tableName)
        {
            return "DELETE FROM [" + tableName + "] " +
                  "WHERE " + FIELD_ENFASIS_QUAN_TYPE + " = 777";
        }
        /// <summary>
        /// Gets the query for updating the comment table
        /// </summary>
        /// <param name="tableName">The name of the table</param>
        /// <param name="comments">The new comment</param>
        /// <returns>The update query</returns>
        public String UpdateComments(string tableName, string comments)
        {
            return "UPDATE " + "[" + tableName + "]" +
                            "SET " +
                                FIELD_COMENTARIO + " = '" + FormatValue(comments) + "' " +
                            "WHERE " + FIELD_BASE + " = '" + tableName + "'";
        }
        /// <summary>
        /// Gets the query for inserting a new comment
        /// </summary>
        /// <param name="tableName">The name of the table</param>
        /// <param name="comments">The new comment</param>
        /// <returns>The Insertion query</returns>
        public String InsertComment(string tableName, string comments, String zone)
        {
            return "INSERT INTO " + "[" + TableName + "]" +
                                    "( " +
                                        FIELD_BASE + ", " +
                                        FIELD_COMENTARIO + ", " +
                                        FIELD_AREA + " " +
                                    " )" +
                                "VALUES " +
                                    "( " +
                                        "'" + tableName + "', " +
                                        "'" + FormatValue(comments) + "', " +
                                        "'" + FormatValue(zone) + "' " +
                                    " )";
        }
        /// <summary>
        /// Gets the query for inserting a new comment
        /// </summary>
        /// <param name="tableName">The name of the table</param>
        /// <param name="comments">The new comment</param>
        /// <returns>The Insertion query</returns>
        public String InsertComment(string tableName, string comments)
        {
            return "INSERT INTO " + "[" + TableName + "]" +
                                    "( " +
                                        FIELD_BASE + ", " +
                                        FIELD_COMENTARIO + " " +
                                    " )" +
                                "VALUES " +
                                    "( " +
                                        "'" + tableName + "', " +
                                        "'" + FormatValue(comments) + "' " +
                                    " )";
        }
        /// <summary>
        ///  Gets the query for inserting a new row in the selected Quantification
        /// </summary>
        /// <param name="tableName">The name of the table</param>
        /// <param name="row">The row to be inserted</param>
        /// <param name="zone">The element zone</param>
        /// <returns>The Insertion query</returns>
        public String InsertDaNTeRow(string tableName, DaNTeRow row, String zone)
        {
            return "INSERT INTO " + "[" + tableName + "]" +
                                    "( " +
                                        FIELD_CLAVE + ", " +
                                        FIELD_VALUE + ", " +
                                        FIELD_HAND + ", " +
                                        FIELD_AREA + ", " +
                                        FIELD_ENFASIS_QUAN_TYPE + " " +
                                    " )" +
                                "VALUES " +
                                    "( " +
                                        "'" + row.Clave + "', " +
                                            +row.Value + ", " +
                                        "'" + row.Handle + "', " +
                                        "'" + zone + "', " +
                                        "777" +
                                    " )";
        }
        /// <summary>
        ///  Gets the query for inserting a new row in the selected Quantification
        /// </summary>
        /// <param name="tableName">The name of the table</param>
        /// <param name="row">The row to be inserted</param>
        /// <param name="handle">El handle del código de los elementos asociados</param>
        /// <param name="zone">The element zone</param>
        /// <returns>The Insertion query</returns>
        public String InsertAsociadoRow(string tableName, AsociadoRow row, string handle)
        {
            return "INSERT INTO " + "[" + tableName + "]" +
                                    "( " +
                                        FIELD_CLAVE + ", " +
                                        FIELD_VALUE + ", " +
                                        FIELD_HAND + " " +
                                    " )" +
                                "VALUES " +
                                    "( " +
                                        "'" + row.ItemKey + "', " +
                                            +row.Count + ", " +
                                        "'" + handle + "' " +
                                    " )";
        }
        /// <summary>
        ///  Gets the query for inserting a new row in the selected Quantification
        /// </summary>
        /// <param name="tableName">The name of the table</param>
        /// <param name="row">The row to be inserted</param>
        /// <returns>The Insertion query</returns>
        public String InsertDaNTeRow(string tableName, DaNTeRow row)
        {

            return "INSERT INTO " + "[" + tableName + "]" +
                                    "( " +
                                        FIELD_CLAVE + ", " +
                                        FIELD_VALUE + ", " +
                                        FIELD_HAND +
                                        (row.IsModule ? ", " + FIELD_MODULE + ", " : ", ") +
                                        FIELD_ENFASIS_QUAN_TYPE + " " +
                                    " )" +
                                "VALUES " +
                                    "( " +
                                        "'" + row.Clave + "', " +
                                            +row.Value + ", " +
                                        "'" + row.Handle + "'" +
                                        (row.IsModule ? ", " + "'S'" + ", " : ", ") +
                                        "777" +
                                    " )";
        }
        /// <summary>
        /// Gets the query to drop the Quantification Table
        /// </summary>
        /// <param name="tableName">The name of the table</param>
        /// <returns>The drop query</returns>
        public String DropQuantificationTable(string tableName)
        {
            return "DROP TABLE " + "[" + tableName + "]";
        }
        /// <summary>
        /// Gets the query to delete a Quantification Table
        /// </summary>
        /// <param name="tableName">The name of the table</param>
        /// <returns>The delete query</returns>
        public String DeleteComments(string tableName)
        {
            return "DELETE FROM [" + TableName + "] " +
                          "WHERE " +
                          FIELD_BASE + " = '" + tableName + "'";
        }
    }
}
