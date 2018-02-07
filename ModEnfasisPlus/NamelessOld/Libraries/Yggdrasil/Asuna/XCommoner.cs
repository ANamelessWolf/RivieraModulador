using System;
using System.Xml.Linq;

namespace NamelessOld.Libraries.Yggdrasil.Asuna
{
    public abstract class XCommoner : XData
    {
        /// <summary>
        /// The xml keys
        /// </summary>
        public abstract String[] XCommoner_Keys { get; }
        /// <summary>
        /// The default data
        /// </summary>
        public abstract String[] DefaultData { get; }
        /// <summary>
        /// Creates a new xml xcommoner
        /// </summary>
        /// <param name="parent">The xml parent</param>
        /// <param name="xName">The xElemnent name</param>
        public XCommoner(XData parent, String xName)
            : base(parent, xName)
        {
            this.InitNodes();
        }
        /// <summary>
        /// Creates a new xml xcommoner
        /// </summary>
        /// <param name="el">The xml </param>
        /// <param name="parent">Accede al padre del xml</param>
        public XCommoner(XElement el, XData parent)
            : base(el, parent)
        {
            this.InitNodes();
        }

        /// <summary>
        /// Initialize the xcommoner nodes
        /// </summary>
        public void InitNodes()
        {
            for (int i = 0; i < XCommoner_Keys.Length; i++)
                if (this.Data.Attribute(XCommoner_Keys[i]) == null)
                    this.Add(this.XCommoner_Keys[i], this.DefaultData[i]);
        }
        /// <summary>
        /// Load the default data
        /// </summary>
        /// <param name="data">The data as object</param>
        public abstract void LoadData(Object data);





    }
}
