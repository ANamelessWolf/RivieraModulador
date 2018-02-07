using DaSoft.Riviera.OldModulador.Model.Delta;
using DaSoft.Riviera.OldModulador.Model.Dev;
using DaSoft.Riviera.OldModulador.Runtime;
using NamelessOld.Libraries.HoukagoTeaTime.Yui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DaSoft.Riviera.OldModulador.UI.Delta
{
    /// <summary>
    /// Interaction logic for Ctrl_Developer.xaml
    /// </summary>
    public partial class Ctrl_Developer : UserControl
    {
        public Ctrl_Developer()
        {
            InitializeComponent();
        }

        public long Handle
        {
            get
            {
                return long.Parse(this.selHandle.Text);
            }
            set
            {
                this.selHandle.Text = value.ToString();
            }
        }

        public String Code
        {
            get
            {
                return this.selCode.Text;
            }
            set
            {
                this.selCode.Text = value;
            }
        }

        public PointerDirection Direction;

        public void Fill(List<SnoopRowItem> items)
        {
            snoopData.ItemsSource = items.Where(x => x.IsXRecord).ToList();
            snoopGeometry.ItemsSource = items.Where(x => x.IsCoordinate).ToList();
        }

        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void InspectElement_Click(object sender, RoutedEventArgs e)
        {
            Selector.InvokeCMD(CMD.DEV_SNOOP_DATA);
        }

        public void FindElement_Click(object sender, RoutedEventArgs e)
        {
            String action = (sender as Button).Content.ToString();
            String code = this.Code.Substring(0, 3);

            if (action == "Parent")
                this.Direction = PointerDirection.Parent;
            else if (action == "Same")
                this.Direction = PointerDirection.Same;
            else if (action == "Front")
                this.Direction = PointerDirection.Front;
            else if (action == "Back")
                this.Direction = PointerDirection.Back;
            else if (action == "Left")
                this.Direction = PointerDirection.Left;
            else if (action == "Right")
                this.Direction = PointerDirection.Right;
            else if (action == "DoubleFront")
                this.Direction = PointerDirection.DoubleFront;
            else if (action == "DoubleBack")
                this.Direction = PointerDirection.DoubleBack;
            else if (action == "DoubleLeft")
                this.Direction = PointerDirection.DoubleLeft;
            else if (action == "DoubleRight")
                this.Direction = PointerDirection.DoubleRight;
            else
                this.Direction = PointerDirection.None;
            if (this.Direction != PointerDirection.None)
                Selector.InvokeCMD(CMD.DEV_FIND_ELEMENT);
        }
    }
}
