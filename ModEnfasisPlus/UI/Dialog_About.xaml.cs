using DaSoft.Riviera.OldModulador.Runtime;
using System;
using System.Text;
using System.Windows;

namespace DaSoft.Riviera.OldModulador.UI
{
    /// <summary>
    /// Interaction logic for Dialog_About.xaml
    /// </summary>
    public partial class Dialog_About : Window
    {
        public Dialog_About()
        {
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Versión: ");
            sb.Append(App.Riviera.AppVersion);
            sb.Append('\n');
            sb.Append("Fecha de compilación: ");
            sb.Append('\n');
            sb.Append(App.Riviera.Last_Access_Date.ToShortDateString());
            sb.Append('\n');
            sb.Append("Copyright");
            sb.Append('\n');
            sb.Append("DaSoft S.A. de C.V. " + DateTime.Now.Year + "©");
            this.appDetails.Text = sb.ToString();
        }
    }
}
