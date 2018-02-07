using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessOld.Libraries.DB.Mikasa.Model
{
    public abstract class DB_Connector
    {
        public ConnectionData ConnectionObject;
        /// <summary>
        /// The connection string for the current connection.
        /// </summary>
        public string ConnectionString { get { return _ConnString == null ? ConnectionObject.Content.GenerateConnectionString() : _ConnString; } }
        String _ConnString;
        /// <summary>
        /// Gets the current connection interface
        /// </summary>
        public ConnectionInterface Interface { get { return _Interface == ConnectionInterface.NotSupported ? ConnectionObject.Content.Interface : _Interface; } }
        ConnectionInterface _Interface;
        /// <summary>
        /// Last occurred error
        /// </summary>
        public string Error;
        /// <summary>
        /// The current state of the connection object.
        /// </summary>
        public ConnectionState Status;
        /// <summary>
        /// True if the class is connected to Oracle
        /// </summary>
        public Boolean IsConnected { get { return Status == ConnectionState.Open; } }

        /// <summary>
        /// If the value is true, means the query succed.
        /// </summary>
        public bool QuerySucced;
        /// <summary>
        /// Initialize the connection base
        /// </summary>
        /// <param name="conData">The connection data.</param>
        public DB_Connector(ConnectionData conData)
        {
            this.ConnectionObject = conData;
            this._Interface = ConnectionInterface.NotSupported;
        }
        /// <summary>
        /// Initialize the connection base
        /// </summary>
        /// <param name="connString">The connection string.</param>
        /// <param name="connInterface">The current connection interface</param>
        public DB_Connector(String connString, ConnectionInterface connInterface)
        {
            this._ConnString = connString;
            this._Interface = connInterface;
        }

    /// <summary>
    /// Select one field, and just one item from a query
    /// </summary>
    /// <param name="selectQuery">The selection query</param>
    /// <returns>The selected Item.</returns>
    public abstract string SelectOne(String selectQuery);
    /// <summary>
    /// Select a group of items from a query and returned in a list. 
    /// The selection query must select just from one field only.
    /// A bad query will return nothing(Empty list).
    /// </summary>
    /// <param name="selectQuery">The selection query</param>
    /// <returns>The list of selected items taken by one field</returns>
    public abstract List<String> SelectItems(String selectQuery);
    /// <summary>
    /// Select a group of tables from a query and returned in a list. 
    /// A bad query will return nothing (Empty list).
    /// </summary>
    /// <returns>The list of available tables</returns>
    public abstract List<String> SelectTables();
    /// <summary>
    /// Check if a table exist in the current Schema
    /// </summary>
    /// <param name="tableName">The name of the table to be checked</param>
    /// <returns>True if the table exist.</returns>
    public abstract Boolean TableExist(string tableName);
    /// <summary>
    /// Select just one row from a query and returned in array.
    /// </summary>
    /// <param name="selectQuery">The selection query</param>
    /// <returns>The selected rows.</returns>
    public abstract String[] SelectRow(String selectQuery);
    /// <summary>
    /// Select a group of items from a query and returned in a list, each field is separeted
    /// by an escape character.
    /// </summary>
    /// <param name="selectQuery">The selection query</param>
    /// <param name="rowSeparator">The escape character to define a Cell</param>
    /// <returns>The list of selected rows.</returns>
    public abstract List<String> SelectRows(String selectQuery, char rowSeparator);
    /// <summary>
    /// Execute an insert, update or delete query
    /// </summary>
    /// <param name="query">The update query as dbCommand</param>
    public abstract void Update(String query);
    /// <summary>
    /// Execute an insert, update or delete query
    /// </summary>
    /// <param name="cmd">The db cmd</param>
    public abstract void Update(DbCommand cmd);
    /// <summary>
    /// Execute a procedure an obtains the procedure value
    /// </summary>
    /// <param name="cmd">The db cmd</param>
    /// <returns>The procedure result</returns>
    public abstract List<Object> CallProcedure(DbCommand cmd);

}
}
