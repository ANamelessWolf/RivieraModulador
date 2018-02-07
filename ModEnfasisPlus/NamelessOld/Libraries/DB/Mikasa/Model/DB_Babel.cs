using NamelessOld.Libraries.DB.Mikasa.Resources;
using NamelessOld.Libraries.Yggdrasil.Lilith;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessOld.Libraries.DB.Mikasa.Model
{
    public class DB_Babel : ILangle
    {
        /// <summary>
        /// Gets the database language
        /// </summary>
        public Babel AppLangle { get { return this.langle; } set { this.langle = value; } }
        /// <summary>
        /// Gets the supported language
        /// </summary>
        public SupportedLanguage Current_Langle { get { return AppLangle.Langle; } set { AppLangle.Langle = value; } }
        /// <summary>
        /// The Language manager
        /// </summary>
        Babel langle;
        /// <summary>
        /// Access the Database language
        /// </summary>
        /// <param name="lan">The current database language</param>
        public DB_Babel(Babel lan)
        {
            this.langle = lan;
        }
        /// <summary>
        /// Initialize the translated resource
        /// </summary>
        /// <param name="ci">Cultural information</param>
        public void TranslateResources(CultureInfo ci)
        {
            Messages.Culture = ci;
        }
        /// <summary>
        /// Initialize the application language
        /// </summary>
        public void InitLangle()
        {
            
        }


    }
}
