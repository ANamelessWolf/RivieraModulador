using NamelessOld.Libraries.DB.Misa.Model;
using System;

namespace DaSoft.Riviera.OldModulador.Query
{
    public class Query_Acabados : OracleQuery
    {
        #region Constants
        const string TABLENAME = "DT_COD_ACAB";
        const string FRENTE_TYPE = "FG";
        const string ALTO_TYPE = "AG";
        const string FIELD_CODE = "CODIGO";
        const string FIELD_ACABADO = "ACABADO";
        const string FIELD_DESC = "DESC";
        #endregion
        /// <summary>
        /// Selecciona todas las descripciones disponibles
        /// </summary>
        /// <returns>El query de selección de descripción</returns>
        public String SelecAcabados()
        {
            return String.Format("SELECT * FROM {0}", TABLENAME);
        }
    }
}
