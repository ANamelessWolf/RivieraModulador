using NamelessOld.Libraries.Yggdrasil.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcadExc = Autodesk.AutoCAD.Runtime.Exception;
namespace NamelessOld.Libraries.HoukagoTeaTime.Runtime
{
    /// <summary>
    /// Romio Exception are found in the Tsumugi Api DB Manager, will take care of any database error.
    /// Romio(Romeo) was the character played by Ritsu in the school play.
    /// </summary>
    public class RomioException : NamelessException
    {
        /// <summary>
        /// Creates a new RomioException (Mio AutoCAD API) Exception
        /// </summary>
        /// <param name="msg">Specify the text for the current exception</param>
        public RomioException(string msg)
            : base(msg) { }
        /// <summary>
        /// Creates a new RomioException (Mio AutoCAD API) Exception
        /// </summary>
        /// <param name="msg">Specify the text for the current exception</param>
        /// <param name="innerException">Specify the Inner Exception for this exception</param>
        public RomioException(string msg, System.Exception innerException)
            : base(msg, innerException) { }
        /// <summary>
        /// Creates a new RomioException (Mio AutoCAD API) Exception
        /// </summary>
        /// <param name="msg">Specify the text for the current exception</param>
        /// <param name="exc">Specify the AutoCAD exception</param>
        public RomioException(string msg, AcadExc exc)
            : base(msg, exc) { }
    }
}
