using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using NamelessOld.Libraries.HoukagoTeaTime.Runtime;
using NamelessOld.Libraries.Yggdrasil.Lilith;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessOld.Libraries.HoukagoTeaTime.Tsumugi
{
    public class AttManager : NamelessObject
    {
        /// <summary>
        /// El id del block reference
        /// </summary>
        public new ObjectId Id;
        /// <summary>
        /// Crea una nueva instancia del attribute manager
        /// </summary>
        /// <param name="id">El id del block reference</param>
        public AttManager(ObjectId id)
        {
            this.Id = id;
        }

        /// <summary>
        /// Gets the value of the parameter
        /// </summary>
        /// <param name="attname">The name of the attribute</param>
        /// <returns>The string of the attribute</returns>
        public String Get(String attname)
        {
            BlankTransactionWrapper<String> trW = new BlankTransactionWrapper<string>(
                delegate (Document doc, Transaction tr)
                {
                    return Get(attname, tr);
                });
            return trW.Run();
        }
        /// <summary>
        /// Gets the value of the parameter
        /// </summary>
        /// <param name="attname">The name of the attribute</param>
        /// <param name="tr">The active transaction</param>
        /// <returns>The string of the attribute</returns>
        public String Get(String attname, Transaction tr)
        {
            String value = String.Empty;
            AttributeReference attRef;
            if (HasAttribute(attname, out attRef, tr))
            {
                attRef = (AttributeReference)attRef.Id.GetObject(OpenMode.ForRead);
                value = attRef.TextString;
            }
            return value;
        }
        /// <summary>
        /// Gets the value of the parameter
        /// </summary>
        /// <param name="attname">The name of the attribute</param>
        /// <param name="value">The attribute new value</param>
        /// <returns>The string of the attribute</returns>
        public void SetAttribute(String attname, String value)
        {
            FastTransactionWrapper trW = new FastTransactionWrapper(
                delegate (Document doc, Transaction tr)
                {
                    SetAttribute(attname, value, tr);
                });
            trW.Run();
        }
        /// <summary>
        /// Gets the value of the parameter
        /// </summary>
        /// <param name="attname">The name of the attribute</param>
        /// <param name="value">The attribute new value</param>
        /// <param name="tr">The active transaction</param>
        /// <returns>The string of the attribute</returns>
        public void SetAttribute(String attname, String value, Transaction tr)
        {
            AttributeReference attRef;
            if (HasAttribute(attname, out attRef, tr))
            {
                attRef = (AttributeReference)
                    attRef.Id.GetObject(OpenMode.ForWrite);
                attRef.TextString = value;
            }
        }
        /// <summary>
        /// Validates if an attribute exists. 
        /// If the definition exists, it creates the attr
        /// </summary>
        /// <param name="attname">The attribute name</param>
        /// <param name="attRef">The attribute reference</param>
        /// <param name="tr">The active transaction</param>
        /// <returns>True if a attribute definition exist with the given name</returns>
        private Boolean HasAttribute(String attname, out AttributeReference attRef, Transaction tr)
        {
            attRef = null;
            IEnumerable<AttributeDefinition> attDefs;
            IEnumerable<AttributeReference> attRefs;
            AttributeCollection attColl;
            //1: Abrir el bloque
            BlockReference blkRef = (BlockReference)this.Id.GetObject(OpenMode.ForRead);
            BlockTableRecord blkRec = (BlockTableRecord)blkRef.BlockTableRecord.GetObject(OpenMode.ForRead);
            attColl = blkRef.AttributeCollection;
            //2: Selección de todos los objetos del bloque
            IEnumerable<DBObject> blockEntities = blkRec.OfType<ObjectId>().Select<ObjectId, DBObject>(x => x.GetObject(OpenMode.ForRead));
            //Deben existir atributos de definición para poder crear un parámetro
            if (blockEntities.Where(x => x is AttributeDefinition).Count() > 0)
            {
                //Se realiza la selección de los atributos de definición
                attDefs = blockEntities.Where(x => x is AttributeDefinition).Select<DBObject, AttributeDefinition>(y => y as AttributeDefinition);
                //Se abren los atributos de referencia existentes
                attRefs = attColl.OfType<ObjectId>().Select<ObjectId, AttributeReference>(x => x.GetObject(OpenMode.ForRead) as AttributeReference);
                //Se realiza la validación
                if (blkRef.AttributeCollection.Count > 0 && attRefs.Where(x => x.Tag.ToUpper() == attname.ToUpper()).Count() > 0)
                    attRef = attRefs.Where(x => x.Tag.ToUpper() == attname.ToUpper()).FirstOrDefault();
                else if (attDefs.Where(x => x.Tag.ToUpper() == attname.ToUpper()).Count() > 0)
                {
                    AttributeDefinition attDef;
                    //Si no existen la referencia se debe crear
                    attDef = attDefs.Where(x => x.Tag.ToUpper() == attname.ToUpper()).FirstOrDefault();
                    attRef = new AttributeReference();
                    attRef.SetAttributeFromBlock(attDef, blkRef.BlockTransform);
                    blkRef.UpgradeOpen();
                    blkRef.AttributeCollection.AppendAttribute(attRef);
                    tr.AddNewlyCreatedDBObject(attRef, true);
                }
            }
            return attRef != null;
        }
        /// <summary>
        /// Validates if an attribute exists. 
        /// If the definition exists, it creates the attr
        /// </summary>
        /// <param name="attname">The attribute name</param>
        /// <param name="attRef">The attribute reference</param>
        /// <returns>True if a attribute definition exist with the given name</returns>
        private Boolean HasAttribute(String attname, out AttributeReference attRef)
        {
            attRef = null;
            BlankTransactionWrapper<AttributeReference> trW = new BlankTransactionWrapper<AttributeReference>(
                delegate (Document doc, Transaction tr)
                {
                    AttributeReference att;
                    HasAttribute(attname, out att, tr);
                    return att;
                });
            attRef = trW.Run();
            return attRef != null;
        }
    }
}
