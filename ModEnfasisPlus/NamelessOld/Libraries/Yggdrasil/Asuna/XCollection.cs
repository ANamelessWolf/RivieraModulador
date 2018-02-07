using NamelessOld.Libraries.Yggdrasil.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace NamelessOld.Libraries.Yggdrasil.Asuna
{
    public class XCollection : XCommoner, ICollection<XCommoner>
    {
        /// <summary>
        /// The default data
        /// </summary>
        public override String[] DefaultData { get { return new String[0]; } }
        /// <summary>
        /// Access the Commoner keys
        /// </summary>
        public override String[] XCommoner_Keys { get { return new String[0]; } }
        /// <summary>
        /// Access the Load Data
        /// </summary>
        /// <param name="data">The data to be loaded</param>
        public override void LoadData(object data) { return; }
        /// <summary>
        /// Even as commoner nodes, each node must define a key attribute name
        /// to be use as a key to avoid duplicated registries.
        /// </summary>
        public String KeyAttributeName;
        /// <summary>
        /// The list of common childrens
        /// </summary>
        List<XCommoner> XCollection_Children;
        /// <summary>
        /// The size of the collection
        /// </summary>
        public int XCollection_Count
        {
            get
            {
                return this.XCollection_Children.Count;
            }
        }
        /// <summary>
        /// Get a Xcommoner by its key
        /// </summary>
        /// <param name="key">The Xcommoner key</param>
        /// <returns>The Xcommoner key</returns>
        public override Object this[String key]
        {
            get
            {
                return this.XCollection_Children.Where(X => X.GetAttribute(KeyAttributeName) == key).FirstOrDefault();
            }
        }
        /// <summary>
        /// Get a Xcommoner by its key
        /// </summary>
        /// <param name="key">The Xcommoner key</param>
        /// <returns>The Xcommoner key</returns>
        public new Object this[int index]
        {
            get
            {
                return XCollection_Children[index];
            }
        }
        /// <summary>
        /// Creates a new XCollection object
        /// </summary>
        /// <param name="keyAttName">The name of the paremeter used as id parameter</param>
        /// <param name="xUniqueNode">The XCollection XElement node</param>
        /// <param name="data">The Xcollection data</param>
        public XCollection(String keyAttName, XElement xUniqueNode, XData parent, params XCommoner[] data) :
            base(xUniqueNode, parent)
        {
            this.KeyAttributeName = keyAttName;
            this.XCollection_Children = new List<XCommoner>();
            foreach (XCommoner d in data)
                this.Add(d);
        }
        /// <summary>
        /// Adds a new children to the collection
        /// XCollection does not save the document
        /// </summary>
        /// <param name="item">The item to be added</param>
        public virtual void Add(XCommoner item)
        {
            if (!this.Contains(item))
                this.XCollection_Children.Add(item);
            XElement node = this.Data.Elements().Where(X => X.Attribute(KeyAttributeName).Value == item.GetAttribute(KeyAttributeName) as String).FirstOrDefault();
            if (node == null)
                this.Data.Add(item.Data);
        }

        /// <summary>
        /// Updates X Collection
        /// </summary>
        public virtual void Update()
        {
        }
        /// <summary>
        /// Clear all childrens from the collection
        /// XCollection does not save the document
        /// </summary>
        public void Clear()
        {
            this.XCollection_Children.Clear();
            this.Data.Remove();
        }
        /// <summary>
        /// Verify if a XCommoner is contained in the XCollection
        /// </summary>
        /// <param name="item">The item to be tested</param>
        /// <returns>True if the item is contained</returns>
        public bool Contains(XCommoner item)
        {
            foreach (XCommoner child in this.XCollection_Children)
            {
                if (child.GetAttribute(KeyAttributeName) == item.GetAttribute(KeyAttributeName))
                    return true;
            }
            return false;
        }
        /// <summary>
        /// Verify if a XCommoner is contained in the XCollection
        /// Using the paramter key as index
        /// </summary>
        /// <param name="key">The item to be tested</param>
        /// <returns>True if the item is contained</returns>
        public bool ContainsKey(string key)
        {
            foreach (XCommoner child in this.XCollection_Children)
            {
                if (child.GetAttribute(KeyAttributeName) == key)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// Copy and array from a given index
        /// </summary>
        /// <param name="array">Copy the data from and array index</param>
        /// <param name="arrayIndex">The array index</param>
        public void CopyTo(XCommoner[] array, int arrayIndex)
        {
            this.XCollection_Children.CopyTo(array, arrayIndex);
        }
        /// <summary>
        /// Check if the collection is read only
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }
        /// <summary>
        /// Removes an Xcommoner from the collection
        /// </summary>
        /// <param name="item">The item to be removed</param>
        /// <returns>True if the item is removed</returns>
        public bool Remove(XCommoner item)
        {
            Boolean flag = false;
            if (item != null)
            {
                this.XCollection_Children.Remove(item);
                item.Data.Remove();
                flag = true;
            }
            return flag;
        }
        /// <summary>
        /// Get the XCollection enumerator
        /// </summary>
        /// <returns>The Xcommoner enumerator</returns>
        public IEnumerator<XCommoner> GetEnumerator()
        {
            return this.XCollection_Children.GetEnumerator();
        }
        /// <summary>
        /// Get the XCollection enumerator
        /// </summary>
        /// <returns>The Xcommoner enumerator</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.XCollection_Children.GetEnumerator();
        }
        /// <summary>
        /// Get the XCollection string value
        /// </summary>
        /// <returns>The XCollection string</returns>
        public override string ToString()
        {
            return String.Format(this.GetType().Name + ": " + Notices.XCollectionDataString, this.KeyAttributeName, this.XCollection_Count);
        }


    }
}
