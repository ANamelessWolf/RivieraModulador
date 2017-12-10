using DaSoft.Riviera.Modulador.Bordeo.Model;
using DaSoft.Riviera.Modulador.Core.Runtime;
using DaSoft.Riviera.Modulador.Core.UI;
using MahApps.Metro.Controls;
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

namespace RivieraUITesting
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //if (DaSoft.Riviera.Modulador.Core.Runtime.App.Riviera == null)
                  //  DaSoft.Riviera.Modulador.Core.Runtime.App.Riviera = new RivieraApplication();
                //WinAppSettings win = new WinAppSettings();
                //win.ShowDialog();
               // DaSoft.Riviera.Modulador.Core.Runtime.App.Riviera.Database.LineDB.Add(DaSoft.Riviera.Modulador.Core.Model.DesignLine.Bordeo, new BordeoDesignDatabase());
                //DaSoft.Riviera.Modulador.Core.Runtime.App.Riviera.Database.Init(this);
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }
        }
    }
}
