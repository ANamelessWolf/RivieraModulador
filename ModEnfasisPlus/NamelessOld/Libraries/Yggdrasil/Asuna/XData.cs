using NamelessOld.Libraries.Yggdrasil.Exceptions;
using NamelessOld.Libraries.Yggdrasil.Lilith;
using NamelessOld.Libraries.Yggdrasil.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace NamelessOld.Libraries.Yggdrasil.Asuna
{
    public class XData : NamelessObject
    {
        /// <summary>
        /// Access the Xml Parent
        /// </summary>
        public XData Parent;
        /// <summary>
        /// True if the element is root
        /// </summary>
        public Boolean IsRoot { get { return this.Parent == null; } }
        /// <summary>
        /// Access the Xml Data
        /// </summary>
        public XElement Data;
        /// <summary>
        /// Get the list of attribute keys
        /// </summary>
        public String[] AttributeKeys
        {
            get
            {
                String[] keys = this.Data.Attributes().Select<XAttribute, String>(x => x.Name.ToString()).ToArray();
                if (keys == null)
                    keys = new String[0];
                return keys;
            }
        }
        /// <summary>
        /// Get the atribute data as key value pair
        /// </summary>
        public KeyValuePair<String, String>[] Keys
        {
            get
            {
                String[] keys = this.Data.Attributes().Select<XAttribute, String>(x => x.Name.ToString()).ToArray();
                if (keys != null)
                {
                    KeyValuePair<String, String>[] key_Values = new KeyValuePair<String, String>[keys.Length];
                    for (int i = 0; i < keys.Length; i++)
                        key_Values[i] = new KeyValuePair<string, string>(keys[i], this[keys[i]] as String);
                    return key_Values;
                }
                else
                    return new KeyValuePair<String, String>[0];
            }
        }
        /// <summary>
        /// Get the list of children nodes
        /// </summary>
        public XData[] Children
        {
            get
            {
                XData[] data = this.Data.Elements().Select<XElement, XData>(x => new XData(x, this)).ToArray();
                if (data == null)
                    data = new XData[0];
                return data;
            }
        }
        /// <summary>
        /// The xml node value
        /// </summary>
        public String Value
        {
            get
            {
                return this.Data.Value;
            }
            set
            {
                this.Data.Value = value;
            }
        }
        /// <summary>
        /// Access the attribute by its name
        /// </summary>
        /// <param name="attName">The name of the attribute</param>
        /// <returns>The attribute value</returns>
        public virtual Object this[string attName] { get { return GetAttribute(attName); } set { SetAttribute(attName, value.ToString()); } }
        /// <summary>
        /// Set the name of the attribute
        /// </summary>
        /// <param name="attName">The name of the attribute to set its value</param>
        /// <param name="value">The attribute value</param>
        public void SetAttribute(string attName, string value)
        {
            if (AttributeKeys.Contains(attName))
                this.Data.Attribute(attName).Value = value;
            else
                throw new TitaniaException(String.Format(Errors.XmlAttNameNotExist, attName, this.Data.Name));
        }
        /// <summary>
        /// Get the attribute by name
        /// </summary>
        /// <param name="attName">The name of the attribute</param>
        public String GetAttribute(string attName)
        {
            if (AttributeKeys.Contains(attName))
                return this.Data.Attribute(attName).Value;
            else
                throw new TitaniaException(String.Format(Errors.XmlAttNameNotExist, attName, this.Data.Name));
        }
        /// <summary>
        /// Access a child by it index
        /// </summary>
        /// <param name="index">The index for the child</param>
        /// <returns>The attribute value</returns>
        public XData this[int index]
        {
            get
            {
                if (Children.Length - 1 <= index)
                    return this.Children[index];
                else
                    throw new TitaniaException(String.Format(Errors.XmlNodeOutOfBounds, index.ToString()));
            }
        }
        /// <summary>
        /// The number of children for this node
        /// </summary>
        public int Count { get { return this.Data.Elements().Count(); } }

        #region Constructor
        /// <summary>
        /// Check if the XData has an specific attribute.
        /// The name is case sensitive
        /// </summary>
        /// <param name="attName">The name of the attribute</param>
        /// <returns>True if the attribute exist</returns>
        public Boolean HasAttribute(String attName)
        {
            return AttributeKeys.Contains(attName);
        }
        /// <summary>
        /// Creates a new XData Object
        /// </summary>
        /// <param name="content">The Xml data</param>
        /// <param name="parent">The xData parent</param>
        public XData(XElement content, XData parent)
        {
            this.Parent = parent;
            this.Data = content;
        }

        /// <summary>
        /// Creates a new XData Object
        /// </summary>
        /// <param name="name">The xElemnent node name</param>
        /// <param name="parent">The xElement parent</param>
        public XData(XData parent, String name)
        {
            XElement xElement = new XElement(name);
            parent.Data.Add(xElement);
            this.Parent = parent;
            this.Data = xElement;
        }

        /// <summary>
        /// Adds a new attribute to existent node
        /// </summary>
        /// <param name="attName">The name of the attribute</param>
        /// <param name="value">The attribute value</param>
        public virtual void Add(String attName, String value)
        {
            if (!AttributeKeys.Contains(attName))
                this.Data.Add(new XAttribute(attName, value));
            else
                throw new TitaniaException(String.Format(Errors.XAttributeExist, attName, this.Data.Name));
        }
        /// <summary>
        /// Adds a new element to existent node
        /// </summary>
        /// <param name="nodeName">The name of the node to add</param>
        public virtual XElement Add(String nodeName)
        {
            XElement el = new XElement(nodeName);
            this.Data.Add(el);
            return el;
        }
        /// <summary>
        /// Exports the xdata node to file
        /// </summary>
        /// <param name="fileName">The name of the file</param>
        /// <returns>True if the file is created</returns>
        public void Export(String rootName, string fileName)
        {
            if (!System.IO.File.Exists(fileName))
                System.IO.File.Create(fileName).Close();
            else
            {
                System.IO.File.Delete(fileName);
                System.IO.File.Create(fileName).Close();
            }
            XNpc xml = new XNpc(rootName, new System.IO.FileInfo(fileName));
            XElement expXml = new XElement(this.Data.Name);
            try
            {
                Copy(expXml, this.Data);
                xml.Document.Root.Add(expXml);
                xml.Save();
            }
            catch (Exception exc)
            {
                throw new TitaniaException(String.Format(Errors.XmlExportNode, rootName), exc);
            }
        }
        /// <summary>
        /// Imports an Xml node to another xml file
        /// </summary>
        /// <param name="rootName">The root node of the xml file</param>
        /// <param name="selNodeName">The selected node of the xml file to import.
        /// The first node with this name is selected</param>
        /// <param name="fileName">The name of the file</param>
        /// <param name="parentNode">The node where the data is going to be inserted</param>
        public void Import(String rootName, String fileName, String selNodeName, XElement parentNode)
        {
            if (!System.IO.File.Exists(fileName))
                throw new TitaniaException(String.Format(Errors.XmlFileDoesNotExists, fileName));
            else
            {
                XNpc xml = new XNpc(rootName, new System.IO.FileInfo(fileName));
                XElement srcNode = xml.Document.Descendants().Where(x => x.Name.ToString() == selNodeName).FirstOrDefault();
                XElement newNode = new XElement(selNodeName);
                Copy(newNode, srcNode);
                parentNode.Add(newNode);
                xml.Save();
            }

        }

        /// <summary>
        /// Copy an xml node recursively
        /// </summary>
        /// <param name="expXml">The node where the data is going to be exported</param>
        /// <param name="expXml">The source data</param>
        public void Copy(XElement expXml, XElement srcXml)
        {
            XElement node;
            XAttribute att;
            foreach (XElement e in srcXml.Elements().ToArray())
            {
                node = new XElement(e.Name);
                foreach (XAttribute a in e.Attributes().ToArray())
                {
                    att = new XAttribute(a.Name, a.Value);
                    node.Add(att);
                }
                expXml.Add(node);
                Copy(node, e);
            }
        }
        /// <summary>
        /// Destroy this element
        /// </summary>
        public void Delete()
        {
            this.Data.Remove();
        }
        /// <summary>
        /// Remove an attribute by its name
        /// </summary>
        public virtual void Remove(String attName)
        {
            if (AttributeKeys.Contains(attName))
                this.Data.Attributes(attName).Remove();
            else
                throw new TitaniaException(String.Format(Errors.XmlAttNameNotExist, attName, this.Data.Name));
        }
        #endregion
        /// <summary>
        /// Print the xml value
        /// </summary>
        /// <returns>The xml string value</returns>
        public override string ToString()
        {
            return this.Data.ToString();
        }





    }
}
