using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaSoft.Riviera.Modulador.Bordeo.Model.UI
{
    /// <summary>
    /// Gets the item that fills a combo box
    /// </summary>
    public struct BordeoPanelHeightItem
    {
        /// <summary>
        /// The panel height
        /// </summary>
        public BordeoPanelHeight Height;
        /// <summary>
        /// Gets the name of the image.
        /// </summary>
        /// <value>
        /// The name of the image.
        /// </value>
        public string ImageName => String.Format("{0}.png", this.Height.ToString());
        /// <summary>
        /// Gets the size of the nominal.
        /// </summary>
        /// <value>
        /// The size of the nominal.
        /// </value>
        public String NominalSize
        {
            get
            {
                String value = String.Empty;
                switch (this.Height)
                {
                    case BordeoPanelHeight.NormalMini:
                        value = "42\"";
                        break;
                    case BordeoPanelHeight.NormalMiniNormal:
                        value = "69\"";
                        break;
                    case BordeoPanelHeight.NormalThreeMini:
                        value = "72\"";
                        break;
                    case BordeoPanelHeight.NormalTwoMinis:
                        value = "57\"";
                        break;
                    case BordeoPanelHeight.ThreeNormals:
                        value = "81\"";
                        break;
                    case BordeoPanelHeight.TwoNormalOneMini:
                        value = "69\"";
                        break;
                    case BordeoPanelHeight.TwoNormals:
                        value = "54\"";
                        break;
                }
                return value;
            }
        }
        /// <summary>
        /// Gets the size of the nominal.
        /// </summary>
        /// <value>
        /// The size of the nominal.
        /// </value>
        public String SizeDescription
        {
            get
            {
                String value = String.Empty;
                switch (this.Height)
                {
                    case BordeoPanelHeight.NormalMini:
                        value = "(27\" + 15\")";
                        break;
                    case BordeoPanelHeight.NormalMiniNormal:
                        value = "(27\" + 15\" + 27\")";
                        break;
                    case BordeoPanelHeight.NormalThreeMini:
                        value = "(27\" + 15\" + 15\" + 15\")";
                        break;
                    case BordeoPanelHeight.NormalTwoMinis:
                        value = "(27\" + 15\" + 15\")";
                        break;
                    case BordeoPanelHeight.ThreeNormals:
                        value = "(27\" + 27\" + 27\")";
                        break;
                    case BordeoPanelHeight.TwoNormalOneMini:
                        value = "(27\" + 27\" + 15\")";
                        break;
                    case BordeoPanelHeight.TwoNormals:
                        value = "(27\" + 27\")";
                        break;
                }
                return value;
            }
        }
    }
}
