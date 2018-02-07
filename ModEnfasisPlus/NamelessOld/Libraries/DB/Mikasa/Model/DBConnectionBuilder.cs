using NamelessOld.Libraries.Yggdrasil.Lilith;
using System;
namespace NamelessOld.Libraries.DB.Mikasa.Model
{
    public abstract class DBConnectionBuilder : NamelessObject
    {
        /// <summary>
        ///The username for the connection.
        /// </summary>
        public String Username;
        /// <summary>
        ///The password for the connection.
        /// </summary>
        public String Password;
        /// <summary>
        /// SQLite host name or server address
        /// </summary>
        public String Host;
        /// <summary>
        /// SQLite connection port
        /// </summary>
        public int Port;
        /// <summary>
        ///The connection time out
        /// </summary>
        public int TimeOut;
        /// <summary>
        /// SQLite persist security
        /// </summary>
        public Boolean PersistSecurity;
        /// <summary>
        /// Gets the data into array, used to create an DB Connection content
        /// </summary>
        public abstract String[] Data { get; }
    }
}
