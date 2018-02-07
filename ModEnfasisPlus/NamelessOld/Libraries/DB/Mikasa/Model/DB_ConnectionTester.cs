using NamelessOld.Libraries.Yggdrasil.Lilith;
using System;
using System.ComponentModel;
using System.Windows;
using static NamelessOld.Libraries.DB.Mikasa.Resources.Messages;
namespace NamelessOld.Libraries.DB.Mikasa.Model
{
    /// <summary>
    /// Creates an database connection test
    /// </summary>
    public abstract class DB_ConnectionTester : NamelessObject
    {
        /// <summary>
        /// This event is run once the connection has finished
        /// And the connection succed
        /// </summary>
        public event RoutedEventHandler ConnectionSucced;
        /// <summary>
        /// This event is run once the connection has falied
        /// And the connection succed
        /// </summary>
        public event RoutedEventHandler ConnectionFailed;

        /// <summary>
        /// Validates if the connection succeded event is defined
        /// </summary>
        public Boolean ConnectionSuccedFlag
        {
            get { return this.ConnectionSucced != null; }
        }
        /// <summary>
        /// Validates if the connection failed event is defined
        /// </summary>
        public Boolean ConnectionFailedFlag
        {
            get { return this.ConnectionFailed != null; }
        }
        /// <summary>
        /// Excecute the connection transaction
        /// </summary>
        /// <param name="data">Database connection data</param>
        public abstract void Connect(ConnectionData data);
        /// <summary>
        /// Excecute the connection transaction
        /// </summary>
        /// <param name="connString">Database connection string</param>
        public abstract void Connect(String connString);
        /// <summary>
        /// Excecute once the task is finished
        /// </summary>
        public void TaskIsFinished(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result is Exception && ConnectionFailedFlag)
                ConnectionFailed(this, new ConnectionArgs() { Message = ERR_CONN, Error = (e.Result as Exception).Message });
            else if (e.Result is Boolean)
            {
                if ((Boolean)e.Result)
                {
                    if (ConnectionSuccedFlag)
                        ConnectionSucced(this, new ConnectionArgs() { Message = MSG_CON, Error = String.Empty });
                }
                else if (ConnectionFailedFlag)
                    ConnectionFailed(this, new ConnectionArgs() { Message = ERR_CONN, Error = ERR_CON_UNKNOWN });
            }
            else
                ConnectionFailed(this, new ConnectionArgs() { Message = ERR_CONN, Error = ERR_CON_UNKNOWN });
        }
    }
}
