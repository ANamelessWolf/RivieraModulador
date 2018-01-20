using DaSoft.Riviera.Modulador.Core.Controller.Transactions;
using DaSoft.Riviera.Modulador.Core.Model;
using DaSoft.Riviera.Modulador.Core.Runtime;
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
using static DaSoft.Riviera.Modulador.Core.Controller.UI.DialogUtils;
using static DaSoft.Riviera.Modulador.Core.Assets.Strings;
using DaSoft.Riviera.Modulador.Core.Model.DB;

namespace DaSoft.Riviera.Modulador.Core.UI
{
    /// <summary>
    /// Lógica de interacción para CtrlOracleLogin.xaml
    /// </summary>
    public partial class CtrlOracleLogin : UserControl
    {
        /// <summary>
        /// The project identifier
        /// </summary>
        public int ProjectId;
        /// <summary>
        /// Get the current credentials
        /// </summary>
        /// <returns>The current credentials</returns>
        public UserCredential Credentials
        {
            get
            {
                RivieraCompany selCompany;
                if (this.lvCompanies.SelectedIndex != -1)
                    selCompany = (RivieraCompany)this.lvCompanies.SelectedItem;
                else
                    selCompany = RivieraCompany.None;
                return new UserCredential()
                {
                    Company = selCompany,
                    Password = this.passUser.Password,
                    ProjectId = this.ProjectId,
                    Username = this.tboUser.Text
                };
            }
            set
            {
                this.passUser.Password = value.Password;
                this.tboUser.Text = value.Username;
                this.lvCompanies.SelectedIndex = this.lvCompanies.Items.IndexOf(value.Company);
                this.ProjectId = value.ProjectId;
            }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="CtrlOracleLogin"/> class.
        /// </summary>
        public CtrlOracleLogin()
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
            if (this.lvCompanies.ItemsSource == null)
                this.lvCompanies.ItemsSource = Enum.GetValues(typeof(RivieraCompany)).OfType<RivieraCompany>().Where(x => x != RivieraCompany.None);
            if (App.Riviera.Credentials == null)
                App.Riviera.Credentials = UserCredential.RivieraCredentials;
            this.Credentials = App.Riviera.Credentials;
            if (this.IsEnabled)
                LoadDBData();
        }
        /// <summary>
        /// Realizá la carga de información de la base de datos
        /// </summary>
        public void LoadDBData()
        {
            if (this.lvProjects.ItemsSource == null)
            {
                this.lvProjects.ItemsSource = OracleTransactions.GetProjects().Where(x => !x.ProjectName.ToLower().Contains(".rvt")).OrderBy(x => x.ProjectName);
                if (this.Credentials.ProjectId != -1)
                    try
                    {
                        this.tboProject.Text = OracleTransactions.GetProjectName(this.Credentials);
                    }
                    catch (Exception exc)
                    {
                        this.tboProject.Text = String.Empty;
                        App.Riviera.Log.AppendEntry(exc, null, false);
                    }
            }
        }

        /// <summary>
        /// Handles the Click event of the Login control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            ActiveWindow = this.GetWindow();
            await ShowProgressDialog(String.Format(MSG_LOGGIN_IN, this.Credentials.Username), String.Empty);
            this.Login(this.Credentials);
        }
        /// <summary>
        /// Handles the SelectionChanged event of the lvProjects control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void lvProjects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.lvProjects.SelectedIndex != -1)
                this.tboProject.Text = (this.lvProjects.SelectedItem as RivieraProject).ProjectName;
        }
    }
}
