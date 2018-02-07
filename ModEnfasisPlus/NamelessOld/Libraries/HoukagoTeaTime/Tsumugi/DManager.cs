using Autodesk.AutoCAD.DatabaseServices;
using NamelessOld.Libraries.HoukagoTeaTime.Assets;
using NamelessOld.Libraries.HoukagoTeaTime.Mio;
using NamelessOld.Libraries.HoukagoTeaTime.Runtime;
using NamelessOld.Libraries.Yggdrasil.Lilith;
using System;

namespace NamelessOld.Libraries.HoukagoTeaTime.MugiChan
{
    public class DManager : NamelessObject
    {
        /// <summary>
        /// The id of the dictionary
        /// </summary>
        public new ObjectId Id;
        /// <summary>
        /// Creates a new manager
        /// </summary>
        /// <param name="dic">El diccionario a manejar</param>
        public DManager(DBDictionary dic)
        {
            this.Id = dic.Id;
        }

        /// <summary>
        /// Adds a new registry to the dictionary
        /// If an xrecord exist with the same key, the record is overwritten
        /// </summary>
        /// <param name="key">The name of the xRecord to add</param>
        /// <param name="tr">The active transaction</param>
        /// <returns>The new xRecord</returns>
        public Xrecord AddRegistry(String key, Transaction tr)
        {
            try
            {
                return this.Id.OpenObject<DBDictionary>(tr).AddXRecord(key, tr);
            }
            catch (Exception exc)
            {
                throw new RomioException(String.Format("{0}: {1}", String.Format(Errors.ErrorCreatingDictionary), exc.Message), exc);
            }
        }
        /// <summary>
        /// Gets a xrecord from the dictionary
        /// </summary>
        /// <param name="key">The name of the field</param>
        /// <param name="tr">The active transaction</param>
        /// <returns>The xRecord</returns>
        public Xrecord GetRegistry(string key, Transaction tr)
        {
            try
            {
                return this.Id.OpenObject<DBDictionary>(tr).GetXRecord(key, tr);
            }
            catch (Exception exc)
            {
                throw new RomioException(String.Format("{0}: {1}", String.Format(Errors.ErrorReadingXrecord), exc.Message), exc);
            }
        }
        /// <summary>
        /// Gets a xrecord from the dictionary
        /// </summary>
        /// <param name="key">The name of the field</param>
        /// <param name="tr">The active transaction</param>
        /// <param name="xrecord">Como parámetro de salida el registro encontrado</param>
        /// <returns>The xRecord</returns>
        public Boolean TryGetRegistry(string key, out Xrecord xrecord, Transaction tr)
        {
            try
            {
                xrecord = this.Id.OpenObject<DBDictionary>(tr).GetXRecord(key, tr);
                return true;
            }
            catch (Exception)
            {
                xrecord = null;
                return false;
            }
        }
        /// <summary>
        /// Adds a new dictionary to the dictionary
        /// </summary>
        public DBDictionary AddDictionary(String key, Transaction tr)
        {
            try
            {
                return this.Id.OpenObject<DBDictionary>(tr).AddDictionary(key, tr);
            }
            catch (Exception exc)
            {
                throw new RomioException(String.Format("{0}: {1}", String.Format(Errors.ErrorCreatingDictionary), exc.Message), exc);
            }
        }
        /// <summary>
        /// Gets a dictionary from the dictionary
        /// </summary>
        /// <param name="key">The name of the field</param>
        /// <param name="tr">The active transaction</param>
        /// <returns>The dictionary</returns>
        public DBDictionary GetDictionary(string key, Transaction tr)
        {
            try
            {
                return this.Id.OpenObject<DBDictionary>(tr).GetDictionary(key, tr);
            }
            catch (Exception exc)
            {
                throw new RomioException(String.Format("{0}: {1}", String.Format(Errors.ErrorReadingXrecord), exc.Message), exc);
            }
        }
        /// <summary>
        /// Gets a dictionary from the dictionary
        /// </summary>
        /// <param name="key">The name of the field</param>
        /// <param name="tr">The active transaction</param>
        /// <param name="dic">Como parámetro de salida el diccionario encontrado</param>
        /// <returns>The dictionary</returns>
        public Boolean TryGetDictionary(string key, out DBDictionary dic, Transaction tr)
        {
            try
            {
                dic = this.Id.OpenObject<DBDictionary>(tr).GetDictionary(key, tr);
                return true;
            }
            catch (Exception)
            {
                dic = null;
                return false;
            }
        }

    }
}
