using NamelessOld.Libraries.Yggdrasil.Exceptions;
using NamelessOld.Libraries.Yggdrasil.Lilith;
using NamelessOld.Libraries.Yggdrasil.Resources;
using System;
using System.Linq;
using System.Xml.Linq;
using IoFile = System.IO.File;
using IoFileInfo = System.IO.FileInfo;
namespace NamelessOld.Libraries.Yggdrasil.Asuna
{
    ///<summary>
    ///Asuna is Xml Manager, this class makes easy to create an xml file, read it an snoop it.
    ///It's call NPC beacuse is a not supouse to use it alone, an NPC is a not playable character 
    ///in this case a not playable class.
    ///</summary>
    public class XNpc : NamelessObject
    {
        #region Properties
        /// <summary>
        /// The name of the root Node
        /// </summary>
        public String Root;
        /// <summary>
        /// The Xml Document.
        /// </summary>
        public XDocument Document { get { return doc; } }
        /// <summary>
        /// The xml file
        /// </summary>
        public IoFileInfo File { get { return xml_file; } }
        /// <summary>
        /// Get the xml descendants nodes
        /// </summary>
        public XData[] Descendants
        {
            get
            {
                XData[] data = this.Document.Descendants().Select<XElement, XData>(x => new XData(x, null)).ToArray();
                if (data == null)
                    data = new XData[0];
                return data;
            }
        }
        #endregion
        #region Variables
        XDocument doc;
        IoFileInfo xml_file;
        #endregion
        #region Constructor
        /// <summary>
        /// Creates a new xml file 
        /// </summary>
        /// <param name="xmlFile">The xml file</param>
        public XNpc(String rootname, IoFileInfo xmlFile)
        {
            //1: El manejador de XML solo funciona si el archivo existe,
            //No crea el archivo xml
            if (IoFile.Exists(xmlFile.FullName))
            {
                try
                {
                    //2: Guardamos el nombre del archivo.
                    this.xml_file = xmlFile;
                    this.Root = rootname;
                    //3: Revisa si el archivo es nuevo, en caso de ser nuevo
                    //o estar dañado se crea un nuevo nodo de xml
                    this.CheckXml();
                }
                catch (TitaniaException exc)
                {
                    throw new TitaniaException(exc.Message);
                }
            }
            else
                throw new TitaniaException(String.Format(Errors.XmlFileDoesNotExists, xml_file.FullName));
        }
        #endregion
        #region Acciones



        /// <summary>
        /// Saves the xml file
        /// </summary>
        public void Save()
        {
            this.Document.Save(this.File.FullName);
        }
        /// <summary>
        /// Creates a new xml document
        /// </summary>
        public void CreateDocument()
        {
            this.doc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));
            this.doc.Add(new XElement(this.Root));
            this.Save();
        }
        /// <summary>
        /// Print the xml value
        /// </summary>
        /// <returns>The xml string value</returns>
        public override string ToString()
        {
            return this.Document.ToString();
        }
        #endregion
        #region Help Methods
        /// <summary>
        /// Check if the xml file is new or is damaged
        /// In case of damaged or new xml document is created
        /// Default xml options
        /// Version: 1.0 Encoding: UTF-8, StandAlone: yes
        /// </summary>
        /// <returns>True if the file is new</returns>
        private void CheckXml()
        {
            if (!this.TryToLoadXml())
                CreateDocument();
        }


        /// <summary>
        /// Try to loads and xml
        /// </summary>
        /// <returns>True if xml is loaded, False if fails</returns>
        private Boolean TryToLoadXml()
        {
            Boolean flag = true;
            try
            {
                this.doc = XDocument.Load(this.File.FullName);
            }
            catch (Exception)
            {
                flag = false;
            }
            return flag;
        }
        #endregion
    }
}
