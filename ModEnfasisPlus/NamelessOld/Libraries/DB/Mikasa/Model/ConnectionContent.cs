using NamelessOld.Libraries.DB.Mikasa.Resources;
using NamelessOld.Libraries.Yggdrasil.Asuna;
using NamelessOld.Libraries.Yggdrasil.Lilith;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NamelessOld.Libraries.DB.Mikasa.Model
{
    public class ConnectionContent : NamelessObject
    {
        XData Root;
        const String ROOT_NAME = "ConnectionFile";
        const String FIELD_INTERFACE = "ConnectionInterface";
        const String FIELD_USERNAME = "Username";
        const String FIELD_PASSWORD = "Password";
        const String FIELD_TIME_OUT = "TimeOut";
        /// <summary>
        ///The username for the connection.
        /// </summary>
        public string Username { get { return this[FIELD_USERNAME]; } set { this[FIELD_USERNAME] = value; } }
        /// <summary>
        ///The password for the connection.
        /// </summary>
        public string Password { get { return this[FIELD_PASSWORD]; } set { this[FIELD_PASSWORD] = value; } }
        /// <summary>
        ///The connection time out
        /// </summary>
        public int TimeOut { get { int time; if (int.TryParse(this[FIELD_TIME_OUT], out time)) return time; else return 0; } set { this[FIELD_TIME_OUT] = value.ToString(); } }
        /// <summary>
        /// The connection interface
        /// </summary>
        public ConnectionInterface Interface { get { return (ConnectionInterface)int.Parse(this[FIELD_INTERFACE]); } set { this[FIELD_INTERFACE] = ((int)value).ToString(); } }
        /// <summary>
        /// Updates a field property
        /// </summary>
        /// <param name="fieldName">The name of the field property to be updated</param>
        /// <returns>The property value</returns>
        public String this[String fieldName]
        {
            get { if (Root.HasAttribute(fieldName)) return Root[fieldName].ToString(); else return String.Empty; }
            set { if (Root.HasAttribute(fieldName)) Root[fieldName] = value; }
        }

        /// <summary> 
        /// Creates a new connection content
        /// </summary>
        /// <param name="fields">The fields contained on the given connection data</param>
        public ConnectionContent(params KeyValuePair<String, String>[] fields)
        {
            XElement connRoot = new XElement(ROOT_NAME);
            this.Root = new XData(connRoot, null);
            foreach (KeyValuePair<String, String> entry in fields)
                this.Root.Add(entry.Key, entry.Value);
        }
        /// <summary>
        /// Creates the access connection string
        /// </summary>
        /// <returns>The Access Connection String</returns>
        public virtual String GenerateConnectionString()
        {
            return String.Empty;
        }
        /// <summary>
        /// Convert the current connection content to an Xml file
        /// </summary>
        /// <returns>The file as an xml</returns>
        public String CreateXml()
        {
            XDocument document;
            document = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));
            document.Add(this.Root.Data);
            return document.ToString();
        }

        /// <summary>
        /// Print the current connection data content
        /// </summary>
        /// <returns>The connection data content</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(String.Format(Messages.ConnTitle, this.Interface));
            List<String> keys = this.Root.AttributeKeys.ToList();
            keys.Sort();
            foreach (String key in keys)
                if (key != FIELD_INTERFACE)
                    sb.Append(String.Format("\n{0}: {1}", key, this[key]));
            return sb.ToString();
        }

    }

}
