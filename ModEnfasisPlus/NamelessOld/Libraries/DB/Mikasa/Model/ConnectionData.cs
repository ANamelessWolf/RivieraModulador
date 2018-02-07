using NamelessOld.Libraries.DB.Mikasa.Exceptions;
using NamelessOld.Libraries.DB.Mikasa.Resources;
using NamelessOld.Libraries.Yggdrasil.Alice;
using NamelessOld.Libraries.Yggdrasil.Asuna;
using NamelessOld.Libraries.Yggdrasil.Lilith;
using NamelessOld.Libraries.Yggdrasil.Yuffie;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace NamelessOld.Libraries.DB.Mikasa.Model
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="NamelessOld.Libraries.Yggdrasil.Lilith.NamelessObject" />
    public class ConnectionData : NamelessObject
    {
        const String FIELD_INTERFACE = "ConnectionInterface";
        #region Variables
        byte[] key,                           //The encription key.
               vec;                           //The encription vector.
        public FileInfo Conn_File;                    //The configuration file does not exist
        /// <summary>
        /// Access the connection data content
        /// </summary>
        public ConnectionContent Content;
        /// <summary>
        /// True if the connection file exists
        /// </summary>
        public bool Exist { get { return File.Exists(this.Conn_File.FullName); } }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionData"/> class.
        /// Creates the connection data from a file
        /// Exception may ocurre trying to read a file in a protected directory.
        /// </summary>
        /// <param name="cryptoKey">The key of the encrypt file.</param>
        /// <param name="cryptoVector">The vector of the encrypted file.</param>
        /// <param name="conn_File">The encrypted database configuration file.</param>

        public ConnectionData(byte[] cryptoKey, byte[] cryptoVector, FileInfo conn_File)
        {
            this.key = cryptoKey;
            this.vec = cryptoVector;
            KeyValuePair<String, String>[] connData;
            Caterpillar cat = new Caterpillar(this.key, this.vec);
            ExtractConnData(cat, conn_File, out connData);
            this.Content = new ConnectionContent(connData);
            this.Conn_File = conn_File;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionData"/> class.
        /// Fills the connection data with the connection content and creates the encrypted file
        /// Exception may ocurre trying to read a file in a protected directory.
        /// </summary>
        /// <param name="cryptoKey">The key of the encrypt file.</param>
        /// <param name="cryptoVector">The vector of the encrypted file.</param>
        /// <param name="content">The connection content</param>
        /// <param name="conn_File">The encrypted database configuration file.</param>
        public ConnectionData(byte[] cryptoKey, byte[] cryptoVector, FileInfo conn_File, ConnectionContent content)
        {
            this.key = cryptoKey;
            this.vec = cryptoVector;
            this.Content = content;
            this.Conn_File = conn_File;
            this.CreateConnectionFile();
        }
        /// <summary>
        /// Creates a new encrypted configuration file
        /// </summary>
        public void CreateConnectionFile()
        {
            this.CreateConnectionFile(this.Conn_File.FullName);
        }
        /// <summary>
        /// Creates a new encrypted configuration file
        /// </summary>
        /// <param name="filePath">The path to create the configuration file</param>
        public void CreateConnectionFile(String filePath)
        {
            Caterpillar cat = new Caterpillar(this.key, this.vec);
            String connString = this.Content.CreateXml();
            String[] encryptedString = new String[] { cat.Encrypt(connString) };
            if (!this.Exist)
                File.Create(filePath).Close();
            File.WriteAllLines(filePath, encryptedString);
        }

        /// <summary>
        /// Extracts the connection data.
        /// </summary>
        /// <returns>The connection data sorted by keys</returns>
        /// <exception cref="TitanException">A problem reading the connection file</exception>
        public KeyValuePair<String, String>[] Extract()
        {
            ConnectionInterface connInterface = ConnectionInterface.NotSupported;
            KeyValuePair<String, String>[] connData = new KeyValuePair<String, String>[0];
            Caterpillar cat = new Caterpillar(this.key, this.vec);
            if (File.Exists(this.Conn_File.FullName))
            {
                String[] lines = File.ReadAllLines(this.Conn_File.FullName);
                if (lines.Length == 1)
                {
                    String xml = cat.Decrypt(lines[0]);
                    XDocument doc = XDocument.Parse(xml, LoadOptions.PreserveWhitespace);
                    XData data = new XData(doc.FirstNode as XElement, null);
                    connData = data.Keys;
                    connInterface = (ConnectionInterface)int.Parse(data[FIELD_INTERFACE].ToString());
                }
                return connData;
            }
            else
                throw new TitanException(String.Format(Messages.ConnMising, this.Conn_File.FullName));
        }
        /// <summary>
        /// Loads the encrypted configuration file
        /// </summary>
        /// <param name="cat">The caterpillar cryptographer</param>
        /// <param name="conn_File">The path for the connection file</param>
        /// <param name="data">The connection data</param>
        /// <returns>The connection content</returns>
        public static ConnectionInterface ExtractConnData(Caterpillar cat, FileInfo conn_File, out KeyValuePair<String, String>[] connData)
        {
            ConnectionInterface connInterface = ConnectionInterface.NotSupported;
            connData = new KeyValuePair<String, String>[0];
            if (File.Exists(conn_File.FullName))
            {
                String[] lines = File.ReadAllLines(conn_File.FullName);
                if (lines.Length == 1)
                {
                    String xml = cat.Decrypt(lines[0]);
                    XDocument doc = XDocument.Parse(xml, LoadOptions.PreserveWhitespace);
                    XData data = new XData(doc.FirstNode as XElement, null);
                    connData = data.Keys;
                    connInterface = (ConnectionInterface)int.Parse(data[FIELD_INTERFACE].ToString());
                }
                return connInterface;
            }
            else
                throw new TitanException(String.Format(Messages.ConnMising, conn_File.FullName));
        }
        /// <summary>
        /// Return the connection data string
        /// </summary>
        /// <returns>The connection String</returns>
        public override string ToString()
        {
            return String.Format("Key: {0}, IV: {1}, Exist: {2}", Encoding.ASCII.GetChars(this.key).GetString(), Encoding.ASCII.GetChars(this.vec).GetString(), this.Exist);
        }



    }
}
