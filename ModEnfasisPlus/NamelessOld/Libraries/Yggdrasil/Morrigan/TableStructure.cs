using NamelessOld.Libraries.Yggdrasil.Exceptions;
using NamelessOld.Libraries.Yggdrasil.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessOld.Libraries.Yggdrasil.Morrigan
{
    /// <summary>
    /// Defines an structure of table type
    /// </summary>
    public class TableStructure
    {
        /// <summary>
        /// The colecction of colums
        /// </summary>
        public List<ColumnData> Columns;
        /// <summary>
        /// The colecction of rows
        /// </summary>
        public List<RowData> Rows;
        /// <summary>
        /// The cell collection
        /// </summary>
        public List<CellData> Cells;
        /// <summary>
        /// Creates a new table structure
        /// </summary>
        public TableStructure()
        {
            this.Columns = new List<ColumnData>();
            this.Rows = new List<RowData>();
            this.Cells = new List<CellData>();
        }
        /// <summary>
        /// Adds a new column
        /// </summary>
        public void AddColumn()
        {
            //Agregá una nueva columna
            this.Columns.Add(
                new ColumnData()
                {
                    Index = this.Columns.Count,
                    Next = null,
                    Previous = this.Columns.Count == 0 ? null : this.Columns[this.Columns.Count - 1],
                });
            //Estable el contenido de las columnas
            this.Columns[this.Columns.Count - 1].SetData(this.Cells);
            //Actualiza el contenido de la siguiente columna
            if (this.Columns.Count != 1)
                this.Columns[this.Columns.Count - 2].Next = this.Columns[this.Columns.Count - 1];
        }
        /// <summary>
        /// Adds a new column
        /// </summary>
        public void AddColumn(String name)
        {
            this.AddColumn();
            this.Columns[this.Columns.Count - 1].Name = name;
        }
        /// <summary>
        /// Adds a new row
        /// </summary>
        /// <param name="data">The data to be added</param>
        public void AddRow(params CellData[] data)
        {
            if (data.Length == this.Columns.Count)
            {
                //Agregá una nueva fila
                RowData row = new RowData()
                {
                    Index = this.Rows.Count,
                    Next = null,
                    Previous = this.Rows.Count == 0 ? null : this.Rows[this.Rows.Count - 1]
                };
                row.SetData(this.Cells);
                this.Rows.Add(row);
                //Estable el valor del apuntador de la siguiente columna
                if (this.Rows.Count != 1)
                    this.Rows[this.Rows.Count - 2].Next = this.Rows[this.Columns.Count - 1];
                //Establece la información recien agregada a las celdas seleccionadas.
                for (int i = 0; i < this.Columns.Count; i++)
                {
                    data[i].Column = this.Columns[i];
                    data[i].Row = row;
                }
            }
            else
                throw (new DarkIllusionException(Errors.ColumnDifferentSize));
        }
        /// <summary>
        /// Print the table description
        /// </summary>
        /// <returns>The table description as string</returns>
        public override string ToString()
        {
            return String.Format("Rows: {0}, Columns: {1}", this.Columns.Count(), this.Rows.Count());
        }
    }
}
