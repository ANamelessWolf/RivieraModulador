using Nameless.Libraries.DB.Misa.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DaSoft.Riviera.Modulador.Core.Controller.UI
{
    /// <summary>
    /// Defines an interface that connects to Oracle
    /// </summary>
    public interface IOracleUIConnector
    {
        /// <summary>
        /// Gets the user controler sender.
        /// </summary>
        /// <value>
        /// The sender.
        /// </value>
        UserControl Sender { get; }
        /// <summary>
        /// Gets the connection data.
        /// </summary>
        /// <returns>The connection string</returns>
        OracleConnectionData GetConnection();
        /// <summary>
        /// Loads the connection data in to the user interface.
        /// </summary>
        void LoadConnection();
    }
}
