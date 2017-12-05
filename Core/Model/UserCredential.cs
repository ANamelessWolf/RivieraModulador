using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaSoft.Riviera.Modulador.Core.Model
{
    /// <summary>
    /// Defines the application user
    /// </summary>
    public class UserCredential
    {
        /// <summary>
        /// The name or Id used to log in on the Riviera Database
        /// </summary>
        public String Username;
        /// <summary>
        /// The id of the active project
        /// </summary>
        public int ProjectId;
        /// <summary>
        /// The password to connect to the Riviera Database
        /// </summary>
        public String Password;
        /// <summary>
        /// The Rivera Company
        /// </summary>
        public RivieraCompany Company;
        /// <summary>
        /// Accede al usuario especial
        /// </summary>
        /// <value>
        /// Las credenciales del usuario
        /// </value>
        public static UserCredential RivieraCredentials
        {
            get
            {
                return new UserCredential()
                {
                    Company = RivieraCompany.Riviera,
                    Password = "x",
                    ProjectId = -1,
                    Username = "99"
                };
            }
        }
    }
}
