using System;
using System.Windows;

namespace NamelessOld.Libraries.DB.Mikasa.Model
{
    public class ConnectionArgs : RoutedEventArgs
    {
        /// <summary>
        /// Connection message
        /// </summary>
        public String Message;
        /// <summary>
        /// Connection error
        /// </summary>
        public String Error;
    }
}
