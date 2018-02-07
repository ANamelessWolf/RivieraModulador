using NamelessOld.Libraries.DB.Mikasa.Exceptions;
using NamelessOld.Libraries.DB.Mikasa.Model;
using NamelessOld.Libraries.DB.Mikasa.Resources;
using NamelessOld.Libraries.DB.Tessa.Exceptions;
using NamelessOld.Libraries.Yggdrasil.Lilith;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;

namespace NamelessOld.Libraries.DB.Tessa.Model
{
    public class Access_Connector : DB_Connector, IDisposable
    {
        /// <summary>
        /// Returns the Access connection content
        /// </summary>
        public AccessConnectionContent Content { get { return this.ConnectionObject.Content as AccessConnectionContent; } }
        /// <summary>
        /// The connection Object
        /// </summary>
        OleDbConnection Connection;
        /// <summary>
        /// Initialize a connection to Access
        /// </summary>
        /// <param name="conData">The connection data.</param>
        public Access_Connector(ConnectionData conData)
            : base(conData)
        {
            try
            {
                base.ConnectionObject.Content = new AccessConnectionContent(conData.Extract());
                this.Connection = new OleDbConnection(base.ConnectionString);
                this.Connection.Open();
                this.Status = this.Connection.State;
            }
            catch (OleDbException exc)
            {
                throw new MithrilException(MikasaConstants.CONNECTION_FAILED, exc);
            }
            catch (TitanException exc)
            {
                throw new MithrilException(MikasaConstants.CONNECTION_FAILED, exc);
            }
            catch (System.Exception exc)
            {
                throw new MithrilException(MikasaConstants.CONNECTION_FAILED, exc);
            }
        }
        /// <summary>
        /// Initialize a connection to Access
        /// </summary>
        /// <param name="connStr">The connection string</param>
        public Access_Connector(String connStr)
            : base(connStr, ConnectionInterface.Access)
        {
            try
            {
                this.Connection = new OleDbConnection(base.ConnectionString);
                this.Connection.Open();
                this.Status = this.Connection.State;
            }
            catch (OleDbException exc)
            {
                throw new MithrilException(MikasaConstants.CONNECTION_FAILED, exc);
            }
            catch (TitanException exc)
            {
                throw new MithrilException(MikasaConstants.CONNECTION_FAILED, exc);
            }
            catch (System.Exception exc)
            {
                throw new MithrilException(MikasaConstants.CONNECTION_FAILED, exc);
            }
        }
        /// <summary>
        /// Select a group of tables from a query and returned in a list. 
        /// A bad query will return nothing (Empty list).
        /// </summary>
        /// <returns>The list of available tables</returns>
        public override List<string> SelectTables()
        {
            QueryOperation<List<string>> op = new QueryOperation<List<string>>
             (delegate (DB_Connector conn)
             {
                 List<string> tableNames = new List<string>();
                 DataTable dt = (conn as Access_Connector).Connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
                 for (int i = 0; i < dt.Rows.Count; ++i)
                     tableNames.Add(dt.Rows[i]["TABLE_NAME"].ToString());
                 return tableNames;
             });
            return op.Run(this, new List<String>());
        }
        /// <summary>
        /// Check if a table exist in the current Schema
        /// </summary>
        /// <param name="tableName">The name of the table to be checked</param>
        /// <returns>True if the table exist.</returns>
        public override Boolean TableExist(string tableName)
        {
            QueryOperation<Boolean> op = new QueryOperation<Boolean>
             (delegate (DB_Connector conn)
             {
                 bool exist = false;
                 DataTable dbSchema = (conn as Access_Connector).Connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, tableName, "TABLE" });
                 // Si existe ponemos la bandera en true.
                 if (dbSchema.Rows.Count > 0)
                     exist = true;
                 return exist;
             });
            return op.Run(this, false);
        }
        /// <summary>
        /// Select one field, and just one item from a query.
        /// A bad query will return nothing(String.Empty).
        /// </summary>
        /// <param name="selectQuery">The selection query</param>
        /// <returns>The selected item as String</returns>
        public override string SelectOne(string selectQuery)
        {
            QueryOperation<string> op = new QueryOperation<string>
                (delegate (DB_Connector conn, String query)
                {
                    OleDbDataAdapter sqlAdapter = new OleDbDataAdapter(query, (conn as Access_Connector).Connection);
                    DataSet ds = new DataSet();
                    sqlAdapter.Fill(ds);
                    return ds.Tables[0].Rows[0].ItemArray[0].ToString();
                });
            return op.Run(this, selectQuery, String.Empty);
        }
        /// <summary>
        /// Select a group of items from a query and returned in a list. 
        /// The selection query selects just from one field of the table.
        /// A bad query will return nothing(Empty list).
        /// </summary>
        /// <param name="selectQuery">The selection query</param>
        /// <returns>The list of selected items taken by one field</returns>
        public override List<string> SelectItems(string selectQuery)
        {
            QueryOperation<List<string>> op = new QueryOperation<List<string>>
                (delegate (DB_Connector conn, String query)
                {
                    List<string> fields = new List<string>();
                    string item;
                    OleDbDataAdapter sqlAdapter = new OleDbDataAdapter(query, (conn as Access_Connector).Connection);
                    DataSet ds = new DataSet();
                    sqlAdapter.Fill(ds);
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        item = String.Empty;
                        for (int j = 0; j < ds.Tables[0].Rows[i].ItemArray.Length; j++)
                            item += ds.Tables[0].Rows[i].ItemArray[j].ToString();
                        fields.Add(item);
                    }
                    return fields;
                });
            return op.Run(this, selectQuery, new List<string>());
        }
        /// <summary>
        /// Select just one row from a query and returned in array,
        /// A bad query will return nothing(Empty array).
        /// </summary>
        /// <param name="selectQuery">The selection query</param>
        /// <returns>The selected rows.</returns>
        public override string[] SelectRow(string selectQuery)
        {
            List<string> strings = SelectRows(selectQuery, LilithConstants.ESCAPECHAR);
            string[] row = new string[] { };
            if (strings.Count > 0)
                row = strings[0].Split(LilithConstants.ESCAPECHAR);
            return row;
        }
        /// <summary>
        /// Select a group of items from a query and returned in a list, each field is separeted
        /// by an escape character.
        /// A bad query will return nothing(Empty list).
        /// </summary>
        /// <param name="selectQuery">The selection query</param>
        /// <param name="rowSeparator">The escape character to define a Cell</param>
        /// <returns>The list of selected rows.</returns>
        public override List<string> SelectRows(string selectQuery, char rowSeparator)
        {
            QueryOperation<List<string>> op = new QueryOperation<List<string>>
                (delegate (DB_Connector conn, String query)
                {
                    List<string> rows = new List<string>();
                    string row;
                    OleDbDataAdapter sqlAdapter = new OleDbDataAdapter(query, (conn as Access_Connector).Connection);
                    DataSet ds = new DataSet();
                    sqlAdapter.Fill(ds);
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        row = String.Empty;
                        for (int j = 0; j < ds.Tables[0].Rows[i].ItemArray.Length; j++)
                        {
                            row += ds.Tables[0].Rows[i].ItemArray[j].ToString();
                            if (j != ds.Tables[0].Rows[i].ItemArray.Length - 1)
                                row += rowSeparator;
                        }
                        rows.Add(row);
                    }
                    return rows;
                });
            return op.Run(this, selectQuery, new List<string>());
        }
        /// <summary>
        /// Execute an insert, update or delete query
        /// </summary>
        /// <param name="query">The update query as dbCommand</param>
        public override void Update(DbCommand cmd)
        {
            QueryOperation<Nothing> op = new QueryOperation<Nothing>
                (delegate (DB_Connector conn, DbCommand queryCmd)
                {
                    queryCmd.Connection = (conn as Access_Connector).Connection;
                    queryCmd.ExecuteNonQuery();
                });
            op.Run(this, cmd);
        }
        /// <summary>
        /// Execute an insert, update or delete query
        /// </summary>
        /// <param name="query">The update query as string</param>
        public override void Update(string query)
        {
            OleDbCommand cmd = new OleDbCommand();
            cmd.CommandText = query;
            this.Update(cmd);
        }
        /// <summary>
        /// Execute a procedure an obtains the procedure value
        /// </summary>
        /// <param name="cmd">The db cmd</param>
        /// <returns>The procedure result</returns>
        public override List<Object> CallProcedure(DbCommand cmd)
        {
            List<Object> result = new List<Object>();
            QueryOperation<List<Object>> op = new QueryOperation<List<Object>>
                (delegate (DB_Connector conn, DbCommand queryCmd)
                {
                    queryCmd.Connection = (conn as Access_Connector).Connection;
                    queryCmd.ExecuteNonQuery();
                });
            op.Run(this, cmd);
            if (cmd.Parameters != null)
                foreach (DbParameter obj in cmd.Parameters)
                    result.Add(obj.Value);
            return result;
        }
        /// <summary>
        /// Dispose the current connection
        /// </summary>
        public void Dispose()
        {
            this.Connection.Close();
            this.Connection.Dispose();
        }


    }
}
