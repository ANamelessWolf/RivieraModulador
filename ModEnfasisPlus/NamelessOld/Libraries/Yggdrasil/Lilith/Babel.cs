using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessOld.Libraries.Yggdrasil.Lilith
{
    public class Babel : NamelessObject
    {
        #region Properties
        /// <summary>
        /// Change the Application Language
        /// </summary>
        public SupportedLanguage Langle
        {
            get
            {
                return _langle;
            }
            set
            {
                this._langle = value;
                SelectLanguage(this._langle);
            }
        }
        /// <summary>
        /// Returns the current culture
        /// </summary>
        public CultureInfo Culture
        {
            get { return SelectLanguage(this._langle); }
        }
        #endregion
        #region Variables
        /// <summary>
        /// Add a resource to the Babel Manager
        /// </summary>
        public delegate void ResourceTranslator(CultureInfo culture);
        SupportedLanguage _langle;
        #endregion
        #region Constructor
        /// <summary>
        /// Creates a new language manager
        /// </summary>
        /// <param name="langle">The language</param>
        public Babel(SupportedLanguage langle, ResourceTranslator translator)
        {
            this.Langle = langle;
            CultureInfo ci = this.Culture;
            Resources.Notices.Culture = ci;
            Resources.CommonStrings.Culture = ci;
            Resources.Errors.Culture = ci;
            translator(ci);
        }
        #endregion
        #region Help Methods
        /// <summary>
        /// Select the Culture
        /// </summary>
        /// <param name="langle">The selected language</param>
        /// <returns>The current cultural info</returns>
        public static CultureInfo SelectLanguage(SupportedLanguage langle)
        {
            CultureInfo ci;
            if (langle == SupportedLanguage.Spanish)
                ci = new CultureInfo("es");
            else
                ci = new CultureInfo("en");
            return ci;
        }
        #endregion


        public static SupportedLanguage Current 
        {
            get
            {
                String cu = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
                if (cu == "es")
                    return SupportedLanguage.Spanish;
                else
                    return SupportedLanguage.English;
            }
        }
    }
}
