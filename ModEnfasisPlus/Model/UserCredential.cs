using System;

namespace DaSoft.Riviera.OldModulador.Model
{
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
        /// Creates a Blank Credential
        /// </summary>
        public static UserCredential Empty
        {
            get
            {
                return new UserCredential()
                {
                    ProjectId = -1,
                    Password = String.Empty,
                    Username = String.Empty,
                    Company = RivieraCompany.Riviera
                };
            }
        }
    }
}
