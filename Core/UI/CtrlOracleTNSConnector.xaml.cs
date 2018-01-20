using DaSoft.Riviera.Modulador.Core.Controller.UI;
using DaSoft.Riviera.Modulador.Core.Runtime;
using Nameless.Libraries.DB.Misa.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DaSoft.Riviera.Modulador.Core.UI
{
    /// <summary>
    /// Lógica de interacción para CtrlOracleTNSConnector.xaml
    /// </summary>
    public partial class CtrlOracleTNSConnector : UserControl, IOracleUIConnector
    {
        /// <summary>
        /// Gets the user controler sender.
        /// </summary>
        /// <value>
        /// The sender.
        /// </value>
        public UserControl Sender => this;
        /// <summary>
        /// Initialize a new instance of the SID connector
        /// </summary>
        public CtrlOracleTNSConnector()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Gets the connection data.
        /// </summary>
        /// <returns>
        /// The connection data
        /// </returns>
        public OracleConnectionData GetConnection()
        {
            int port;
            OracleConnectionBuilder builder = new OracleConnectionBuilder()
            {
                ConnType = ConnectionType.Service_Name,
                Password = this.passOracle.Password,
                Username = this.tboUser.Text,
                Tns = this.tboTNS.Text,
                Host = this.tboHost.Text,
                PersistSecurity = false,
                Port = int.TryParse(this.tboPort.Text, out port) ? port : 1521,
                TimeOut = 10
            };
            return builder.GetConnectionData();
        }
        /// <summary>
        /// Loads the connection data in to the user interface.
        /// </summary>
        public void LoadConnection()
        {
            if (App.Riviera?.OracleConnection != null && this.Visibility == Visibility.Visible)
            {
                var conn = App.Riviera.OracleConnection;
                this.tboHost.Text = conn.Host;
                this.tboPort.Text = conn.Port.ToString();
                this.tboTNS.Text = conn.TNS;
                this.tboUser.Text = conn.Username;
                this.passOracle.Password = conn.Password;
            }
        }
        /// <summary>
        /// Handles the Loaded event of the UserControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.LoadConnection();
        }
    }
}