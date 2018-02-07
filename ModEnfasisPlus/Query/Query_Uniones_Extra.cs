using NamelessOld.Libraries.DB.Misa.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaSoft.Riviera.OldModulador.Query
{
    public class Query_Uniones_Extra : OracleQuery
    {
        #region Constants
        const string TABLENAME = "DT_UNIONES_EXTRA";
        const string FIELD_ID = "ID_UNION";
        const string FIELD_TREAT = "TRATAMIENTO";
        #endregion
        /// <summary>
        /// Selecciona todas las descripciones disponibles
        /// </summary>
        /// <returns>El query de selección de descripción</returns>
        public String SelecRules()
        {
            return String.Format("SELECT * FROM {0}", TABLENAME);
        }
    }
}
