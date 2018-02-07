using NamelessOld.Libraries.Yggdrasil.Lilith;
using NamelessOld.Libraries.Yggdrasil.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessOld.Libraries.Yggdrasil.Exceptions
{
    /// <summary>
    /// BlackMateriaException are the exception found in Aerith (File management). 
    /// Black Materia was the materia used to summon doomsday magic in Final Fantasy VII, 
    /// Aerith with holy stops the meteor.
    /// </summary>
    public class BlackMateriaException : NamelessException
    {
        public static BlackMateriaException FileNotFound
        {
            get { return new BlackMateriaException(Errors.FileNotFound); }
        }

        #region Constructores
        /// <summary>
        /// Creates a new BlackMateriaException (Nameless File Manager) Exception
        /// </summary>
        /// <param name="msg">Specify the text for the current exception</param>
        public BlackMateriaException(string msg)
            : base(msg) { }
        /// <summary>
        /// Creates a new BlackMateriaException (Nameless File Manager) Exception
        /// </summary>
        /// <param name="msg">Specify the text for the current exception</param>
        /// <param name="innerException">Specify the Inner Exception for this exception</param>
        public BlackMateriaException(string msg, System.Exception innerException)
            : base(msg, innerException) { }
        #endregion
    }
}
