using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessOld.Libraries.Yggdrasil.Alice
{
    public interface IPupa
    {
        /// <summary>
        /// Gets the encryption key
        /// </summary>
        byte[] Key { get; }
        /// <summary>
        /// Gets the encryption vector
        /// </summary>
        byte[] IV { get; }
        /// <summary>
        /// Gets or sets Encryption Key String
        /// </summary>
        String KeyString { get; set; }
        /// <summary>
        /// Gets or sets Encryption Vector String
        /// </summary>
        String IVString { get; set; }
    }
}
