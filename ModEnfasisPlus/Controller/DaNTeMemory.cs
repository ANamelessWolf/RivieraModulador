using Autodesk.AutoCAD.DatabaseServices;
using DaSoft.Riviera.OldModulador.Model;
using NamelessOld.Libraries.HoukagoTeaTime.MugiChan;
using static DaSoft.Riviera.OldModulador.Assets.Strings;
namespace DaSoft.Riviera.OldModulador.Controller
{
    public class DaNTeMemory
    {
        /// <summary>
        /// El registro para guardar el id de la entidad
        /// </summary>
        Xrecord IdXRecord;
        /// <summary>
        /// El registro que guarda el tipo de unidades que usa la entidad
        /// </summary>
        Xrecord UnitsXRecord;
        /// <summary>
        /// Establece el Id de la entidad seleccionada
        /// </summary>
        /// <param name="handleId">El id de la entidad selecionada</param>
        /// <param name="tr">La transacción activa</param>
        public void SetId(long handleId, Transaction tr)
        {
            this.IdXRecord.SetData(tr, handleId.ToString());
        }
        /// <summary>
        /// Obtiene el Id de la entidad seleccionada
        /// </summary>
        /// <param name="tr">La transacción activa</param>
        public long GetId(Transaction tr)
        {
            long id;
            return long.TryParse(this.IdXRecord.GetDataAsString(tr)[0], out id) ? id : 0;
        }
        /// <summary>
        /// Establece el tipo de unidades de la entidad seleccionada
        /// </summary>
        /// <param name="units">El tipo de unidades</param>
        /// <param name="tr">La transacción activa</param>
        public void SetUnits(DaNTeUnits units, Transaction tr)
        {
            this.UnitsXRecord.SetData(tr, ((int)units).ToString());
        }
        /// <summary>
        /// Obtiene el tipo de unidades de la entidad seleccionada
        /// </summary>
        /// <param name="tr">La transacción activa</param>
        public DaNTeUnits GetUnits(Transaction tr)
        {
            int units;
            return (DaNTeUnits)(int.TryParse(this.IdXRecord.GetDataAsString(tr)[0], out units) ? units : 0);
        }
        /// <summary>
        /// Crea un Id
        /// </summary>
        /// <param name="entId">El object id de la entidad</param>
        /// <param name="handleId">El handle del id</param>
        /// <param name="tr">La transacción activa</param>
        public DaNTeMemory(ObjectId entId, Transaction tr)
        {
            ExtensionDictionaryManager dman = new ExtensionDictionaryManager(entId, tr);
            Xrecord xRec;

            if (!dman.TryGetRegistry(FIELD_REG_ID, out xRec, tr))
                xRec = dman.AddRegistry(FIELD_REG_ID, tr);
            this.IdXRecord = xRec;
            if (!dman.TryGetRegistry(FIELD_REG_UNITS, out xRec, tr))
                xRec = dman.AddRegistry(FIELD_REG_UNITS, tr);
            this.UnitsXRecord = xRec;
        }
    }
}
