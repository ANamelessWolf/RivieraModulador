using NamelessOld.Libraries.Yggdrasil.Exceptions;
using NamelessOld.Libraries.Yggdrasil.Lain;
using NamelessOld.Libraries.Yggdrasil.Lilith;
using NamelessOld.Libraries.Yggdrasil.Resources;
using NamelessOld.Libraries.Yggdrasil.Yuffie;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessOld.Libraries.Yggdrasil.Medaka
{
    /// <summary>
    /// An abstract class dedicated to create a more
    /// useful configuration file. Kaishin is the Alter God Mode of Medaka Kurokami
    /// </summary>
    public abstract class KaishinConfiguration : NamelessObject, IMedakaApplication
    {
        #region Constants
        const string CONFIG_FILE = "configOld.medaka";
        const string SP_CAPTION = "KAISHIN";
        const string APP_VERSION = "Version";
        const string LAST_ACCESS_DATE = "LastAccess";
        #endregion
        #region Properties
        #region Aplication Settings
        /// <summary>
        /// The name of the current application
        /// </summary>
        public String ApplicationName { get { return app_Name; } set { app_Name = value; } }
        /// <summary>
        /// Application Directory
        /// </summary>
        public DirectoryInfo AppDirectory { get { return new DirectoryInfo(Path.GetDirectoryName(System.Reflection.Assembly.GetAssembly(typeof(KaishinConfiguration)).Location)); } }
        /// <summary>
        /// Configuration File
        /// </summary>
        public FileInfo ConfigurationFile { get { return new FileInfo(Path.Combine(AppDirectory.FullName, CONFIG_FILE)); } }
        /// <summary>
        /// Gets or sets the value of a category
        /// </summary>
        /// <param name="category">The name of the category</param>
        /// <param name="prop">The name of the property</param>
        /// <returns>The category value as String</returns>
        public String this[String category, String prop]
        {
            get
            {
                try
                {
                    return this.Configuration[category][prop];
                }
                catch (Exception exc)
                {
                    throw new GodModeException(String.Format(Errors.ConfigDoesNotExist, prop, category), exc);
                }
            }
            set
            {
                try
                {
                    this.Configuration[category][prop] = value;
                    this.Save();
                }
                catch (Exception exc)
                {
                    throw new GodModeException(String.Format(Errors.ConfigDoesNotExist, prop, category), exc);
                }
            }
        }        
        /// <summary>
        /// Gets a defined category
        /// </summary>
        /// <param name="category">The name of the category</param>
        /// <param name="prop">The name of the property</param>
        /// <returns>The category value as String</returns>
        public ConfigCategory this[String category]
        {
            get
            {
                try
                {
                    return this.Configuration[category];
                }
                catch (Exception exc)
                {
                    throw new GodModeException(String.Format(Errors.ConfigCategoryDoesNotExist, category), exc);
                }
            }
        }
        /// <summary>
        /// Application Version
        /// </summary>
        public string AppVersion
        {
            get
            {
                return this.Configuration.App_Node.GetAttribute(APP_VERSION);
            }
            set
            {
                this.Configuration.App_Node.SetAttribute(APP_VERSION, value);
                this.Configuration.Xml.Save();
            }
        }
        /// <summary>
        /// Last Access application date
        /// </summary>
        public DateTime Last_Access_Date
        {
            get
            {
                String dtStr = this.Configuration.App_Node.GetAttribute(LAST_ACCESS_DATE);
                DateTime dt = DateTime.ParseExact(dtStr, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                return dt;
            }
            set
            {
                this.Configuration.App_Node.SetAttribute(LAST_ACCESS_DATE, String.Format("{0}/{1}/{2}", String.Format("{0:00}", value.Day), String.Format("{0:00}", value.Month), value.Year));
                this.Configuration.Xml.Save();
            }
        }
        /// <summary>
        /// Save the current xml
        /// </summary>
        public void Save()
        {
            this.Configuration.Xml.Save();
        }
        /// <summary>
        /// The configuration data
        /// </summary>
        public ConfigMedaka Configuration;
        /// <summary>
        /// The log
        /// </summary>
        public WiredGhost Log;
        /// <summary>
        /// Gets the name of the Log
        /// </summary>
        String Log_Name { get { return this.appN + ".log"; } }
        #endregion
        #endregion
        #region Variables
        String appN, app_Name;                            //Aplication Name
        #endregion
        #region Constructor
        /// <summary>
        /// Creates a new Kaishing configuration manager
        /// </summary>
        /// <param name="appName">The name of the application</param>
        /// <param name="logIsEnabled">True if the log is enabled</param>
        public KaishinConfiguration(String appName, Boolean logIsEnabled = false)
        {
            this.appN = appName.ToXmlName();
            this.app_Name = this.appN;
            FileInfo logFile = new FileInfo(Path.Combine(this.AppDirectory.FullName, this.Log_Name));
            this.Log = new WiredGhost(logFile, logIsEnabled);
            this.Log.GhostMode = logIsEnabled;
            try
            {
                this.Configuration = InitializeConfigurationFile();
                this.Configuration.Xml.Save();
            }
            catch (Exception exc)
            {
                throw new GodModeException(exc.Message, exc);
            }
        }
        #endregion
        #region Actions


        /// <summary>
        /// This method is excecuted when the configuration file is created.
        /// </summary>
        /// <param name="conf">The medaka configuration object</param>
        protected abstract void InitConfiguration(ref ConfigMedaka conf);
        /// <summary>
        /// This method is excecuted if the configuration file has been already created
        /// </summary>
        /// <param name="conf">The medaka configuration object</param>
        protected abstract void UpdateConfiguration(ref ConfigMedaka conf);
        #endregion
        #region Help Methods
        /// <summary>
        /// Initialize the configuration file
        /// </summary>
        /// <returns>The configuration file initialized</returns>
        private ConfigMedaka InitializeConfigurationFile()
        {
            this.Configuration = new ConfigMedaka(this.ConfigurationFile, this.appN);
            if (Configuration.Count == 0)
                InitConfiguration(ref this.Configuration);
            else
                UpdateConfiguration(ref this.Configuration);
            this.InitAppAttributes();
            return Configuration;
        }
        /// <summary>
        /// Initialize the application attributes
        /// </summary>
        private void InitAppAttributes()
        {
            if (!this.Configuration.App_Node.HasAttribute(APP_VERSION))
                this.Configuration.App_Node.Add(APP_VERSION, LilithConstants.DEFAULT_VERSION);
            if (!this.Configuration.App_Node.HasAttribute(LAST_ACCESS_DATE))
                this.Configuration.App_Node.Add(LAST_ACCESS_DATE, LilithConstants.DEFAULT_DATE);

        }
        #endregion
    }
}
