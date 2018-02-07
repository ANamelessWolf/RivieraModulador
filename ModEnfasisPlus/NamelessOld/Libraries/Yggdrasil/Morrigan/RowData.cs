using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessOld.Libraries.Yggdrasil.Morrigan
{
    public class RowData
    {
        /// <summary>
        /// The index of the row
        /// </summary>
        public int Index;
        /// <summary>
        /// The next row
        /// </summary>
        public RowData Next;
        /// <summary>
        /// The previous row
        /// </summary>
        public RowData Previous;
        /// <summary>
        /// The row content
        /// </summary>
        List<CellData> Cells;
        /// <summary>
        /// The column data
        /// </summary>
        public IEnumerable<CellData> Data
        {
            get { return this.Cells.Where(x => x.Row.Index == this.Index); }
        }
        /// <summary>
        /// Set the data to the rows
        /// </summary>
        public void SetData(List<CellData> cells)
        {
            this.Cells = cells;
        }
        /// <summary>
        /// Print the row description
        /// </summary>
        /// <returns>The row description as string</returns>
        public override string ToString()
        {
            return String.Format("R: {0}, Count: {1}", this.Index, this.Data.Count());
        }

    }
}
