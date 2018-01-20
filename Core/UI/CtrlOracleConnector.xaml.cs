using DaSoft.Riviera.Modulador.Core.Controller;
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
    /// Interaction logic for CtrlOracleConnector.xaml
    /// </summary>
    public partial class CtrlOracleConnector : UserControl
    {
        /// <summary>
        /// Gets or sets the connection method.
        /// </summary>
        /// <value>
        /// The connection method.
        /// </value>
        public ConnectionType ConnectionMethod
        {
            get
            {
                return (ConnectionType)GetValue(ConnectionMethodProperty);
            }
            set
            {
                SetValue(ConnectionMethodProperty, value);
            }
        }
        /// <summary>
        /// Gets the connector.
        /// </summary>
        /// <value>
        /// The connector.
        /// </value>
        public IOracleUIConnector Connector
        {
            get
            {
                ConnectionType connTp = (ConnectionType)this.cboConnType.SelectedIndex;
                if (connTp == ConnectionType.Service_Name)
                    return this.connServiceName as IOracleUIConnector;
                else if (connTp == ConnectionType.SID)
                    return this.connSID as IOracleUIConnector;
                else if (connTp == ConnectionType.TNS)
                    return this.connTNS as IOracleUIConnector;
                else
                    return null;
            }
        }
        /// <summary>
        /// The connection method property
        /// </summary>
        public static DependencyProperty ConnectionMethodProperty;
        /// <summary>
        /// Static constructor
        /// </summary>
        static CtrlOracleConnector()
        {
            ConnectionMethodProperty = DependencyProperty.Register("ConnectionMethod", typeof(ConnectionType), typeof(CtrlOracleConnector),
                new FrameworkPropertyMetadata(ConnectionType.None, FrameworkPropertyMetadataOptions.AffectsRender, ConnectionType_Changed));
        }
        /// <summary>
        /// Connections the type changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void ConnectionType_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            CtrlOracleConnector ctr = sender as CtrlOracleConnector;
            ConnectionType res = (ConnectionType)e.NewValue;
            if (res == ConnectionType.Service_Name)
            {
                ctr.connServiceName.Visibility = Visibility.Visible;
                ctr.connSID.Visibility = Visibility.Hidden;
                ctr.connTNS.Visibility = Visibility.Hidden;
            }
            else if (res == ConnectionType.SID)
            {
                ctr.connServiceName.Visibility = Visibility.Hidden;
                ctr.connSID.Visibility = Visibility.Visible;
                ctr.connTNS.Visibility = Visibility.Hidden;
            }
            else if (res == ConnectionType.TNS)
            {
                ctr.connServiceName.Visibility = Visibility.Hidden;
                ctr.connSID.Visibility = Visibility.Hidden;
                ctr.connTNS.Visibility = Visibility.Visible;
            }
            else
            {
                ctr.connServiceName.Visibility = Visibility.Hidden;
                ctr.connSID.Visibility = Visibility.Hidden;
                ctr.connTNS.Visibility = Visibility.Hidden;
            }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="CtrlOracleConnector"/> class.
        /// </summary>
        public CtrlOracleConnector()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Handles the Loaded event of the UserControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.cboConnType.ItemsSource = Enum.GetValues(typeof(ConnectionType)).OfType<ConnectionType>().Where(x => x != ConnectionType.None).Select(x => x.GetName());
            if (App.Riviera != null)
                this.cboConnType.SelectedIndex = (int)App.Riviera.OracleConnection.ConnectionMethod;
            else
                this.cboConnType.SelectedIndex = 0;
            this.cboConnType.SelectionChanged += CboConnType_SelectionChanged;
            this.ConnectionMethod = (ConnectionType)this.cboConnType.SelectedIndex;
            this.Connector?.LoadConnection();
        }
        /// <summary>
        /// Handles the SelectionChanged event of the CboConnType control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void CboConnType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.ConnectionMethod = (ConnectionType)this.cboConnType.SelectedIndex;
            this.Connector?.LoadConnection();
        }
        /// <summary>
        /// Handles the Click event of the TestConnection control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void TestConnection_Click(object sender, RoutedEventArgs e)
        {
            this.Connector.Connect();
        }
    }
}
