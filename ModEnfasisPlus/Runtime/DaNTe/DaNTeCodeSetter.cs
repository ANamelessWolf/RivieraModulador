using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using DaSoft.Riviera.OldModulador.Model;
using NamelessOld.Libraries.HoukagoTeaTime.Mio;
using NamelessOld.Libraries.HoukagoTeaTime.Tsumugi;
using System;
using System.Linq;
using static DaSoft.Riviera.OldModulador.Assets.Strings;
namespace DaSoft.Riviera.OldModulador.Runtime.DaNTe
{
    public class DaNTeCodeSetter
    {
        /// <summary>
        /// La lista de elementos satisfactiorios
        /// </summary>
        public ObjectIdCollection SuccedElements;
        /// <summary>
        /// La lista de elementos que no se pudo agregar DaNTe
        /// </summary>
        public ObjectIdCollection FailedElements;
        /// <summary>
        /// Crea una nueva editor de códigos de DaNTe
        /// </summary>
        public DaNTeCodeSetter()
        {
            this.SuccedElements = new ObjectIdCollection();
            this.FailedElements = new ObjectIdCollection();
        }
        /// <summary>
        /// Agrega el código de dante a los elementos trabajados en la aplicación
        /// </summary>
        public void AddDanteToDatabase(Transaction tr)
        {
            String strHandle;
            ObjectId entId = new ObjectId();
            XDataManager xMan;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            foreach (RivieraObject obj in App.DB.Objects)
            {
                try
                {
                    strHandle = obj.Data[FIELD_GEOMETRY, tr];
                    if (strHandle.Contains(','))
                        strHandle = strHandle.Split(',')[0];
                    entId = long.Parse(strHandle).GetId();
                    if (!this.SuccedElements.Contains(entId))
                    {
                        xMan = new XDataManager(entId);
                        xMan.Set(tr,
                            FIELD_DIC_DANTE.ToUpper(),
                            new TypedValue((int)DxfCode.ExtendedDataControlString, "{"),
                            new TypedValue((int)DxfCode.ExtendedDataAsciiString, "CON"),
                            new TypedValue((int)DxfCode.ExtendedDataAsciiString, obj.Code),
                            new TypedValue((int)DxfCode.ExtendedDataControlString, "}"));
                        this.SuccedElements.Add(entId);
                    }
                }
                catch (Exception exc)
                {
                    if (entId.IsValid)
                        this.FailedElements.Add(entId);
                    ed.WriteMessage("\n{0}", exc.Message);
                }
            }
        }
        /// <summary>
        /// Resalta los elementos que no se puedo agregar información de DaNTe
        /// </summary>
        public void HiglightFailed()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            ed.SetImpliedSelection(this.FailedElements.OfType<ObjectId>().ToArray());
        }

    }
}
