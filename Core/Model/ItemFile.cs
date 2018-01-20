using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaSoft.Riviera.Modulador.Core.Model
{
    /// <summary>
    /// Defines a bindable item file
    /// </summary>
    public class ItemFile
    {
        /// <summary>
        /// The item name
        /// </summary>
        public string ItemName;
        /// <summary>
        /// The file path
        /// </summary>
        public string FilePath;
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemFile"/> class.
        /// </summary>
        /// <param name="file">The file.</param>
        public ItemFile(FileInfo file)
        {
            this.ItemName = file.Name;
            this.FilePath = file.FullName;
        }
        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return ItemName;
        }
    }
}
