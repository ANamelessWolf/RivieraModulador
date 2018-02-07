using NamelessOld.Libraries.DB.Mikasa.Exceptions;
using NamelessOld.Libraries.Yggdrasil.Lilith;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessOld.Libraries.DB.Mikasa.Model
{
    public class QueryOperation<output> : NamelessObject
    {
        /// <summary>
        /// Defines a query action
        /// </summary>
        /// <param name="conn">The database connector</param>
        /// <param name="query">The string query</param>
        /// <returns>The query result</returns>
        public delegate output QueryAction(DB_Connector conn, String query);
        /// <summary>
        /// Defines a query action
        /// </summary>
        /// <param name="conn">The database connector</param>
        /// <param name="query">The string query</param>
        /// <returns>The query result</returns>
        public delegate void CmdAction(DB_Connector conn, DbCommand query);
        /// <summary>
        /// Defines a db action
        /// </summary>
        /// <param name="conn">The database connector</param>
        /// <returns>The query result</returns>
        public delegate output DbAction(DB_Connector conn);
        /// <summary>
        /// Action
        /// </summary>
        QueryAction queryAction;
        CmdAction cmdAction;
        DbAction dbAction;
        /// <summary>
        /// Creates a new query operation
        /// A command query
        /// </summary>
        /// <param name="query">The query operation</param>
        public QueryOperation(CmdAction action)
        {
            this.queryAction = null;
            this.cmdAction = action;
            this.dbAction = null;
        }

        /// <summary>
        /// Creates a new query operation
        /// A string query
        /// </summary>
        /// <param name="query">The query operation</param>
        public QueryOperation(QueryAction action)
        {
            this.queryAction = action;
            this.cmdAction = null;
            this.dbAction = null;
        }
        /// <summary>
        /// Creates a db Action
        /// </summary>
        /// <param name="query">The query operation</param>
        public QueryOperation(DbAction action)
        {
            this.queryAction = null;
            this.cmdAction = null;
            this.dbAction = action;
        }
        /// <summary>
        /// Runs a query
        /// </summary>
        /// <param name="conn">The current database connector</param>
        /// <param name="query">The connection query</param>
        /// <param name="defaultData">The default data in case the query failed</param>
        /// <returns>The query result</returns>
        public output Run(DB_Connector conn, String query, output defaultData)
        {
            output result = defaultData;
            try
            {
                conn.Error = String.Empty;
                result = this.queryAction(conn, query);
                conn.QuerySucced = true;
            }
            catch (TitanException exc)
            {
                conn.QuerySucced = false;
                conn.Error = exc.Message;
            }
            catch (System.Exception exc)
            {
                conn.QuerySucced = false;
                conn.Error = exc.Message;
            }
            return result;
        }

        /// <summary>
        /// Runs a command
        /// </summary>
        /// <param name="conn">The current database connector</param>
        /// <param name="cmd">The database command</param>
        /// <param name="defaultData">The default data in case the query failed</param>
        /// <returns>The query result</returns>
        public void Run(DB_Connector conn, DbCommand cmd)
        {
            try
            {
                conn.Error = String.Empty;
                this.cmdAction(conn, cmd);
                conn.QuerySucced = true;
            }
            catch (TitanException exc)
            {
                conn.QuerySucced = false;
                conn.Error = exc.Message;
            }
            catch (System.Exception exc)
            {
                conn.QuerySucced = false;
                conn.Error = exc.Message;
            }
        }
        /// <summary>
        /// Runs a command
        /// </summary>
        /// <param name="conn">The current database connector</param>
        /// <param name="defaultData">The default data in case the query failed</param>
        /// <returns>The query result</returns>
        public output Run(DB_Connector conn, output defaultData)
        {
            output result = defaultData;
            try
            {
                conn.Error = String.Empty;
                conn.QuerySucced = true;
                result = this.dbAction(conn);
            }
            catch (TitanException exc)
            {
                conn.QuerySucced = false;
                conn.Error = exc.Message;
            }
            catch (System.Exception exc)
            {
                conn.QuerySucced = false;
                conn.Error = exc.Message;
            }
            return result;
        }


    }
}
