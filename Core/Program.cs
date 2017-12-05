using DaSoft.Riviera.Modulador.Core.Runtime;
using DaSoft.Riviera.Modulador.Core.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaSoft.Riviera.Modulador.Core
{

    public class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                if (App.Riviera == null)
                    App.Riviera = new RivieraApplication();
                WinAppSettings win = new WinAppSettings();
                win.ShowDialog();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }

        }
    }
}
