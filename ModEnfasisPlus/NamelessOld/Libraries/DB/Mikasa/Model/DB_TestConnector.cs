using NamelessOld.Libraries.Yggdrasil.Lilith;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessOld.Libraries.DB.Mikasa.Model
{
    public abstract class DB_TestConnector : NamelessObject
    {
        /// <summary>
        /// The connection data
        /// </summary>
        public ConnectionData ConnectionData;
        /// <summary>
        /// The background worker
        /// </summary>
        public BackgroundWorker BgWorker;
        /// <summary>
        /// Connection Test 
        /// </summary>
        public abstract void DoWork_TestConnection(object sender, DoWorkEventArgs e);
        /// <summary>
        /// Run when the connection test has finished
        /// </summary>
        public abstract void RunWorkerCompleted_TestConnection(object sender, RunWorkerCompletedEventArgs e);
        /// <summary>
        /// Create a new DB Test connector
        /// </summary>
        /// <param name="connData">The connection data</param>
        public DB_TestConnector(ConnectionData connData)
        {
            this.ConnectionData = connData;
        }
        /// <summary>
        /// Test the current connection
        /// </summary>
        public abstract void TestConnection();
        /// <summary>
        /// Test the current connection
        /// in background mode
        /// </summary>
        public virtual void TestBgConnection()
        {
            this.BgWorker = new BackgroundWorker();
            this.BgWorker.WorkerSupportsCancellation = false;
            this.BgWorker.DoWork += DoWork_TestConnection;
            this.BgWorker.RunWorkerCompleted += RunWorkerCompleted_TestConnection;
            this.BgWorker.RunWorkerAsync(this.ConnectionData);
        }

    }
}
