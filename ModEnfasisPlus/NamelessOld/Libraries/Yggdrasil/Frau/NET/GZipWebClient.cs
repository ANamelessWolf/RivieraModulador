using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NamelessOld.Libraries.Yggdrasil.Frau.NET
{
    /// <summary>
    /// Create a Gzip web client
    /// </summary>
    public class GZipWebClient : WebClient
    {
        /// <summary>
        /// Override the gzip web client
        /// </summary>
        /// <param name="url">The url web request</param>
        /// <returns>The web request</returns>
        protected override WebRequest GetWebRequest(Uri url)
        {
            HttpWebRequest request = (HttpWebRequest)base.GetWebRequest(url);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            return request;
        }
    }
}
