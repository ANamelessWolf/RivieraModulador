using Autodesk.AutoCAD.DatabaseServices;
using NamelessOld.Libraries.HoukagoTeaTime.Assets;
using NamelessOld.Libraries.HoukagoTeaTime.Runtime;
using System;
using System.Linq;

namespace NamelessOld.Libraries.HoukagoTeaTime.MugiChan
{
    public static class DataExtender
    {
        /// <summary>
        /// Adds a new dictionary to the dictionary
        /// </summary>
        /// <param name="dic">The parent dictionary</param>
        /// <param name="key">The key of the dictionary</param>
        /// <param name="tr">The active transaction</param>
        public static DBDictionary AddDictionary(this DBDictionary dic, String key, Transaction tr)
        {
            try
            {
                DBDictionary dictionary = dic.Id.GetObject(OpenMode.ForWrite) as DBDictionary;
                DBDictionary d = new DBDictionary();
                dictionary.SetAt(key, d);
                tr.AddNewlyCreatedDBObject(d, true);
                return d;
            }
            catch (Exception exc)
            {
                throw new RomioException(String.Format("{0}: {1}", String.Format(Errors.ErrorCreatingDictionary), exc.Message), exc);
            }
        }
        /// <summary>
        /// Adds a new registry to the dictionary
        /// </summary>
        public static Xrecord AddXRecord(this DBDictionary dic, String key, Transaction tr)
        {
            try
            {
                DBDictionary dictionary = dic.Id.GetObject(OpenMode.ForWrite) as DBDictionary;
                Xrecord xr = new Xrecord();
                dictionary.SetAt(key, xr);
                tr.AddNewlyCreatedDBObject(xr, true);
                return xr;
            }
            catch (Exception exc)
            {
                throw new RomioException(String.Format("{0}: {1}", String.Format(Errors.ErrorCreatingXRecord), exc.Message), exc);
            }
        }
        /// <summary>
        /// Gets a dictionary from the dictionary
        /// </summary>
        /// <param name="dic">The parent dictionary</param>
        /// <param name="key">The key of the dictionary</param>
        /// <param name="tr">The active transaction</param>
        public static DBDictionary GetDictionary(this DBDictionary dic, String key, Transaction tr)
        {
            try
            {
                DBDictionary dictionary = dic.Id.GetObject(OpenMode.ForWrite) as DBDictionary;
                ObjectId id = dictionary.GetAt(key);
                DBObject obj = id.GetObject(OpenMode.ForRead);
                if (obj is DBDictionary)
                    return obj as DBDictionary;
                else
                    throw new RomioException(Errors.NotADictionary);
            }
            catch (Exception exc)
            {
                throw new RomioException(String.Format("{0}: {1}", String.Format(Errors.ErrorReadingXrecord), exc.Message), exc);
            }
        }
        /// <summary>
        /// Gets a registry from the dictionary
        /// </summary>
        public static Xrecord GetXRecord(this DBDictionary dic, String key, Transaction tr)
        {
            try
            {
                DBDictionary dictionary = dic.Id.GetObject(OpenMode.ForWrite) as DBDictionary;
                ObjectId id = dictionary.GetAt(key);
                DBObject obj = id.GetObject(OpenMode.ForRead);
                if (obj is Xrecord)
                    return obj as Xrecord;
                else
                    throw new RomioException(Errors.NotAXRecord);
            }
            catch (Exception exc)
            {
                throw new RomioException(String.Format("{0}: {1}", String.Format(Errors.ErrorReadingXrecord), exc.Message), exc);
            }
        }
        /// <summary>
        /// Sets the content of the XRecord
        /// </summary>
        /// <param name="xRec">The data host as a Xrecord</param>
        /// <param name="data">The data to be saved</param>
        /// <param name="tr">The active transaction</param>
        public static void SetData(this Xrecord xRec, Transaction tr, params TypedValue[] data)
        {
            try
            {
                Xrecord xRecord = xRec.Id.GetObject(OpenMode.ForWrite) as Xrecord;
                xRecord.Data = new ResultBuffer(data);
            }
            catch (Exception exc)
            {
                throw new RomioException(String.Format("{0}: {1}", String.Format(Errors.NewDataAddedXrecord), exc.Message), exc);
            }
        }
        /// <summary>
        /// Sets the content of the XRecord
        /// </summary>
        /// <param name="xRec">The data host as a Xrecord</param>
        /// <param name="data">The data to be saved, an array of strings</param>
        /// <param name="tr">The active transaction</param>
        public static void SetData(this Xrecord xRec, Transaction tr, params String[] data)
        {
            try
            {
                TypedValue[] tpData = new TypedValue[data.Length];
                Xrecord xRecord = xRec.Id.GetObject(OpenMode.ForWrite) as Xrecord;
                for (int i = 0; i < data.Length; i++)
                    tpData[i] = new TypedValue((int)DxfCode.Text, data[i]);
                xRecord.Data = new ResultBuffer(tpData);
            }
            catch (Exception exc)
            {
                throw new RomioException(String.Format("{0}: {1}", String.Format(Errors.NewDataAddedXrecord), exc.Message), exc);
            }
        }
        /// <summary>
        /// Gets the content of the XRecord
        /// </summary>
        /// <param name="xRec">The data host as a Xrecord</param>
        /// <param name="tr">The active transaction</param>
        public static String[] GetDataAsString(this Xrecord xRec, Transaction tr)
        {
            try
            {
                Xrecord xRecord = xRec.Id.GetObject(OpenMode.ForWrite) as Xrecord;
                if (xRecord.Data != null)
                    return xRecord.Data.OfType<TypedValue>().Select<TypedValue, String>(x => x.Value.ToString()).ToArray();
                else
                    return new String[0];
            }
            catch (Exception exc)
            {
                throw new RomioException(String.Format("{0}: {1}", String.Format(Errors.ErrorReadingXrecord), exc.Message), exc);
            }
        }
    }
}
