using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace NamelessOld.Libraries.Yggdrasil.Lilith
{
    public abstract class UI_Controller : NamelessObject
    {
        /// <summary>
        /// Access the defined controls
        /// </summary>
        public FrameworkElement[] Controls;
        /// <summary>
        /// Clear the content for the text boxes and the password boxes
        /// </summary>
        public void ClearInput()
        {
            foreach (FrameworkElement el in this.Controls)
            {
                if (el is TextBox)
                    (el as TextBox).Text = String.Empty;
                else if (el is PasswordBox)
                    (el as PasswordBox).Password = String.Empty;
            }
        }

        /// <summary>
        /// Used to define the controls
        /// </summary>
        /// <param name="elements">The defined controls</param>
        public abstract void DefineControls(params FrameworkElement[] element);
        /// <summary>
        /// Used to translate the controls, to the current language
        /// </summary>
        public abstract void Translate();
        /// <summary>
        /// Used to loads the default data of the control
        /// </summary>
        public abstract void LoadDefaults(params Object[] input);
        /// <summary>
        /// Used to initialize the UI
        /// </summary>
        /// <param name="elements">The defined controls</param>
        public abstract void InitUI();
        /// <summary>
        /// Create a Tooltip
        /// </summary>
        /// <param name="title">The tooltip title</param>
        /// <param name="content">The tooltip content</param>
        public abstract ToolTip CreateToolTip(String title, String content);
    }
}
