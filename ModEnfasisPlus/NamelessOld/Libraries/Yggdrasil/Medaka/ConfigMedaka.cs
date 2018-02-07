using NamelessOld.Libraries.Yggdrasil.Asuna;
using NamelessOld.Libraries.Yggdrasil.Lilith;
using NamelessOld.Libraries.Yggdrasil.Yuffie;
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using IoFile = System.IO.File;
using IoFileInfo = System.IO.FileInfo;
using IoFileStream = System.IO.FileStream;
namespace NamelessOld.Libraries.Yggdrasil.Medaka
{
    public class ConfigMedaka : NamelessObject
    {
        /// <summary>
        /// Access the configuration file as an xml
        /// </summary>
        public XNpc Xml;
        /// <summary>
        /// Gets the xml application root node in the configuration file.
        /// </summary>
        public XUnique App_Node { get { return new XUnique(this.Xml.Document.Element(AppName), null); } }
        /// <summary>
        /// The name of the application
        /// </summary>
        public String AppName { get { return aName; } }
        /// <summary>
        /// Access the configuration categories
        /// </summary>
        public Dictionary<String, ConfigCategory> categories;
        /// <summary>
        /// The name of the application
        /// </summary>
        String aName;
        /// <summary>
        /// Access a configuration category by its name
        /// </summary>
        /// <param name="catName">The name of the category</param>
        /// <returns>The configuration category</returns>
        public ConfigCategory this[String catName]
        {
            get
            {
                return categories[catName];
            }
        }
        /// <summary>
        /// The size total number of categories
        /// </summary>
        public int Count
        {
            get { return this.categories.Count; }
        }
        /// <summary>
        /// The configuration is not read only
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }
        /// <summary>
        /// Creates access to a configuration file, a configuration file doesn't allow duplicate
        /// nodes, each configuration category must be unique
        /// </summary>
        /// <param name="config_File">The configuration file, path</param>
        /// <param name="appName">The application name is the name for the root xml element</param>
        public ConfigMedaka(IoFileInfo config_File, string appName)
        {
            this.aName = appName.ToXmlName();
            //1: Checamos la existencia del archivo, en caso de no existir lo creamos
            this.CheckFile(config_File.FullName);
            //2: Generamos el Xml NPC
            this.Xml = new XNpc(this.aName, config_File);
            //3: Inicializamos la colección
            this.categories = new Dictionary<string, ConfigCategory>();
            switch (GetConfigFileStatus())
            {
                case ConfigStatus.Damage:
                    this.Xml.CreateDocument();
                    break;
                default:
                    FillCategory();
                    break;
            }
        }

        /// <summary>
        /// Adds a new category
        /// </summary>
        /// <param name="item">The category to be added</param>
        public void Add(ConfigCategory item)
        {
            this.categories.Add(item.Name, item);

        }
        /// <summary>
        /// Adds a new category
        /// </summary>
        /// <param name="item">The category to be added</param>
        public void Add(String catName)
        {
            ConfigCategory item = new ConfigCategory(this.App_Node, null, catName);
            this.categories.Add(catName, item);
        }
        /// <summary>
        /// Check if the file exist if the file does not exist is created
        /// </summary>
        /// <param name="filePath">The file path</param>
        private void CheckFile(string filePath)
        {
            if (!IoFile.Exists(filePath))
            {
                IoFileStream stream = IoFile.Create(filePath);
                stream.Close();
            }
        }
        /// <summary>
        /// Get the current configuration file status
        /// </summary>
        /// <returns>The current configuration status</returns>
        private ConfigStatus GetConfigFileStatus()
        {
            ConfigStatus conStatus = ConfigStatus.Ok;
            if (this.App_Node.Count == 0)
                conStatus = ConfigStatus.Empty;
            else if (this.App_Node.Count < 1 || (this.App_Node.Count == 1 && this.App_Node.Data.Name != this.AppName))
                conStatus = ConfigStatus.Damage;
            return conStatus;
        }

        /// <summary>
        /// Fills the node category
        /// </summary>
        private void FillCategory()
        {
            ConfigCategory cat;
            foreach (XElement catNode in this.App_Node.Data.Elements())
            {
                cat = new ConfigCategory(this.App_Node, null, catNode.Name.ToString());
                this.Add(cat);
            }
        }
        /// <summary>
        /// Clear the configuration file
        /// </summary>
        public void Clear()
        {
            this.App_Node.Delete();
            this.Xml.CreateDocument();
            this.categories.Clear();
        }
        /// <summary>
        /// Check if a category is already defined
        /// </summary>
        /// <param name="item">The configuration category to be tested</param>
        /// <returns>True if the configuration category is contained</returns>
        public bool Contains(ConfigCategory item)
        {
            return this.categories.ContainsKey(item.Name);
        }


        /// <summary>
        /// Check if a category is already defined
        /// </summary>
        /// <param name="catName">The name of the category</param>
        /// <returns>True if the configuration category is contained</returns>
        public bool Contains(String catName)
        {
            return this.categories.ContainsKey(catName);
        }

        /// <summary>
        /// Returns the Config Medaka Content
        /// </summary>
        /// <returns>The xml file</returns>
        public override string ToString()
        {
            return this.Xml.ToString();
        }
    }
}
