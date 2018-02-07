using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessOld.Libraries.Yggdrasil.Lilith
{
    public interface ILangle
    {
        /// <summary>
        /// Application Directory
        /// </summary>
        Babel AppLangle { get; set; }
        /// <summary>
        /// Translate Resources
        /// </summary>
        void TranslateResources(CultureInfo ci);
        /// <summary>
        /// Init Langle
        /// </summary>
        void InitLangle();
        /// <summary>
        /// Access the supported language
        /// </summary>
        SupportedLanguage Current_Langle { get; set; }
    }
}
