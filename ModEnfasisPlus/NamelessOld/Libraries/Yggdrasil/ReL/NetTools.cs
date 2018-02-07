using NamelessOld.Libraries.Yggdrasil.Exceptions;
using NamelessOld.Libraries.Yggdrasil.Lilith;
using NamelessOld.Libraries.Yggdrasil.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NamelessOld.Libraries.Yggdrasil.ReL
{
    public class NetTools : NamelessObject
    {
        /// <summary>
        /// Create a new net tools
        /// </summary>
        public NetTools()
        {

        }

        /// <summary>
        /// Download an image from an url
        /// </summary>
        /// <param name="url">The url image downloader</param>
        /// <returns>The bytes of the image</returns>
        public byte[] GetImageFromUrl(string url)
        {
            try
            {
                using (var webClient = new WebClient())
                {
                    return webClient.DownloadData(url);
                }
            }
            catch (WebException exc)
            {
                throw new ErgoProxyException(String.Format(Errors.DownloadImage, url), exc);
            }
            catch (Exception)
            {
                throw new ErgoProxyException(String.Format(Errors.DownloadImage, url));
            }

        }



    }
}
