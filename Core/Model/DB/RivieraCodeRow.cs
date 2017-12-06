using Nameless.Libraries.DB.Mikasa.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaSoft.Riviera.Modulador.Core.Model.DB
{
    /// <summary>
    /// Defines a row taken from the Riviera Code View
    /// </summary>
    /// <seealso cref="Nameless.Libraries.DB.Mikasa.Model.DBMappingViewObject" />
    public class RivieraCodeRow : DBMappingViewObject
    {
        const string TABLENAME = "VIEW_RIVERA_CODE";
        const string FIELD_ID = "CODIGO";
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
        /// Parses the object.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <exception cref="NotImplementedException"></exception>
        protected override void ParseObject(SelectionResult[] result)
        {
            throw new NotImplementedException();
        }
    }
}
