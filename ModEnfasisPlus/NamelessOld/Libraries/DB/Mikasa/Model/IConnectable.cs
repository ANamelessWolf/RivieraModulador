using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessOld.Libraries.DB.Mikasa.Model
{
    public interface IConnectable
    {
        /// <summary>
        /// The connection data encription key
        /// </summary>
        byte[] Key { get; }
        /// <summary>
        /// The connection data encription Vector
        /// </summary>
        byte[] IV { get; }
        /// <summary>
        /// The connection file
        /// </summary>
        FileInfo ConnectionFile { get; }
    }
}
