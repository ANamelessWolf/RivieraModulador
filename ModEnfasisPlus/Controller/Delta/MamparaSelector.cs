using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using DaSoft.Riviera.OldModulador.Model;
using DaSoft.Riviera.OldModulador.Model.Delta;
using DaSoft.Riviera.OldModulador.Runtime;
using DaSoft.Riviera.OldModulador.UI.Delta;
using NamelessOld.Libraries.HoukagoTeaTime.Runtime;
using NamelessOld.Libraries.HoukagoTeaTime.Yui;
using System.Collections.Generic;
using System.Linq;
using static DaSoft.Riviera.OldModulador.Assets.Strings;

namespace DaSoft.Riviera.OldModulador.Controller.Delta
{
    public class MamparaSelector
    {
        /// <summary>
        /// El código seleccionado
        /// </summary>
        public string Code;
        /// <summary>
        /// Accede al objectId de la entidad seleccionada
        /// </summary>
        public ObjectId SelectedId;
        /// <summary>
        /// El tamaño seleccionado
        /// </summary>
        public RivieraSize Size;
        /// <summary>
        /// El controlador activo de código y tamaño de mampara
        /// </summary>
        public Ctrl_Mampara Controller;
        /// <summary>
        /// El objeto que se encarga de iniciar el sembrado
        /// </summary>
        MamparaSower Sower;
        /// <summary>
        /// Crea un nuevo selector de mampara
        /// </summary>
        /// <param name="ctrl">El contralador de tamaño y código de mampara</param>
        public MamparaSelector(Ctrl_Mampara ctrl)
        {
            this.Controller = ctrl;
            this.Size = ctrl.Size;
            this.Code = ctrl.Code;
        }

        /// <summary>
        /// Realiza la selección de una mampara o una
        /// articulación
        /// </summary>
        /// <returns>Verdadero si se realizá la selección</returns>
        public bool Pick()
        {
            SelectionFilterBuilder fb = new SelectionFilterBuilder(typeof(BlockReference), typeof(Polyline));
            return Selector.ObjectId(MSG_SEL_OBJ, out SelectedId);
        }
        /// <summary>
        /// Inicia el sembrado de la aplicación
        /// </summary>
        public void Sow()
        {
            //1: Se tuvo que seleccionar o una mampara o una articulación
            RivieraObject obj = App.DB[this.SelectedId];
            if (obj != null)
            {
                //2: Mostramos la flechas segun el tipo de selección, solo hay dos válidos
                //Mampara o Mampara Joint
                if (obj is Mampara)
                {
                    (obj as Mampara).CheckRemate();
                    //De aqui seran dos tipos de mampara, Inicial, Final
                    if (obj.Parent == 0 && !obj.HasChildren)//Inicial sin hijos
                        ShowOnlyMampara(obj as Mampara, out this.Sower);
                    else if ((obj.Parent == 0 && obj.HasChildren) || //Inicial con hijos o final con hijos
                             (obj.Parent != 0 && obj.HasChildren))
                        ShowMamparaAndChild(obj as Mampara, out this.Sower);
                    else if (obj.Parent != 0 && !obj.HasChildren)//Final sin hijos
                        ShowMamparaAndParent(obj as Mampara, out this.Sower);
                }
                else if (obj is MamparaJoint && obj.Parent != 0)
                    //La articulación solo muestra las flechas de ella y de su padre
                    ShowJointAndParent(obj as MamparaJoint, out this.Sower);
                else if (obj is MamparaJoint)
                    ShowByJoint(obj as MamparaJoint, out this.Sower);
                //3: Se continua el proceso de sembrado
                if (this.Sower != null)
                    this.Sower.Sow();
            }
            else
                Selector.Ed.WriteMessage(MSG_NOT_IN_DB);
        }
        /// <summary>
        /// Genera la vista de las mamparas
        /// </summary>
        /// <param name="mamparaJoint">La union de la mampara</param>
        /// <param name="sower">El objeto a sembrar</param>
        private void ShowByJoint(MamparaJoint mamparaJoint, out MamparaSower sow)
        {
            RivieraObject[] children = GetChildren(mamparaJoint);
            //Intentamos cambiar el padre de una mampara.
            Mampara prospect = null;
            int c;
            foreach (Mampara mam in children.Where(x => x is Mampara))
            {
                c = mam.Children.Count(x => x.Value != 0);
                if (c == 1 || (c == 3 && mam.HasPanels))
                {
                    prospect = mam;
                    break;
                }
            }
            sow = null;
            if (prospect != null)
            {
                prospect.CheckRemate();

                //De aqui seran dos tipos de mampara, Inicial, Final
                if (prospect.Parent == 0 && !prospect.HasChildren)//Inicial sin hijos
                    ShowOnlyMampara(prospect, out sow);
                else if ((prospect.Parent == 0 && prospect.HasChildren) || //Inicial con hijos o final con hijos
                         (prospect.Parent != 0 && prospect.HasChildren))
                    ShowMamparaAndChild(prospect, out sow);
                else if (prospect.Parent != 0 && !prospect.HasChildren)//Final sin hijos
                    ShowMamparaAndParent(prospect, out sow);
            }

            //if (prospect != null)
            //{
            //    Handle jointId = mamparaJoint.Handle,
            //           mamId = prospect.Handle;
            //    VoidTransactionWrapper<RivieraObject> trw =
            //        new VoidTransactionWrapper<RivieraObject>(delegate (Document doc, Transaction tr, RivieraObject[] input)
            //        {
            //            mamparaJoint.Parent = mamId.Value;
            //            mamparaJoint.Data.Set(FIELD_PARENT, tr, mamId.Value);
            //            prospect.Parent = 0;
            //            prospect.Data.Set(FIELD_PARENT, tr, 0);
            //            if (prospect.Children[FIELD_FRONT] == 0)
            //            {
            //                prospect.Children[FIELD_FRONT] = jointId.Value;
            //                prospect.Data.Set(FIELD_FRONT, tr, jointId.Value);
            //            }
            //            else if (prospect.Children[FIELD_BACK] == 0)
            //            {
            //                prospect.Children[FIELD_BACK] = jointId.Value;
            //                prospect.Data.Set(FIELD_BACK, tr, jointId.Value);
            //            }


            //        });
            //    trw.Run(mampara);
            //}
            //Mampara mampara = App.DB[joint.Parent] is Mampara ? App.DB[joint.Parent] as Mampara : null;
            //if (mampara != null)
            //{
            //    mampara.CheckRemate();
            //    VoidTransactionWrapper<RivieraObject> trw =
            //        new VoidTransactionWrapper<RivieraObject>(delegate (Document doc, Transaction tr, RivieraObject[] input)
            //        {
            //            input[0].ShowDirections(tr);
            //            joint.ShowDirections(tr);
            //        });
            //    trw.Run(mampara);
            //    sow = new MamparaSower(this.Controller, mampara);
            //    sow.Joint = joint as MamparaJoint;
            //}
            //else
            //    sow = null;
        }

        /// <summary>
        /// Obtiene los hijos del elemento seleccionado
        /// </summary>
        /// <param name="obj">El elemento seleccionada</param>
        /// <returns>La colección de hijos del objeto seleccionado</returns>
        private RivieraObject[] GetChildren(RivieraObject obj)
        {
            return obj.HasChildren ?
                   obj.Children.Values.Where(x => x != 0).
                   Select<long, RivieraObject>(y => App.DB[y]).Where(z => z != null).ToArray() :
                   new RivieraObject[0];
        }

        /// <summary>
        /// Muestra las flechas de dirección de la mampara
        /// </summary>
        /// <param name="mampara">La mampara seleccionada</param>
        /// <param name="sow">El parámetro de salida es el sembrador de mamparas</param>
        private void ShowOnlyMampara(Mampara mampara, out MamparaSower sow)
        {
            VoidTransactionWrapper<Mampara> trw =
                new VoidTransactionWrapper<Mampara>(delegate (Document doc, Transaction tr, Mampara[] input)
                {
                    input[0].ShowDirections(tr);
                });
            trw.Run(mampara);
            sow = new MamparaSower(this.Controller, mampara);
        }
        /// <summary>
        /// Muestra las flechas de dirección de la mampara y de la
        /// articulación siempre y cuando la articulación sea de tipo MamparaJoint
        /// </summary>
        /// <param name="mampara">La mampara seleccionada</param>
        /// <param name="sow">El parámetro de salida es el sembrador de mamparas</param>
        private void ShowMamparaAndChild(Mampara mampara, out MamparaSower sow)
        {
            JointObject joint = this.SelectJointByMampara(mampara);
            VoidTransactionWrapper<RivieraObject> trw =
                new VoidTransactionWrapper<RivieraObject>(delegate (Document doc, Transaction tr, RivieraObject[] input)
                {
                    input[0].ShowDirections(tr);
                    if (joint is MamparaJoint)
                        joint.ShowDirections(tr);
                });
            trw.Run(mampara);
            sow = new MamparaSower(this.Controller, mampara);
            if (joint is MamparaJoint)
                sow.Joint = joint as MamparaJoint;
        }
        /// <summary>
        /// Muestra las flechas de dirección de la mampara y de la
        /// articulación siempre y cuando la articulación sea de tipo MamparaJoint
        /// </summary>
        /// <param name="mampara">La mampara seleccionada</param>
        /// <param name="sow">El parámetro de salida es el sembrador de mamparas</param>
        private void ShowMamparaAndParent(Mampara mampara, out MamparaSower sow)
        {
            JointObject joint = App.DB[mampara.Parent] is JointObject ? App.DB[mampara.Parent] as JointObject : null;
            VoidTransactionWrapper<RivieraObject> trw =
                new VoidTransactionWrapper<RivieraObject>(delegate (Document doc, Transaction tr, RivieraObject[] input)
                {
                    input[0].ShowDirections(tr);
                    if (joint != null && joint is MamparaJoint)
                        joint.ShowDirections(tr);
                });
            trw.Run(mampara);
            sow = new MamparaSower(this.Controller, mampara);
            if (joint != null && joint is MamparaJoint)
                sow.Joint = joint as MamparaJoint;
        }
        /// <summary>
        /// Muestra las flechas de dirección de la articulación y del 
        /// padre de la articulación
        /// </summary>
        /// <param name="mampara">La mampara seleccionada</param>
        /// <param name="sow">El parámetro de salida es el sembrador de mamparas</param>
        private void ShowJointAndParent(MamparaJoint joint, out MamparaSower sow)
        {
            Mampara mampara = App.DB[joint.Parent] is Mampara ? App.DB[joint.Parent] as Mampara : null;
            if (mampara != null)
            {
                mampara.CheckRemate();
                VoidTransactionWrapper<RivieraObject> trw =
                    new VoidTransactionWrapper<RivieraObject>(delegate (Document doc, Transaction tr, RivieraObject[] input)
                    {
                        input[0].ShowDirections(tr);
                        joint.ShowDirections(tr);
                    });
                trw.Run(mampara);
                sow = new MamparaSower(this.Controller, mampara);
                sow.Joint = joint as MamparaJoint;
            }
            else
                sow = null;
        }
        /// <summary>
        /// Selecciona una articulación con base a una mampara existente
        /// </summary>
        /// <param name="mam">La mampara existente</param>
        /// <returns>La articulación seleccionada</returns>
        private JointObject SelectJointByMampara(Mampara mam)
        {
            //Buscamos si tiene conectada una articulación de tipo mampara
            IEnumerable<long> validChildren = mam.Children.Values.Where(x => x != 0);
            //Seleccionamos la union
            RivieraObject joint = App.DB.Objects.Where(x => validChildren.Contains(x.Handle.Value)).FirstOrDefault();
            if (joint is JointObject)
                return joint as JointObject;
            else
                return null;
        }
    }
}
