using Nameless.Libraries.DB.Mikasa;
using Nameless.Libraries.DB.Mikasa.Model;
using Nameless.Libraries.DB.Misa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaSoft.Riviera.Modulador.Core.Model.DB
{
    /// <summary>
    /// Defines an Executive object taken from the table EJECUTIVO
    /// </summary>
    /// <seealso cref="Nameless.Libraries.DB.Mikasa.Model.DBMappedObject" />
    public class Ejecutivo : DBMappedObject
    {
        const string TABLENAME = "EJECUTIVO";
        const string FIELD_ID = "ID";
        const string FIELD_NAME = "NOMBRE";
        const string FIELD_PASS = "PASSWORD";
        const string FIELD_COMPANY = "CLAVE_COMPANIA";
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
        /// The executive identifier
        /// </summary>
        public new int Id;
        /// <summary>
        /// The excutive password
        /// </summary>
        public String Password;
        /// <summary>
        /// The excutive name
        /// </summary>
        public String User;
        /// <summary>
        /// The company key
        /// </summary>
        public RivieraCompany RivieraCo;
        /// <summary>
        /// Initializes a new instance of the <see cref="Ejecutivo"/> class.
        /// </summary>
        /// <param name="result">The result.</param>
        public Ejecutivo(SelectionResult[] result) : base(result)
        {

        }
        /// <summary>
        /// Parses the object.
        /// </summary>
        /// <param name="result">The result.</param>
        protected override void ParseObject(SelectionResult[] result)
        {
            int id, cmpId;
            this.Id = int.TryParse(result.GetString(this.PrimaryKey), out id) ? id : -1;
            this.Password = result.GetString(FIELD_PASS);
            this.RivieraCo = (RivieraCompany)(int.TryParse(result.GetString(FIELD_COMPANY), out cmpId) ? cmpId : -1);
            this.User = result.GetString(FIELD_NAME);
        }
        /// <summary>
        /// Gets the ejecutivo.
        /// </summary>
        /// <param name="credential">The user credential.</param>
        /// <returns>The selected excutive</returns>
        public static int GetEjecutivoId(UserCredential credential, DB_Connector conn)
        {
            QueryBuilder qB = new QueryBuilder(TABLENAME, conn);
            qB.AddSelectionColumn();
            qB.AddEqualCondition(FIELD_ID, credential.Username, "AND");
            qB.AddEqualCondition(FIELD_PASS, credential.Password, "AND");
            qB.AddEqualCondition(FIELD_COMPANY, (int)credential.Company);
            String query = qB.GetQuery();
            Ejecutivo e = conn.Select<Ejecutivo>(query).FirstOrDefault();
            return e != null ? e.Id : -1;
        }
        /// <summary>
        /// Gets the insertion fields.
        /// Not Supported for this class
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public override DBField[] GetInsertionFields(DB_Connector conn)
        {
            throw new NotSupportedException();
        }
        /// <summary>
        /// Gets the update fields.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public override DBField[] GetUpdateFields(DB_Connector conn)
        {
            throw new NotSupportedException();
        }
        /// <summary>
        /// Updates the data.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <exception cref="NotSupportedException"></exception>
        public override void UpdateData(KeyValuePair<string, object>[] input)
        {
            throw new NotSupportedException();
        }
    }
}
