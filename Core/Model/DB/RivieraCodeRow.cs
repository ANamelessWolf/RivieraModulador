using DaSoft.Riviera.Modulador.Core.Controller;
using DaSoft.Riviera.Modulador.Core.Runtime;
using Nameless.Libraries.DB.Mikasa;
using Nameless.Libraries.DB.Mikasa.Model;
using Nameless.Libraries.Yggdrasil.Lilith;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DaSoft.Riviera.Modulador.Core.Assets.Strings;
namespace DaSoft.Riviera.Modulador.Core.Model.DB
{
    /// <summary>
    /// Defines a row taken from the Riviera Code View
    /// </summary>
    /// <seealso cref="Nameless.Libraries.DB.Mikasa.Model.DBMappingViewObject" />
    public class RivieraCodeRow : DBMappingViewObject
    {
        const string TABLENAME = "RIVIERA.VW_RIVERA_CODE";
        const string FIELD_ID = "CODIGO";
        const string FIELD_LINE = "LINEA";
        const string FIELD_CODE = "CODIGO";
        const string FIELD_CODE_DESC = "CODIGO_DESC";
        const string FIELD_BLOQUE = "BLOQUE";
        const string FIELD_TYPE = "TIPO";
        const string FIELD_PP = "PANEL_DOBLE";
        const string FIELD_ACAB = "ACABADO";
        const string FIELD_ACAB_DESC = "ACABADO_DESC";
        /// <summary>
        /// The dictionary data
        /// </summary>
        public Dictionary<String, String> Data;
        /// <summary>
        /// Gets the name of the table.
        /// </summary>
        /// <value>
        /// The name of the table.
        /// </value>
        public override string TableName => TABLENAME;
        /// <summary>
        /// Gets the primary key.
        /// </summary>
        /// <value>
        /// The primary key.
        /// </value>
        public override string PrimaryKey => FIELD_ID;
        /// <summary>
        /// Initializes a new instance of the <see cref="RivieraCodeRow"/> class.
        /// </summary>
        public RivieraCodeRow(SelectionResult[] result) :
            base(result)
        {

        }
        /// <summary>
        /// Parses the object.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <exception cref="NotImplementedException"></exception>
        protected override void ParseObject(SelectionResult[] result)
        {
            this.Data = new Dictionary<String, String>();
            foreach (var val in result)
                this.Data.Add(val.ColumnName, val.Value.ToString());
        }
        /// <summary>
        /// Gets the riviera code for a specific design line.
        /// </summary>
        /// <param name="rows">The selected rows from the database.</param>
        /// <param name="line">The Riviera design line.</param>
        /// <returns>The List of Riviera codes</returns>
        /// <returns>The Riviera codes</returns>
        public static List<RivieraCode> GetRivieraCodeForLine(IEnumerable<RivieraCodeRow> rows, DesignLine line)
        {
            List<RivieraCode> codes = new List<RivieraCode>();
            RivieraCode code;
            foreach (var row in rows.Where(x => x.Data[FIELD_LINE].ParseDesignLine() == line))
            {
                code = codes.FirstOrDefault(x => x.Code == row.Data[FIELD_ID]);
                if (code == null)
                {
                    code = new RivieraCode()
                    {
                        Block = row.Data[FIELD_BLOQUE],
                        Code = row.Data[FIELD_ID],
                        Description = row.Data[FIELD_CODE_DESC],
                        DoublePanel = row.Data[FIELD_PP] == "S",
                        Line = line,
                        ElementType = row.Data[FIELD_TYPE].ParseElementType()
                    };
                    codes.Add(code);
                }
                code.AddAcabado(row.Data[FIELD_ACAB], row.Data[FIELD_ACAB_DESC]);
            }
            return codes;
        }
        /// <summary>
        /// Selects all rows from the Riviera code table.
        /// </summary>
        /// <param name="conn">The current connection.</param>
        /// <returns></returns>
        public static IEnumerable<RivieraCodeRow> SelectAll(DB_Connector conn)
        {
            try
            {
                QueryBuilder qB = new QueryBuilder(TABLENAME, conn);
                qB.AddSelectionColumn();
                String query = qB.GetQuery();
                var result = conn.SelectView<RivieraCodeRow>(query);
                return result;
            }
            catch (Exception exc)
            {
                throw exc.CreateNamelessException<RivieraException>(ERR_LOAD_RIV_CODES);
            }

        }
        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return String.Join(",", this.Data.Values.ToArray());
        }
    }
}
