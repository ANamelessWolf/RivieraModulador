using Autodesk.AutoCAD.DatabaseServices;
using NamelessOld.Libraries.HoukagoTeaTime.MugiChan;
using System;
using System.Linq;
using static DaSoft.Riviera.OldModulador.Assets.Strings;
namespace DaSoft.Riviera.OldModulador.Model
{
    public class RivieraData
    {
        /// <summary>
        /// El administrador de diccionarios de la aplicación
        /// </summary>
        ExtensionDictionaryManager DMan;
        /// <summary>
        /// El id del objeto
        /// </summary>
        public ObjectId Id;
        /// <summary>
        /// Los campos que guarda un objeto de riviera
        /// </summary>
        String[] Fields = new String[]
        {
            FIELD_CODE,         //0
            FIELD_SIZE,         //1
            FIELD_START,        //2
            FIELD_END,          //3
            FIELD_LEFT_FRONT,   //4
            FIELD_FRONT,        //5
            FIELD_RIGHT_FRONT,  //6
            FIELD_BACK,         //7 
            FIELD_LEFT_BACK,    //8
            FIELD_RIGHT_BACK,   //9
            FIELD_PARENT,       //10
            FIELD_GEOMETRY,     //11
            FIELD_EXTRA,        //12
        };
        /// <summary>
        /// Crea un nuevo objeto Riviera data, para
        /// leer y guardar información de una entidad
        /// </summary>
        /// <param name="id">El id de la entidad</param>
        /// <param name="tr">La transacción activa</param>
        public RivieraData(ObjectId id, Transaction tr)
        {
            DMan = new ExtensionDictionaryManager(id, tr);
            this.Id = id;
        }
        /// <summary>
        /// Guarda la información del objeto
        /// </summary>
        /// <param name="id">El id de la entidad</param>
        /// <param name="tr">La transacción activa</param>
        /// <param name="data">La información a guardar</param>
        public void Save(Transaction tr, String[] data)
        {
            for (int i = 0; i < data.Length; i++)
                DMan.AddRegistry(Fields[i], tr).SetData(tr, data[i]);
        }
        /// <summary>
        /// Obtiene un valor guardado en el diccionario
        /// </summary>
        /// <param name="field">El campo a leer</param>
        /// <param name="tr">La transacción usada para leer el valor</param>
        /// <returns>El valor leido</returns>
        public String this[String field, Transaction tr]
        {
            get
            {
                return this.DMan.GetRegistry(field, tr).GetDataAsString(tr).FirstOrDefault();
            }
        }
        /// <summary>
        /// Extrae la información desde el objeto de AutoCAD
        /// </summary>
        /// <param name="tr">La transacción actual</param>
        /// <returns>La colección de información</returns>
        public String[] Extract(Transaction tr)
        {
            return this.Fields.Select<String, String>(x => this[x, tr]).ToArray();
        }
        /// <summary>
        /// Realiza un cambio en la información de un campo
        /// del objeto
        /// </summary>
        /// <param name="field">El nombre del campo</param>
        /// <param name="tr">La transacción activa</param>
        /// <param name="val">El valor a establecer</param>
        public void Set(String field, Transaction tr, string val)
        {
            this.DMan.GetRegistry(field, tr).SetData(tr, new String[] { val });
        }


    }
}
