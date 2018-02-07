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

namespace DaSoft.Riviera.OldModulador.UI
{
    /// <summary>
    /// Interaction logic for Dialog_MessageBox.xaml
    /// </summary>
    public partial class Dialog_MessageBox : Window
    {
        /// <summary>
        /// Gets the Result Message for the Result
        /// </summary>
        public MessageBoxResult Result;
        /// <summary>
        /// Show the Message Box Dialog
        /// </summary>
        /// <param name="message">The message for the messagebox</param>
        /// <param name="button">The button for the messagebox</param>
        /// <param name="iconImage">The icon for the messabox</param>
        /// <returns>The message Box Result</returns>
        public static MessageBoxResult Show(String message, MessageBoxButton button, MessageBoxImage iconImage)
        {

            Dialog_MessageBox dialog = new Dialog_MessageBox(message, button, iconImage);
            dialog.ShowDialog();
            MessageBoxResult result = dialog.Result;
            return result;
        }
        /// <summary>
        /// Creates a new Message Box Dialog
        /// </summary>
        /// <param name="message">The message box dialog</param>
        /// <param name="button">The dialog type of buttons</param>
        /// <param name="iconImage">The dialog image information</param>
        public Dialog_MessageBox(String message, MessageBoxButton button, MessageBoxImage iconImage)
        {
            InitializeComponent();
            this.field_Message.Text = message;
            this.Width = System.Windows.SystemParameters.PrimaryScreenWidth;
            this.Height = System.Windows.SystemParameters.PrimaryScreenHeight;
            switch (button)
            {
                case MessageBoxButton.YesNoCancel:
                    this.button_Yes_1.Visibility = System.Windows.Visibility.Visible;
                    this.button_No_1.Visibility = System.Windows.Visibility.Visible;
                    this.button_Cancel_1.Visibility = System.Windows.Visibility.Visible;

                    this.button_Ok.Visibility = System.Windows.Visibility.Hidden;
                    this.button_Cancel.Visibility = System.Windows.Visibility.Hidden;

                    this.button_Yes.Visibility = System.Windows.Visibility.Hidden;
                    this.button_No.Visibility = System.Windows.Visibility.Hidden;

                    break;
                case MessageBoxButton.OK:
                    this.button_Yes_1.Visibility = System.Windows.Visibility.Hidden;
                    this.button_No_1.Visibility = System.Windows.Visibility.Hidden;
                    this.button_Cancel_1.Visibility = System.Windows.Visibility.Hidden;

                    this.button_Ok.Visibility = System.Windows.Visibility.Visible;
                    this.button_Cancel.Visibility = System.Windows.Visibility.Hidden;

                    this.button_Yes.Visibility = System.Windows.Visibility.Hidden;
                    this.button_No.Visibility = System.Windows.Visibility.Hidden;
                    break;
                case MessageBoxButton.OKCancel:
                    this.button_Yes_1.Visibility = System.Windows.Visibility.Hidden;
                    this.button_No_1.Visibility = System.Windows.Visibility.Hidden;
                    this.button_Cancel_1.Visibility = System.Windows.Visibility.Hidden;

                    this.button_Ok.Visibility = System.Windows.Visibility.Visible;
                    this.button_Cancel.Visibility = System.Windows.Visibility.Visible;

                    this.button_Yes.Visibility = System.Windows.Visibility.Hidden;
                    this.button_No.Visibility = System.Windows.Visibility.Hidden;
                    break;
                case MessageBoxButton.YesNo:
                    this.button_Yes_1.Visibility = System.Windows.Visibility.Hidden;
                    this.button_No_1.Visibility = System.Windows.Visibility.Hidden;
                    this.button_Cancel_1.Visibility = System.Windows.Visibility.Hidden;

                    this.button_Ok.Visibility = System.Windows.Visibility.Hidden;
                    this.button_Cancel.Visibility = System.Windows.Visibility.Hidden;

                    this.button_Yes.Visibility = System.Windows.Visibility.Visible;
                    this.button_No.Visibility = System.Windows.Visibility.Visible;
                    break;
            }
            switch (iconImage)
            {
                case MessageBoxImage.Information:
                    this.field_CharImage_Ok.Visibility = System.Windows.Visibility.Visible;
                    break;
                case MessageBoxImage.Error:
                    this.field_CharImage_Error.Visibility = System.Windows.Visibility.Visible;
                    break;
                case MessageBoxImage.Warning:
                    this.field_CharImage_Warning.Visibility = System.Windows.Visibility.Visible;
                    break;
                case MessageBoxImage.Question:
                    this.field_CharImage_Question.Visibility = System.Windows.Visibility.Visible;
                    break;
                default:
                    this.field_CharImage_Other.Visibility = System.Windows.Visibility.Visible;
                    break;
            }

        }
        /// <summary>
        /// Gets the Dialog Action
        /// </summary>
        private void DialogAction_Click(object sender, RoutedEventArgs e)
        {
            if (this.button_Yes_1.Name == (sender as FrameworkElement).Name)
                Result = MessageBoxResult.Yes;
            else if (this.button_No_1.Name == (sender as FrameworkElement).Name)
                Result = MessageBoxResult.No;
            else if (this.button_Cancel_1.Name == (sender as FrameworkElement).Name)
                Result = MessageBoxResult.Cancel;
            else if (this.button_Ok.Name == (sender as FrameworkElement).Name)
                Result = MessageBoxResult.OK;
            else if (this.button_Cancel.Name == (sender as FrameworkElement).Name)
                Result = MessageBoxResult.Cancel;
            else if (this.button_Yes.Name == (sender as FrameworkElement).Name)
                Result = MessageBoxResult.Yes;
            else if (this.button_No.Name == (sender as FrameworkElement).Name)
                Result = MessageBoxResult.No;
            else
                Result = MessageBoxResult.Cancel;
            this.Close();
        }
        /// <summary>
        /// Focus the window
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Focus();
        }
    }

}