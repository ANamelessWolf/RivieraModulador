using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessOld.Libraries.Yggdrasil.Morrigan
{
    public class ColumnData
    {
        /// <summary>
        /// The index of the row
        /// </summary>
        public int Index;
        /// <summary>
        /// The name of the column
        /// </summary>
        public String Name;
        /// <summary>
        /// The next column
        /// </summary>
        public ColumnData Next;
        /// <summary>
        /// The previous column
        /// </summary>
        public ColumnData Previous;
        /// <summary>
        /// The column data
        /// </summary>
        public IEnumerable<CellData> Data
        {
            get { return this.Cells.Where(x => x.Column.Index == this.Index); }
        }
        /// <summary>
        /// The column content
        /// </summary>
        List<CellData> Cells;
        /// <summary>
        /// Set the data to the column
        /// </summary>
        public void SetData(List<CellData> cells)
        {
            this.Cells = cells;
        }
        /// <summary>
        /// Print the column description
        /// </summary>
        /// <returns>The column description as string</returns>
        public override string ToString()
        {
            return String.Format("C: {0}, Count: {1}", this.Index, this.Data.Count());
        }
    }
}
