using Autodesk.AutoCAD.DatabaseServices;
using DaSoft.Riviera.OldModulador.Model.Delta;
using DaSoft.Riviera.OldModulador.Runtime;
using DaSoft.Riviera.OldModulador.Runtime.Delta;
using NamelessOld.Libraries.HoukagoTeaTime.MugiChan;
using NamelessOld.Libraries.HoukagoTeaTime.Yui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DaSoft.Riviera.OldModulador.Assets.Strings;
using static DaSoft.Riviera.OldModulador.Assets.RIVIERA_CONST;
using DaSoft.Riviera.OldModulador.Model;
using Autodesk.AutoCAD.Geometry;
using NamelessOld.Libraries.HoukagoTeaTime.Ritsu;

namespace DaSoft.Riviera.OldModulador.Controller.Delta
{
    /// <summary>
    /// Esta clase realiza el giro de una mampara 180"
    /// </summary>
    public class Mampara54Flipper
    {

        public Mampara Mampara;
        /// <summary>
        /// Inicializa una instancia de <see cref="Mampara54Flipper"/>
        /// </summary>
        public Mampara54Flipper()
        {
            if (Mampara54Flipper.Pick(out this.Mampara))
            {
                int frente = int.Parse(Mampara.Code.Substring(6, 2));
                if (frente != 54)
                    throw new DeltaException("La mampara debe contar con un frente de 54\"");
            }
            else
                throw new DeltaException("Cancelado cambio de mampara");
        }
        /// <summary>
        /// Realiza el proceso de selección de una mampara
        /// </summary>
        /// <param name="mampara">La mampara a seleccionar</param>
        /// <returns>Verdadero en caso de seleccionar una mampara.</returns>
        private static bool Pick(out Mampara mampara)
        {
            SelectionFilterBuilder fb =
                new SelectionFilterBuilder(typeof(BlockReference));
            ObjectId selectedId;
            if (Selector.ObjectId(MSG_SEL_OBJ, out selectedId))
                mampara = App.DB[selectedId] as Mampara;
            else
                mampara = null;
            return mampara != null;
        }
        /// <summary>
        /// Realizá el proceso de actualización de la aplicación
        /// </summary>
        /// <param name="tr">La transacción activa</param>
        public void Regen(Transaction tr)
        {
            this.Mampara.Regen();
            //Escalamos el cuerpo si estan el modo imperial
            Mampara54Flipper.Scale(this.Mampara, tr);
            //Elementos asociados a la mampara
            if (this.Mampara.BiomboId != 0)
                this.Regen(App.DB[this.Mampara.BiomboId], tr);
            if (this.Mampara.Children[FIELD_LEFT_FRONT] != 0)
                this.Regen(App.DB[this.Mampara.Children[FIELD_LEFT_FRONT]], tr);
            if (this.Mampara.Children[FIELD_RIGHT_FRONT] != 0)
                this.Regen(App.DB[this.Mampara.Children[FIELD_RIGHT_FRONT]], tr);
            this.Mampara.Save(tr);
        }
        /// <summary>
        /// Realiza la actualización de un elemento asociado a la mampara
        /// </summary>
        /// <param name="obj">El objeto a escalar</param>
        /// <param name="tr">La transacción activa</param>
        private void Regen(RivieraObject obj, Transaction tr)
        {
            obj.Regen();
            if (App.Riviera.Units == DaNTeUnits.Imperial)
            {
                var matrix = Matrix3d.Scaling(IMPERIAL_FACTOR, obj.Line.StartPoint);
                ObjectIdCollection geometry = new ObjectIdCollection(obj.Ids.OfType<ObjectId>().Where(x => x != obj.Line.Id).ToArray());
                geometry.Transform(matrix, tr);
            }
            obj.Save(tr);
        }
        private static void Scale(Mampara mampara, Transaction tr)
        {
            if (App.Riviera.Units == DaNTeUnits.Imperial)
            {
                Double ang = Mampara54Flipper.GetRotation(mampara, tr);
                var matrix = Matrix3d.Scaling(IMPERIAL_FACTOR, (ang == 0 ? mampara.Line.StartPoint : mampara.Line.EndPoint));
                var geometry = new ObjectIdCollection(mampara.Ids.OfType<ObjectId>().Where(x => x != mampara.Line.Id).ToArray());
                geometry.Transform(matrix, tr);
            }
        }

        /// <summary>
        /// Se revisa si la mampara contiene una bandera de geometría invertida
        /// </summary>
        /// <param name="tr">La transacción activa</param>
        public void Swap(Transaction tr)
        {
            //Se rotará la geometría para Mamparas y para Biombos
            this.SetRotationDictionary(this.Mampara.Line.ExtensionDictionary, tr);
            if (this.Mampara.BiomboId != 0)
                this.SetRotationDictionary(App.DB[this.Mampara.BiomboId].Line.ExtensionDictionary, tr);

            //Se realiza el intercambio de paneles.
            //if (this.Mampara.Children[FIELD_LEFT_FRONT] != 0 && this.Mampara.Children[FIELD_RIGHT_FRONT] != 0)
            //{
            //    var left = App.DB[this.Mampara.Children[FIELD_LEFT_FRONT]];
            //    var right = App.DB[this.Mampara.Children[FIELD_RIGHT_FRONT]];
            //    string tmpLeft = left.Data[FIELD_EXTRA, tr],
            //           tmpRight = right.Data[FIELD_EXTRA, tr];
            //    (left as RivieraPanelStack).Collection = PanelRaw.Parse(tmpRight).ToList();
            //    (right as RivieraPanelStack).Collection = PanelRaw.Parse(tmpLeft).ToList();
            //    left.Data.Set(FIELD_EXTRA, tr, tmpRight);
            //    right.Data.Set(FIELD_EXTRA, tr, tmpLeft);
            //}
        }
        /// <summary>
        /// Establece un campo en los objetos dibujados para indicar que seran dibujados con una rotación.
        /// </summary>
        /// <param name="extDicId">El diccionario de extensión</param>
        /// <param name="tr">La transacción activa</param>
        private void SetRotationDictionary(ObjectId extDicId, Transaction tr)
        {
            DBDictionary extDic = extDicId.GetObject(OpenMode.ForWrite) as DBDictionary;
            DManager dman = new DManager(extDic);
            Xrecord rotXRec;
            double angle;
            if (dman.TryGetRegistry(ROTATE_GEOMETRY_XRECORD, out rotXRec, tr))
            {
                angle = double.Parse(rotXRec.GetDataAsString(tr)[0]);
                angle = angle == 0d ? Math.PI : 0d;
            }
            else
            {
                angle = Math.PI;
                rotXRec = extDic.AddXRecord(ROTATE_GEOMETRY_XRECORD, tr);
            }
            rotXRec.SetData(tr, angle.ToString());
        }
        public static Double GetRotation(RivieraObject obj, Transaction tr)
        {
            if (obj.Line != null && obj.Line.ExtensionDictionary.IsValid)
            {
                var extDicId = obj.Line.ExtensionDictionary;
                DBDictionary extDic = extDicId.GetObject(OpenMode.ForWrite) as DBDictionary;
                DManager dman = new DManager(extDic);
                Xrecord rotXRec;
                double angle;
                if (dman.TryGetRegistry(ROTATE_GEOMETRY_XRECORD, out rotXRec, tr))
                    angle = double.Parse(rotXRec.GetDataAsString(tr)[0]);
                else
                    angle = 0;
                return angle;
            }
            else
                return 0;
        }
    }
}
