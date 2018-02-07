using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessOld.Libraries.Yggdrasil.Lain
{
    /// <summary>
    /// Describes the type of entry that's going to be added in the log file,
    /// </summary>
    public enum Protocol
    {
        /// <summary>
        /// The entry is about a successful action
        /// </summary>
        Ok = 1,
        /// <summary>
        /// The entry is about a successful action, with some errors or warnings.
        /// </summary>
        Warning = 2,
        /// <summary>
        /// The entry is about an error during some action.
        /// </summary>
        Error = 3,
        /// <summary>
        /// The entry is about a Test action.
        /// </summary>
        Test = 4,
        /// <summary>
        /// The entry was just adding.
        /// </summary>
        Add = 5,

    }
}
