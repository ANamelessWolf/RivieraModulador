using NamelessOld.Libraries.Yggdrasil.Alice;
using NamelessOld.Libraries.Yggdrasil.Lilith;
using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace NamelessOld.Libraries.Yggdrasil.Medaka
{
    public class NamelessApplication : KaishinConfiguration
    {
        #region Constants
        //Configuration Node Files
        const string CAT_NAMELESS = "Nameless";
        //Nameless properties
        const string PROP_API_VERSION = "API_Version";
        const string PROP_LAST_COMPILED_VERSION = "Compiled";
        const string PROP_DEVELOPER = "Developer";
        const string PROP_COMPANY = "Company";
        #endregion
        #region Properties
        /// <summary>
        /// Gets or sets company
        /// </summary>
        public new String Company { get { return this[CAT_NAMELESS, PROP_COMPANY]; } set { this[CAT_NAMELESS, PROP_COMPANY] = value; } }
        /// <summary>
        /// Gets or sets the developer names
        /// </summary>
        public String Developer { get { return this[CAT_NAMELESS, PROP_DEVELOPER]; } set { this[CAT_NAMELESS, PROP_DEVELOPER] = value; } }
        #endregion
        #region Configuration File Nodes
        //Configuration node categories
        string[] categories = new string[] { CAT_NAMELESS };
        //Configuration node properties
        string[][] properties = new string[][] {  //El primer nodo es el de settings
                                                   new string[]
                                                   {
                                                      PROP_API_VERSION,
                                                      PROP_LAST_COMPILED_VERSION,
                                                      PROP_DEVELOPER, PROP_COMPANY
                                                   }
                                               };
        //The configuration file categories default value
        string[][] propertiesDefaultData = new string[][] { //El primer nodo es el de settings
                                                   new string[]
                                                   {
                                                     Assembly.GetAssembly(typeof(NamelessApplication)).GetName().Version.ToString(3),
                                                     new FileInfo( Assembly.GetAssembly(typeof(NamelessApplication)).Location).LastWriteTime.ToShortDateString(),
                                                     LilithConstants.DEVELOPER,  LilithConstants.COMPANY
                                                   },
                                               };
        #endregion
        #region CryptoKeys
        /// <summary>
        /// Gets or sets the application encryption key
        /// </summary>
        public virtual byte[] Key { get { return _Key; } set { _Key = value; } }
        /// <summary>
        /// Gets or sets the application encryption vector
        /// </summary>
        public virtual byte[] IV { get { return _IV; } set { _IV = value; } }
        byte[] _Key = Encoding.ASCII.GetBytes("N4m3LeSs");               //The application Key
        byte[] _IV = Encoding.ASCII.GetBytes("tYzX78SZ");                //The application Vector
        #endregion
        /// <summary>
        /// Creates a new Kaishing configuration manager
        /// </summary>
        /// <param name="appName">The name of the application</param>
        /// <param name="logIsEnabled">True if the log is enabled</param>
        public NamelessApplication(String appName, Boolean logIsEnabled = false) :
                base(appName, logIsEnabled)
        {
            this[CAT_NAMELESS, PROP_API_VERSION] = Assembly.GetAssembly(typeof(NamelessApplication)).GetName().Version.ToString(3);
            this[CAT_NAMELESS, PROP_LAST_COMPILED_VERSION] = new FileInfo(Assembly.GetAssembly(typeof(NamelessApplication)).Location).LastWriteTime.ToShortDateString();
        }
        #region Actions
        /// <summary>
        /// Encrypt a string
        /// </summary>
        /// <param name="str">The string to encrypt</param>
        /// <returns>The string encrypted</returns>
        public String Encrypt(String str)
        {
            Caterpillar cat = new Caterpillar(this.Key, this.IV);
            return cat.Encrypt(str);
        }
        /// <summary>
        /// Decrypt a string
        /// </summary>
        /// <param name="str">The string to be decrypted</param>
        /// <returns>The string to decrypted</returns>
        public String Decrypt(String str)
        {
            Caterpillar cat = new Caterpillar(this.Key, this.IV);
            return cat.Decrypt(str);
        }
        #endregion
        #region Help Methods
        /// <summary>
        /// Initialize the configuration file
        /// </summary>
        /// <param name="conf">The configuration object</param>
        protected override void InitConfiguration(ref ConfigMedaka conf)
        {
            this.Fill(ref conf, this.categories, this.properties, this.propertiesDefaultData);
        }
        /// <summary>
        /// Updates the configuration file
        /// </summary>
        /// <param name="conf">The configuration object</param>
        protected override void UpdateConfiguration(ref ConfigMedaka conf)
        {
            this.Fill(ref conf, this.categories, this.properties, this.propertiesDefaultData, true);
        }
        /// <summary>
        /// Fill the category data
        /// </summary>
        /// <param name="conf">The current configuration file</param>
        /// <param name="cats">The category list</param>
        /// <param name="props">The property list</param>
        /// <param name="defaultData">The default data</param>
        public void Fill(ref ConfigMedaka conf, String[] cats, String[][] props, String[][] defaultData, Boolean update = false)
        {
            if (update)
            {
                for (int i = 0; i < props.Length; i++)
                {
                    if (!conf.Contains(cats[i]))
                        conf.Add(cats[i]);
                    if (cats.Length <= props.Length)
                        for (int j = 0; j < props[i].Length; j++)
                            if (!conf[cats[i]].HasKey(props[i][j]))
                                conf[cats[i]].Add(props[i][j], defaultData[i][j]);
                }
            }
            else
            {
                for (int i = 0; i < cats.Length; i++)
                {
                    if (!conf.Contains(cats[i]))
                        conf.Add(cats[i]);
                    if (cats.Length <= props.Length)
                        for (int j = 0; j < props[i].Length; j++)
                            conf[cats[i]].Add(props[i][j], defaultData[i][j]);
                }
            }
        }

        #endregion
    }
}
