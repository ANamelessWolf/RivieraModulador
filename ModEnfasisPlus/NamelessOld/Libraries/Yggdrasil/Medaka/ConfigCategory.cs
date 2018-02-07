using NamelessOld.Libraries.Yggdrasil.Asuna;
using System;
using System.Xml.Linq;

namespace NamelessOld.Libraries.Yggdrasil.Medaka
{
    public class ConfigCategory : XUnique
    {
        #region ConfigCategory
        const string ATTNAME_VALUE = "value";
        #endregion

        #region Propiedades
        /// <summary>
        /// Access a property by its name
        /// </summary>
        /// <param name="propertyName">The name of the property</param>
        /// <returns>The name of the property</returns>
        public new string this[String propertyName]
        {
            get
            {
                return this[propertyName, ATTNAME_VALUE];
            }
            set
            {
                this[propertyName, ATTNAME_VALUE] = value;
            }
        }
        #endregion

        /// <summary>
        /// Gets the name of the property;
        /// </summary>
        public string Name { get { return nameCat; } }
        string nameCat;
        /// <summary>
        /// Access the configuration Owner
        /// </summary>
        public ConfigMedaka Owner;


        /// <summary>
        /// Creates a new Config Category
        /// </summary>
        /// <param name="propName">The name of the property</param>
        /// <param name="appNode">The application node</param>
        /// <param name="parent">The Config category parent</param>
        public ConfigCategory(XUnique appNode, XData parent, String propName) :
            base(CreateNode(appNode, propName), parent)
        {
            this.nameCat = propName;

        }

        /// <summary>
        /// Adds a new property to the current configuration property
        /// </summary>
        /// <param name="propertyName">The name of the property</param>
        /// <param name="value">The property value</param>
        /// <returns>The recently added property</returns>
        public override void Add(string propertyName, string value)
        {
            base.Add(propertyName);
            base[propertyName].Add(ATTNAME_VALUE, value);
        }
        /// <summary>
        /// Adds a new property to the current configuration property
        /// </summary>
        /// <param name="propertyName">The name of the property</param>
        /// <returns>The recently added property</returns>
        public override XElement Add(string propertyName)
        {
            XElement el = base.Add(propertyName);
            base[propertyName].Add(ATTNAME_VALUE, String.Empty);
            return el;
        }
        /// <summary>
        /// Remove an existant property
        /// </summary>
        /// <param name="propertyName">The name of the property</param>
        public override void Remove(string propertyName)
        {
            base[propertyName].Data.Remove();
        }
        /// <summary>
        /// Helps to create the xml node property
        /// </summary>
        /// <param name="appNode">The application node</param>
        /// <param name="propName">The name of the property</param>
        /// <returns>The property element</returns>
        private static XElement CreateNode(XUnique appNode, string propName)
        {
            XElement catElement;
            if (appNode.Data.Element(propName) == null)
            {
                catElement = new XElement(propName);
                appNode.Data.Add(catElement);
            }
            else
                catElement = appNode.Data.Element(propName);
            return catElement;
        }



        public bool HasKey(string key)
        {
            return this.Data.Element(key) != null;

        }
    }
}
