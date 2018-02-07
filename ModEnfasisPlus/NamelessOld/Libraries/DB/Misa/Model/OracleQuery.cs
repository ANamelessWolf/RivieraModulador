using NamelessOld.Libraries.DB.Mikasa.Model;

namespace NamelessOld.Libraries.DB.Misa.Model
{
    public class OracleQuery : DB_Query
    {
        /// <summary>
        /// Format value to use in the current connection
        /// </summary>
        /// <param name="value">The value to be formated</param>
        /// <returns>The formated value</returns>
        public override string FormatValue(string value)
        {
            value = value.Replace("'", "''");
            return value;
        }
    }
}
