using Nameless.Libraries.DB.Mikasa;
using Nameless.Libraries.DB.Mikasa.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DaSoft.Riviera.Modulador.Core.Model.DB
{
    public abstract class RivieraMeasureRow : DBMappingViewObject
    {
        const string FIELD_CODE = "CODIGO";
        /// <summary>
        /// The Bordeo panel measure
        /// </summary>
        public RivieraMeasure Measure;
        /// <summary>
        /// The Measure code
        /// </summary>
        public String Code;
        /// <summary>
        /// Gets the name of the table.
        /// </summary>
        /// <value>
        /// The name of the table.
        /// </value>
        /// <exception cref="NotSupportedException"></exception>
        public override string TableName => throw new NotSupportedException();
        /// <summary>
        /// Gets the primary key.
        /// </summary>
        /// <value>
        /// The primary key.
        /// </value>
        public override string PrimaryKey => FIELD_CODE;
        /// <summary>
        /// Creats the meausure.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>Creates the measure</returns>
        protected abstract RivieraMeasure CreateMeausure(SelectionResult[] result);
        /// <summary>
        /// Initializes a new instance of the <see cref="DBMappedObject"/> class.
        /// </summary>
        /// <param name="result">The selection result.</param>
        public RivieraMeasureRow(SelectionResult[] result) :
            base(result)
        {

        }
        /// <summary>
        /// Parses the object.
        /// </summary>
        /// <param name="result">The result.</param>
        protected override void ParseObject(SelectionResult[] result)
        {
            this.Measure = this.CreateMeausure(result);
            this.Code = result.GetString(FIELD_CODE);
        }
        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="conn">The database connection</param>
        /// <param name="tableName">The table name</param>
        /// <typeparam name="T">The type of selected row</typeparam>
        /// <returns>The selected rows</returns>
        public static IEnumerable<T> SelectAll<T>(string tableName, DB_Connector conn) where T : RivieraMeasureRow
        {
            try
            {
                QueryBuilder qb = new QueryBuilder(tableName, conn);
                qb.AddSelectionColumn();
                String query = qb.GetQuery();
                var result = conn.SelectView<T>(query);
                return result;
            }
            catch (Exception)
            {
                return new T[0];
            }
        }
        /// <summary>
        /// Serializes this instance and saves them in the xml file path
        /// </summary>
        /// <param name="xmlPth">The xml file path.</param>
        public void Serialize(String xmlPth)
        {
            XmlSerializer xs = new XmlSerializer(this.GetType());
            TextWriter tw = new StreamWriter(xmlPth);
            xs.Serialize(tw, this);
        }
        /// <summary>
        /// Deserializes the specified XML PTH.
        /// </summary>
        /// <param name="xmlPth">The XML PTH.</param>
        /// <returns>The deserialize instance</returns>
        public static T Deserialize<T>(String xmlPth) where T : RivieraMeasureRow
        {
            T result = null;
            XmlSerializer xs = new XmlSerializer(typeof(T));
            using (var sr = new StreamReader(xmlPth))
                result = (T)xs.Deserialize(sr);
            return result;
        }
    }
}