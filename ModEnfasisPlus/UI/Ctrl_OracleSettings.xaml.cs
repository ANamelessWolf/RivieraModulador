using DaSoft.Riviera.OldModulador.Model;
using DaSoft.Riviera.OldModulador.Runtime;
using NamelessOld.Libraries.DB.Mikasa.Model;
using NamelessOld.Libraries.DB.Misa;
using NamelessOld.Libraries.DB.Misa.Model;
using NamelessOld.Libraries.Yggdrasil.Alice;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using static DaSoft.Riviera.OldModulador.Assets.Strings;
namespace DaSoft.Riviera.OldModulador.UI
{
    /// <summary>
    /// Interaction logic for Ctrl_OracleSettings.xaml
    /// </summary>
    public partial class Ctrl_OracleSettings : UserControl, IConnectable
    {
        public event RoutedEventHandler ConnectionSucced, ConnectionFail;

        /// <summary>
        /// Application connection type
        /// </summary>
        public Connection_Type ConnectionType
        {
            get
            {
                return (Connection_Type)GetValue(ConnectionTypeProperty);
            }
            set
            {
                SetValue(ConnectionTypeProperty, value);
            }
        }
        /// <summary>
        /// Access the application key
        /// </summary>
        public byte[] Key
        {
            get
            {
                return App.Riviera.Key;
            }
        }
        /// <summary>
        /// Access the application Vector
        /// </summary>
        public byte[] IV
        {
            get
            {
                return App.Riviera.IV;
            }
        }
        /// <summary>
        /// Access the connection file
        /// </summary>
        public FileInfo ConnectionFile
        {
            get
            {
                return App.Riviera.ConnectionFile;
            }
        }
        /// <summary>
        /// Access the current connection data
        /// </summary>
        public OracleConnectionBuilder ConnectionBuilder
        {
            get
            {
                int port;
                return new OracleConnectionBuilder()
                {
                    ConnectionType = this.ConnectionType,
                    Host = this.ConnectionType == Connection_Type.Service_Name ? this.fieldHostServicename.Text :
                                 this.ConnectionType == Connection_Type.SID ? this.fieldHostSID.Text : String.Empty,
                    Password = this.fieldOraclePassword.Password,
                    PersistSecurity = false,
                    Port = int.TryParse(this.fieldOraclePort.Text, out port) ? port : 1521,
                    Servicename = this.fieldServicename.Text,
                    SID = this.fieldSID.Text,
                    TimeOut = 10,
                    TNS = this.fieldTNS.Text,
                    Username = this.fieldOracleUsername.Text
                };
            }
        }
        /// <summary>
        /// Creates a new connection data
        /// </summary>
        ConnectionData Connection
        {
            get
            {
                return new ConnectionData(this.Key, this.IV, this.ConnectionFile, new RivieraConnectionContent(ConnectionBuilder));
            }
        }
        /// <summary>
        /// Connection type static property
        /// </summary>
        public static DependencyProperty ConnectionTypeProperty;
        /// <summary>
        /// Static constructor
        /// </summary>
        static Ctrl_OracleSettings()
        {
            ConnectionTypeProperty = DependencyProperty.Register("ConnectionTypeValue", typeof(Connection_Type), typeof(Ctrl_OracleSettings),
                new FrameworkPropertyMetadata(Connection_Type.None, FrameworkPropertyMetadataOptions.AffectsRender, ConnectionType_Changed));
        }
        /// <summary>
        /// Updates the connection view
        /// </summary>
        private static void ConnectionType_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Ctrl_OracleSettings ctr = sender as Ctrl_OracleSettings;
            Connection_Type res = (Connection_Type)e.NewValue;
            if (res == Connection_Type.Service_Name)
            {
                ctr.sectionSID.Visibility = Visibility.Hidden;
                ctr.sectionServicename.Visibility = Visibility.Visible;
                ctr.sectionTNS.Visibility = Visibility.Hidden;
            }
            else if (res == Connection_Type.SID)
            {
                ctr.sectionSID.Visibility = Visibility.Visible;
                ctr.sectionServicename.Visibility = Visibility.Hidden;
                ctr.sectionTNS.Visibility = Visibility.Hidden;
            }
            else if (res == Connection_Type.TNS)
            {
                ctr.sectionSID.Visibility = Visibility.Hidden;
                ctr.sectionServicename.Visibility = Visibility.Hidden;
                ctr.sectionTNS.Visibility = Visibility.Visible;
            }
            else
            {
                ctr.sectionSID.Visibility = Visibility.Hidden;
                ctr.sectionServicename.Visibility = Visibility.Hidden;
                ctr.sectionTNS.Visibility = Visibility.Hidden;
            }
        }
        /// <summary>
        /// Creates a new oracle connection settings
        /// </summary>
        public Ctrl_OracleSettings()
        {
            InitializeComponent();
        }

        private void connType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ConnectionType = (Connection_Type)this.fieldConnType.SelectedIndex;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            String[] opts = new String[]
            {
                 "SID",
                 "Nombre del Servicio",
                 "TNS"
            };
            this.fieldConnType.Items.Clear();
            foreach (String item in opts)
                this.fieldConnType.Items.Add(item);
            this.fieldConnType.SelectedIndex = LoadData();
        }
        /// <summary>
        /// Load the default or current data
        /// </summary>
        /// <returns>The loaded data</returns>
        private int LoadData()
        {
            int index = 0;
            if (File.Exists(this.ConnectionFile.FullName))
            {
                KeyValuePair<String, String>[] fileData;
                ConnectionData.ExtractConnData(new Caterpillar(this.Key, this.IV), this.ConnectionFile, out fileData);
                OracleConnectionContent content = new OracleConnectionContent(fileData);
                this.fieldOracleUsername.Text = content.Username;
                this.fieldOraclePassword.Password = content.Password;
                this.fieldOraclePort.Text = content.Port.ToString();
                this.fieldSID.Text = content.SID;
                this.fieldHostSID.Text = content.Server;
                this.fieldHostServicename.Text = content.Server;
                this.fieldServicename.Text = content.Service_Name;
                this.fieldTNS.Text = content.TNS;
                index = (int)content.ConnectionType;
            }
            else
            {
                OracleConnectionContent data = RivieraConnectionContent.Remote_Riviera_SID;
                this.fieldOracleUsername.Text = data.Username;
                this.fieldOraclePassword.Password = data.Password;
                this.fieldOraclePort.Text = data.Port.ToString();
                this.fieldSID.Text = data.SID;
                this.fieldHostSID.Text = data.Server;
                this.fieldHostServicename.Text = String.Empty;
                this.fieldServicename.Text = String.Empty;
                this.fieldTNS.Text = String.Empty;
            }
            return index;
        }

        private void TestConnection_Click(object sender, RoutedEventArgs e)
        {
            this.areaProgress.Visibility = Visibility.Visible;
            if (this.ValidateFields())
                this.TestConnection();
            else
            {
                Dialog_MessageBox.Show(ERR_ORACLE_CONN_MISSING, MessageBoxButton.OK, MessageBoxImage.Warning);
                this.areaProgress.Visibility = Visibility.Hidden;
            }
        }

        private void TestConnection()
        {
            OracleTask task = new OracleTask();
            task.TaskIsFinished += TaskIsFinished;
            Oracle_Transaction<Object, Object> tr = new Oracle_Transaction<Object, Object>(this.Connection, TesConnectionTask);
            tr.RunBGWorker(task);
        }

        private object TesConnectionTask(Oracle_Connector conn, ref BackgroundWorker bgWorker, Object trParameters)
        {
            return new Object[] { conn.IsConnected };
        }

        private void TaskIsFinished(object sender, RunWorkerCompletedEventArgs e)
        {
            this.areaProgress.Visibility = Visibility.Collapsed;
            Object[] result = e.Result is Exception ? new Object[] { e.Result } : (Object[])e.Result;
            if (result[0] is Exception)
                ConnectionFail(this, new ConnectionArgs() { Message = ERR_CONN, Error = (result[0] as Exception).Message });
            else if (result[0] is Boolean)
            {
                if ((Boolean)result[0])
                {
                    this.Connection.CreateConnectionFile();
                    if (ConnectionSucced != null)
                        ConnectionSucced(this, new ConnectionArgs() { Message = MSG_CONN, Error = String.Empty });
                }
                else if (ConnectionFail != null)
                    ConnectionFail(this, new ConnectionArgs() { Message = ERR_CONN, Error = ERR_UNKNOWN });
            }
        }



        private bool ValidateFields()
        {
            Boolean commonFields =
                this.fieldOracleUsername.Text != String.Empty &&
                this.fieldOraclePassword.Password != String.Empty;
            if (this.ConnectionType == Connection_Type.SID)
                return commonFields && this.fieldSID.Text != String.Empty && this.fieldHostSID.Text != String.Empty;
            else if (this.ConnectionType == Connection_Type.Service_Name)
                return commonFields && this.fieldServicename.Text != String.Empty && this.fieldHostServicename.Text != String.Empty;
            else if (this.ConnectionType == Connection_Type.TNS)
                return commonFields && this.fieldTNS.Text != String.Empty;
            else
                return false;

        }
    }
}
