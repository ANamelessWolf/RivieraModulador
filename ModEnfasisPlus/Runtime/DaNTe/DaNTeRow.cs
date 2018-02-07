using NamelessOld.Libraries.DB.Misa.Model;
using NamelessOld.Libraries.DB.Tessa.Model;
using System;

namespace DaSoft.Riviera.OldModulador.Runtime.DaNTe
{
    public class DaNTeRow
    {
        #region Properties
        /// <summary>
        /// Gets the Clave used as Id Field in DaNTe MDB.
        /// </summary>
        public String Clave { get { return id; } }
        /// <summary>
        /// Gets the Value calculated in the Application, this value
        /// is used as the "Cantidad" field in DaNTe MDB.
        /// </summary>
        public Double Value { get { return quantifyValue; } }
        /// <summary>
        /// The element handle
        /// </summary>
        public String Handle;
        /// <summary>
        /// Verdadero si el elemento es módulo
        /// </summary>
        public Boolean IsModule;
        #endregion
        #region Variables
        Double quantifyValue;
        String id;
        #endregion
        #region Constructor
        /// <summary>
        /// Creates a new DaNTe Row
        /// </summary>
        /// <param name="id">El codigo a insertar</param>
        /// <param name="isModule">Verdadero si es módulo</param>
        /// <param name="quantifyValue">EL valor a cuantificar</param>
        public DaNTeRow(Double quantifyValue, String id, Boolean isModule = false)
        {
            this.quantifyValue = quantifyValue;
            this.id = id;
            this.IsModule = isModule;
        }

        internal bool HasAsociados(Access_Connector conn)
        {
            return true;
        }

        internal bool HasAsociados(Oracle_Connector conn)
        {
            return true;
        }
        #endregion

    }
}
