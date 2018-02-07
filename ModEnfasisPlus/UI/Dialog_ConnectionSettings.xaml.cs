using DaSoft.Riviera.OldModulador.Runtime;
using MahApps.Metro.Controls;
using NamelessOld.Libraries.DB.Mikasa.Model;
using System;
using System.Windows;

namespace DaSoft.Riviera.OldModulador.UI
{
    /// <summary>
    /// Interaction logic for Dialog_ConnectionSettings.xaml
    /// </summary>
    public partial class Dialog_ConnectionSettings : MetroWindow
    {
        /// <summary>
        /// Creates a new Connection Settings Dialog
        /// </summary>
        /// <param name="profile">The connection setting profile</param>
        /// <param name="userCredentials">The user credentials</param>
        /// <param name="action">The actions</param>
        public Dialog_ConnectionSettings(/*ConnectionProfile profile, UserCredential userCredentials, ConnectionSettingsAction action*/)
        {
            InitializeComponent();
        }

        private void Connection_Succed(object sender, RoutedEventArgs e)
        {
            ConnectionArgs a = e as ConnectionArgs;
            App.Riviera.ConnectionBuilder = (sender as Ctrl_OracleSettings).ConnectionBuilder;
            Dialog_MessageBox.Show(a.Message, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Connection_Fail(object sender, RoutedEventArgs e)
        {
            ConnectionArgs a = e as ConnectionArgs;
            Dialog_MessageBox.Show(String.Format("{0}\n{1}", a.Message, a.Error), MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void Ctrl_RivieraLogin_LoginSucced(object sender, RoutedEventArgs e)
        {
            ConnectionArgs a = e as ConnectionArgs;
            App.Riviera.Credentials = (sender as Ctrl_RivieraLogin).Credentials;
            App.Riviera.Save();
            Dialog_MessageBox.Show(a.Message, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Ctrl_RivieraLogin_LoginFail(object sender, RoutedEventArgs e)
        {
            ConnectionArgs a = e as ConnectionArgs;
            Dialog_MessageBox.Show(a.Message, MessageBoxButton.OK, MessageBoxImage.Error);
        }

    }
}
