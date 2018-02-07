using DaSoft.Riviera.OldModulador.Model;
using DaSoft.Riviera.OldModulador.Query;
using DaSoft.Riviera.OldModulador.Runtime;
using NamelessOld.Libraries.DB.Mikasa.Model;
using NamelessOld.Libraries.DB.Misa;
using NamelessOld.Libraries.DB.Misa.Model;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using static DaSoft.Riviera.OldModulador.Assets.Strings;
namespace DaSoft.Riviera.OldModulador.UI
{
    /// <summary>
    /// Interaction logic for Ctrl_RivieraLogin.xaml
    /// </summary>
    public partial class Ctrl_RivieraLogin : UserControl
    {
        public event RoutedEventHandler LoginSucced, LoginFail;
        /// <summary>
        /// El id del proyecto activo
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
                if (this.litsOfAppDBCompany.SelectedIndex != -1)
                    selCompany = (RivieraCompany)this.litsOfAppDBCompany.SelectedItem;
                else
                    selCompany = RivieraCompany.None;
                return new UserCredential()
                {
                    Company = selCompany,
                    Password = this.fieldAppDBPassword.Password,
                    ProjectId = this.ProjectId,
                    Username = this.fieldAppDBUsername.Text
                };
            }
            set
            {
                this.fieldAppDBPassword.Password = value.Password;
                this.fieldAppDBUsername.Text = value.Username;
                this.litsOfAppDBCompany.SelectedIndex = this.litsOfAppDBCompany.Items.IndexOf(value.Company);
                this.ProjectId = value.ProjectId;
            }
        }

        public Ctrl_RivieraLogin()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.litsOfAppDBCompany.Items.Clear();
            RivieraCompany[] companies = new RivieraCompany[] { RivieraCompany.Arquimart, RivieraCompany.Permasa, RivieraCompany.Riviera, RivieraCompany.Sidesa, RivieraCompany.Vialdi };
            foreach (RivieraCompany cp in companies)
                this.litsOfAppDBCompany.Items.Add(cp);
            this.Credentials = App.Riviera.Credentials;
            if (this.Credentials.ProjectId != -1)
                this.fieldProject.Text = this.Credentials.ProjectId.GetProjectName(this.Credentials);
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            this.areaProgress.Visibility = Visibility.Visible;
            OracleTask task = new OracleTask();
            task.TaskIsFinished += TaskIsFinished;
            Oracle_Transaction<Object, Object> tr = new Oracle_Transaction<Object, Object>(App.Riviera.Connection, LoginTask);
            tr.RunBGWorker(task, this.Credentials, this.fieldProject.Text);
        }
        private object LoginTask(Oracle_Connector conn, ref BackgroundWorker bgWorker, Object trParameter)
        {
            Object[] data = trParameter as Object[];
            UserCredential credentials = (UserCredential)data[0];
            String projectname = data[1] as String;
            Query_Ejecutivo q = new Query_Ejecutivo();
            string id = conn.SelectOne(q.SelectUserID(credentials));
            string projectId;
            if (id != String.Empty && id == credentials.Username)
                conn.LoadProjectId(id, projectname, out projectId);
            else
                projectId = String.Empty;
            if (id == String.Empty)
                throw new Exception(ERR_INVALID_USER_PASS);
            else
                return new Object[] { id != String.Empty && id == credentials.Username, projectId };
        }
        private void TaskIsFinished(object sender, RunWorkerCompletedEventArgs e)
        {
            this.areaProgress.Visibility = Visibility.Collapsed;
            if (e.Result is Exception)
                LoginFail(this, new ConnectionArgs() { Message = ERR_BAD_LOGIN, Error = (e.Result as Exception).Message });
            else if (e.Result is Object[])
            {
                Object[] result = e.Result as Object[];
                if ((Boolean)result[0])
                {
                    //App.Riviera.ActiveProject =result[1];
                    this.ProjectId = int.Parse(result[1].ToString());
                    if (LoginSucced != null)
                        LoginSucced(this, new ConnectionArgs() { Message = String.Format(MSG_SESS_INIT, this.Credentials.Username, this.fieldProject.Text), Error = String.Empty });
                }
                else if (LoginFail != null)
                    LoginFail(this, new ConnectionArgs() { Message = ERR_BAD_LOGIN, Error = ERR_UNKNOWN });
            }
        }
    }
}
