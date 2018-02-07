using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessOld.Libraries.DB.Mikasa.Model
{
    public class AccessProvider
    {
        const String PROVIDER_JET_4 = "Microsoft.Jet.OLEDB.4.0";
        const String PROVIDER_ACE_4 = "Microsoft.ACE.OLEDB.4.0";
        const String PROVIDER_ACE_12 = "Microsoft.ACE.OLEDB.12.0";
        const String PROVIDER_ACE_14 = "Microsoft.ACE.OLEDB.14.0";
        /// <summary>
        /// Access the Oledb provider
        /// </summary>
        public OledbProviders Provider;
        /// <summary>
        /// Creates a new Access provider
        /// </summary>
        /// <param name="provider">The oledb provider</param>
        public AccessProvider(OledbProviders provider)
        {
            this.Provider = provider;
        }
        /// <summary>
        /// Get Provider String
        /// </summary>
        /// <returns>The string of an especific provider</returns>
        public override string ToString()
        {
            if (Provider == OledbProviders.JetOleDb_4)
                return PROVIDER_JET_4;
            else if (Provider == OledbProviders.Microsoft_ACE_OleDb_4)
                return PROVIDER_ACE_4;
            else if (Provider == OledbProviders.Microsoft_ACE_OleDb_12)
                return PROVIDER_ACE_12;
            else if (Provider == OledbProviders.Microsoft_ACE_OleDb_14)
                return PROVIDER_ACE_14;
            else
                return String.Empty;
        }
    }
}
