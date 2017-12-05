using DaSoft.Riviera.Modulador.Core.Controller.UI;
using DaSoft.Riviera.Modulador.Core.Runtime;
using DaSoft.Riviera.Modulador.Core.UI;
using MahApps.Metro.Controls;
using Nameless.Libraries.DB.Mikasa.Model;
using Nameless.Libraries.DB.Misa;
using Nameless.Libraries.DB.Misa.Model;
using Nameless.Libraries.Yggdrasil.Lain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DaSoft.Riviera.Modulador.Core.Assets.Strings;
using static DaSoft.Riviera.Modulador.Core.Controller.UI.DialogUtils;
namespace DaSoft.Riviera.Modulador.Core.Controller
{
    /// <summary>
    /// Defines the database connection utils
    /// </summary>
    public static partial class DBUtils
    {
        /// <summary>
        /// Gets the remote riviera TNS.
        /// </summary>
        /// <value>
        /// The remote riviera TNS.
        /// </value>
        public static OracleConnectionData RemoteRivieraTNS
        {
            get
            {
                return new OracleConnectionBuilder()
                {
                    ConnType = ConnectionType.TNS,
                    Password = "s1na1",
                    Username = "riviera",
                    Host = "193.3.5.30",
                    Port = 1521,
                    PersistSecurity = true,
                    Tns = "TRAMITES_RIVIERA",
                    TimeOut = 10,
                }.GetConnectionData();
            }
        }
        /// <summary>
        /// Gets the remote riviera servicename.
        /// </summary>
        /// <value>
        /// The remote riviera servicename.
        /// </value>
        public static OracleConnectionData RemoteRivieraServicename
        {
            get
            {
                return new OracleConnectionBuilder()
                {
                    ConnType = ConnectionType.Service_Name,
                    Password = "s1na1",
                    Username = "riviera",
                    Host = "193.3.5.30",
                    Port = 1521,
                    PersistSecurity = true,
                    ServiceName = "tramites.palmas.rivieramex.com",
                    TimeOut = 10,
                }.GetConnectionData();
            }
        }
        /// <summary>
        /// Gets the remote riviera sid.
        /// </summary>
        /// <value>
        /// The remote riviera sid.
        /// </value>
        public static OracleConnectionData RemoteRivieraSID
        {
            get
            {
                return new OracleConnectionBuilder()
                {
                    ConnType = ConnectionType.SID,
                    Password = "s1na1",
                    Username = "riviera",
                    Host = "193.3.5.30",
                    Port = 1521,
                    PersistSecurity = true,
                    SID = "TRAMITES",
                    TimeOut = 10,
                }.GetConnectionData();
            }
        }
        /// <summary>
        /// Gets the local Riviera connection
        /// </summary>
        public static OracleConnectionData LocalRivieraServiceName
        {
            get
            {
                return new OracleConnectionBuilder()
                {
                    ConnType = ConnectionType.Service_Name,
                    Password = "r4cks",
                    Username = "riviera",
                    Host = "10.0.0.3",
                    Port = 1521,
                    PersistSecurity = true,
                    ServiceName = "pdb_orcl",
                    TimeOut = 10,
                }.GetConnectionData();
            }
        }
        /// <summary>
        /// Creates the oracle connection file.
        /// </summary>
        /// <param name="app">The application.</param>
        public static void CreateOracleConnectionFile(this RivieraApplication app, OracleConnectionData data)
        {
            if (!File.Exists(app.OracleConnectionFile.FullName))
                File.Create(app.OracleConnectionFile.FullName).Close();
            data.Save(app.OracleConnectionFile.FullName);
            app.OracleConnection = data;
        }
        /// <summary>
        /// Connects the specified connector.
        /// </summary>
        /// <param name="connector">The connector.</param>
        public static async void Connect(this IOracleUIConnector connector)
        {
            Oracle_Tester tester = new Oracle_Tester();
            tester.Sender = connector;
            ActiveWindow = connector.GetWindow();
            await ShowProgressDialog(MSG_CONNECTING, String.Empty);
            tester.ConnectionFailed += Tester_ConnectionFailed;
            tester.ConnectionSucced += Tester_ConnectionSucced;
            tester.Connect(connector.GetConnection());
        }
        /// <summary>
        /// Handles the ConnectionSucced event of the Tester control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private async static void Tester_ConnectionSucced(object sender, System.Windows.RoutedEventArgs e)
        {
            Oracle_Tester tester = sender as Oracle_Tester;
            IOracleUIConnector uiConnector = tester.Sender as IOracleUIConnector;
            App.Riviera.OracleConnection = uiConnector.GetConnection();
            App.Riviera.OracleConnection.Save(App.Riviera.OracleConnectionFile.FullName);
            await CloseProgressDialog();
            await uiConnector.Sender.ShowDialog(TIT_ORACLE_CONN, MSG_CONN);
            var win = uiConnector.Sender.GetWindow() as WinAppSettings;
            win.loginSection.IsEnabled = true;
            win.loginSection.LoadDBData();
        }
        /// <summary>
        /// Handles the ConnectionFailed event of the Tester control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        /// <exception cref="NotImplementedException"></exception>
        private async static void Tester_ConnectionFailed(object sender, System.Windows.RoutedEventArgs e)
        {
            Oracle_Tester tester = sender as Oracle_Tester;
            IOracleUIConnector uiConnector = tester.Sender as IOracleUIConnector;
            ConnectionArgs args = e as ConnectionArgs;
            String msg = args.Error;
            App.Riviera.Log.AppendEntry(msg, Protocol.Error, "Tester_ConnectionTest", "IOracleUIConnector");
            await CloseProgressDialog();
            await uiConnector.Sender.ShowDialog(TIT_ORACLE_CONN, msg);
        }
        /// <summary>
        /// Gets the name of the oracle connection.
        /// </summary>
        /// <param name="connType">Type of the connection.</param>
        /// <returns>The string name for the connection type</returns>
        public static string GetName(this ConnectionType connType)
        {
            ConnectionType[] val = new ConnectionType[] { ConnectionType.None, ConnectionType.SID, ConnectionType.Service_Name, ConnectionType.TNS };
            String[] opts = new String[] { "Desconocido", "SID", "Nombre del Servicio", "TNS" };
            return opts[val.ToList().IndexOf(connType)];
        }

    }
}
