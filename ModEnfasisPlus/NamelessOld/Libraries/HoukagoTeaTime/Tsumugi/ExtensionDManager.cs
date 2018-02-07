using Autodesk.AutoCAD.DatabaseServices;
using NamelessOld.Libraries.HoukagoTeaTime.Mio;

namespace NamelessOld.Libraries.HoukagoTeaTime.MugiChan
{
    public class ExtensionDictionaryManager : DManager
    {
        /// <summary>
        /// The id of the entity owner of the extension dictionary
        /// </summary>
        public ObjectId EntityId;
        /// <summary>
        /// Creates a new extension dictionary manager
        /// </summary>
        public ExtensionDictionaryManager(ObjectId entId, Transaction tr) :
            base(CreateDictionary(entId, tr))
        {
            this.EntityId = entId;
        }
        /// <summary>
        /// Crea el diccionario de extensión
        /// </summary>
        /// <param name="entId">El id de la extensión</param>
        /// <param name="tr">La transacción activa</param>
        /// <returns>El diccionario de extensión cargado</returns>
        static DBDictionary CreateDictionary(ObjectId entId, Transaction tr)
        {

            //Se revisa que exista el diccionario de extension
            Entity ent = entId.OpenEntity(tr);
            ObjectId id;
            if (ent.ExtensionDictionary.IsValid)
                id = ent.ExtensionDictionary;
            else
            {
                ent.UpgradeOpen();
                ent.CreateExtensionDictionary();
                id = ent.ExtensionDictionary;
            }
            return (DBDictionary)id.GetObject(OpenMode.ForRead);
        }
    }
}
