using MahApps.Metro.Controls;
using System;

namespace DaSoft.Riviera.OldModulador.UI
{
    /// <summary>
    /// Interaction logic for Dialog_Loader.xaml
    /// </summary>
    public partial class Dialog_Loader : MetroWindow
    {
        public bool AllowClose;
        public Dialog_Loader(String msg)
        {
            InitializeComponent();
            this.fieldMsg.Text = msg;
            this.Update(0);
        }
        public void Update(int val)
        {
            this.AllowClose = false;
            this.percent.Text = String.Format("{0:P0}", (double)val / 100d);
            this.progressBar.Value = val;
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!this.AllowClose)
                e.Cancel = true;
        }
    }
}
