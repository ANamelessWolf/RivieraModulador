using NamelessOld.Libraries.Yggdrasil.Exceptions;
using NamelessOld.Libraries.Yggdrasil.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace NamelessOld.Libraries.Yggdrasil.Asuna
{
    public class XUnique : XData
    {
        /// <summary>
        /// Access the Node Collection
        /// </summary>
        Dictionary<String, XData> Nodes
        {
            get
            {
                Dictionary<String, XData> nodes = new Dictionary<String, XData>();
                foreach (XElement el in this.Data.Elements().ToArray())
                    if (!nodes.ContainsKey(el.Name.ToString()))
                        nodes.Add(el.Name.ToString(), new XData(el, this));
                return nodes;
            }
        }

        /// <summary>
        /// Access an especific xml Node
        /// </summary>
        /// <param name="nodeName">The node name used to select</param>
        /// <returns>The XElement node</returns>
        public new XData this[String nodeName]
        {
            get
            {
                if (this.Nodes.ContainsKey(nodeName))
                    return this.Nodes[nodeName];
                else
                    throw new TitaniaException(String.Format(Errors.XmlNodeNameNotExist, nodeName, this.Data.Name));
            }
        }
        /// <summary>
        /// Access the attribute by its name
        /// </summary>
        /// <param name="nodeName">The node name used to select</param>
        /// <param name="attName">The name of the attribute</param>
        /// <returns>The attribute value</returns>
        public String this[String nodeName, string attName]
        {
            get
            {
                return this[nodeName][attName] as String;
            }
            set
            {
                this[nodeName][attName] = value;
            }
        }

        /// <summary>
        /// Creates a new XData Object
        /// </summary>
        /// <param name="content">The Xml data</param>
        /// <param name="parent">The Xml unique parent</param>
        public XUnique(XElement content, XData parent)
            : base(content, parent)
        {


        }

        /// <summary>
        /// Adds a new element to existent node
        /// </summary>
        /// <param name="nodeName">The name of the node to add</param>
        public override XElement Add(string nodeName)
        {
            if (!this.Nodes.ContainsKey(nodeName))
            {
                XElement uElement = new XElement(nodeName);
                this.Data.Add(uElement);
                return uElement;
            }
            else
                throw new TitaniaException(String.Format(Errors.XElementExist, nodeName, this.Data.Name));
        }

    }
}
