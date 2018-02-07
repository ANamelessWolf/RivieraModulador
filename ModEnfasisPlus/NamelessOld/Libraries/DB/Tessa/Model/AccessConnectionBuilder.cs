using NamelessOld.Libraries.DB.Mikasa.Model;
using System;

namespace NamelessOld.Libraries.DB.Tessa.Model
{
    public class AccessConnectionBuilder : DBConnectionBuilder
    {
        /// <summary>
        /// Access the Oledb Provider
        /// </summary>
        public AccessProvider OleDbProvider;
        /// <summary>
        /// Access the path of the Access database file.
        /// </summary>
        public String Access_DB_File;
        /// <summary>
        /// Gets the data into array, used to create an Oracle Connection content
        /// </summary>
        public override String[] Data
        {
            get
            {
                var provider = OleDbProvider.Provider;

                return new String[]
                {
                        ((int)ConnectionInterface.Access).ToString(),
                        this.Username != null ? this.Username : String.Empty,
                        this.Password != null ? this.Password : String.Empty,
                        provider != OledbProviders.UndefinedProvider ? ((int)provider).ToString() : ((int)OledbProviders.Microsoft_ACE_OleDb_12).ToString(),
                        this.Access_DB_File != null ? this.Access_DB_File : String.Empty,
                        PersistSecurity.ToString()
                };
            }
        }
    }
}
