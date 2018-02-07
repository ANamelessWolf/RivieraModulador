using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using DaSoft.Riviera.OldModulador.Model;
using DaSoft.Riviera.OldModulador.Model.Delta;
using DaSoft.Riviera.OldModulador.Runtime;
using NamelessOld.Libraries.HoukagoTeaTime.Runtime;
using NamelessOld.Libraries.HoukagoTeaTime.Yui;
using System;
using System.Collections.Generic;
using System.Linq;
using static DaSoft.Riviera.OldModulador.Assets.Strings;

namespace DaSoft.Riviera.OldModulador.Controller.Delta
{
    public class MamparaEraser
    {
        /// <summary>
        /// La colección de elementos de la aplicación que estan en lista de eliminación
        /// </summary>
        public List<RivieraObject> ElementsToDelete;
        /// <summary>
        /// Crea un nuevo borrador de mamparas
        /// </summary>
        public MamparaEraser()
        {
            this.ElementsToDelete = new List<RivieraObject>();
        }

        /// <summary>
        /// Realiza la selección de elementos a eliminar
        /// </summary>
        /// <returns>Los elementos a eliminar</returns>
        public Boolean Pick()
        {
            ObjectIdCollection ids;
            SelectionFilterBuilder fb = new SelectionFilterBuilder(typeof(BlockReference), typeof(Polyline));
            if (Selector.ObjectIds(MSG_SEL_TO_DEL, out ids, fb.Filter))
            {
                RivieraObject obj;
                foreach (ObjectId id in ids)
                {
                    obj = App.DB[id];
                    if (obj != null && obj is Mampara)
                        ElementsToDelete.Add(obj);
                }
            }
            return this.ElementsToDelete.Count > 0;
        }
        /// <summary>
        /// Inicia el proceso de borrado
        /// </summary>
        public void Erase()
        {
            try
            {
                FastTransactionWrapper tr = new FastTransactionWrapper(EraseTransaction);
                tr.Run();
            }
            catch (Exception exc)
            {
                Selector.Ed.WriteMessage("\n{0}", exc.Message);
            }
        }
        /// <summary>
        /// Inicia el proceso de eliminado de la aplicación
        /// </summary>
        /// <param name="doc">El documento activo</param>
        /// <param name="tr">La transacción activa</param>
        private void EraseTransaction(Document doc, Transaction tr)
        {
            foreach (RivieraObject obj in this.ElementsToDelete)
                if (obj is Mampara)
                    this.DeleteMampara(obj as Mampara, tr);
            //else if (obj is JointObject)
            //    this.DeleteJointAsNode(obj as JointObject, tr);
        }
        /// <summary>
        /// Verificamos que tipo de mampara es
        /// Inicial, final, o intermedia
        /// </summary>
        /// <param name="mam">La mampara a eliminar</param>
        /// <param name="tr">La transacción activa</param>
        private void DeleteMampara(Mampara mam, Transaction tr)
        {
            //Es una mampara intermedia sin padre.
            Boolean middleNoParent = mam.Children[FIELD_FRONT] != 0 && mam.Children[FIELD_BACK] != 0;
            //Inicial
            if (mam.Parent == 0 && !middleNoParent)
            {
                RivieraObject[] children = this.GetChildren(mam);
                foreach (RivieraObject child in children)
                {
                    //Si la lista de elementos a eliminar no contiene al hijo,
                    //debemos actualizarlo
                    if (this.IsInQueueToDelete(child))
                        this.UpdateMamparaSon(child, tr);
                }
            }
            //Final, su único hijo es un remate final, o no hay hijos
            else if (mam.Parent != 0 && this.IsTerminal(mam))
            {
                RivieraObject parent = App.DB[mam.Parent], grandparent;
                if (mam.HasDoublePanels)
                {
                    var j = mam.JointDoublePanels;
                    mam.JointDoublePanels.PanelArray.Clean(j, tr);
                    j.PanelArray = null;
                }
                //Las mamparas solo son hijos de articulaciones.
                if (parent is JointObject) //Para futuros cambios valido de todas formas
                {
                    JointObject jointParent = (parent as JointObject);
                    //Al quitarle una mampara a una articulación I o L se debe borrar la articulación
                    if (this.IsJointLOrI(jointParent))
                    {
                        if (jointParent.Parent != 0)
                        {
                            grandparent = App.DB[parent.Parent];
                            string key = String.Empty;
                            foreach (String childKey in grandparent.Children.Keys)
                                if (jointParent.Handle.Value == grandparent.Children[childKey])
                                {
                                    key = childKey;
                                    break;
                                }
                            if (key != String.Empty)
                            {
                                grandparent.Children[key] = 0;
                                grandparent.Data.Set(key, tr, "0");
                            }
                        }
                        else
                        {
                            RivieraObject[] children = this.GetChildren(jointParent);
                            foreach (RivieraObject child in children)
                            {
                                child.Parent = 0;
                                child.Data.Set(FIELD_PARENT, tr, "0");
                            }
                        }
                        jointParent.Delete(tr);
                    }
                    else
                        this.UpdateJoint(parent as JointObject, mam, tr);
                }
                //Borramos el remate final
                if (mam.Remates != null)
                    this.DeleteRemate(tr, mam.Remates.ToArray());
                this.DeletePaneles(mam, tr);

            }
            else
            {
                if (mam.HasDoublePanels)
                {
                    var j = mam.JointDoublePanels;
                    mam.JointDoublePanels.PanelArray.Clean(j, tr);
                    j.PanelArray = null;
                }
                JointObject joint;
                //Borrado de los hijos
                RivieraObject[] children = this.GetChildren(mam);
                foreach (RivieraObject child in children)
                {
                    //Si la lista de elementos a eliminar no contiene al hijo, debe analizarse, solo hay articulaciones
                    if (child is JointObject)
                    //Union de mampara, solo se elimina en L y en I
                    {
                        joint = child as JointObject;
                        if (joint.Type == JointType.Joint_L || joint.Type == JointType.Joint_I)
                            this.DeleteJoint(joint, tr);
                        else
                            this.RemoveParent(joint, tr);

                    }
                    else
                        child.Delete(tr);
                }
                RivieraObject parent = App.DB[mam.Parent];
                //Si la lista de elementos a eliminar no contiene al padre
                if (parent is JointObject)
                {
                    joint = parent as JointObject;
                    if (joint.Type == JointType.Joint_L || joint.Type == JointType.Joint_I)
                    {
                        DeleteJoint(parent as JointObject, tr);
                        if (joint.Parent != 0)
                        {
                            RivieraObject grandparent = App.DB[parent.Parent];
                            string key = String.Empty;
                            foreach (String childKey in grandparent.Children.Keys)
                                if (joint.Handle.Value == grandparent.Children[childKey])
                                {
                                    key = childKey;
                                    break;
                                }
                            if (key != String.Empty)
                            {
                                grandparent.Children[key] = 0;
                                grandparent.Data.Set(key, tr, "0");
                            }
                        }
                    }
                    else
                        this.RemoveChild(mam, joint, tr);
                }
            }
            RivieraObject biombo = App.DB[mam.BiomboId];
            if (biombo != null)
                biombo.Delete(tr);
            mam.Delete(tr);
        }
        /// <summary>
        /// Elimina el vinculo entre un objeto y una mampara
        /// </summary>
        /// <param name="mam">La mampara a eliminar</param>
        /// <param name="obj">El objeto a eliminar</param>
        /// <param name="tr">La transacción activa</param>
        private void RemoveChild(Mampara mam, RivieraObject obj, Transaction tr)
        {
            string key = String.Empty;
            foreach (String childKey in mam.Children.Keys)
                if (mam.Handle.Value == obj.Children[childKey])
                {
                    key = childKey;
                    break;
                }
            if (key != String.Empty)
            {
                obj.Children[key] = 0;
                obj.Data.Set(key, tr, "0");
            }
        }

        /// <summary>
        /// Borra los paneles asociados
        /// </summary>
        /// <param name="mam">La mampara inicial</param>
        /// <param name="tr">La transacción activa</param>
        private void DeletePaneles(Mampara mam, Transaction tr)
        {
            if (mam.Children[FIELD_LEFT_FRONT] != 0)
                App.DB[mam.Children[FIELD_LEFT_FRONT]]?.Delete(tr);
            if (mam.Children[FIELD_RIGHT_FRONT] != 0)
                App.DB[mam.Children[FIELD_RIGHT_FRONT]]?.Delete(tr);

        }

        /// <summary>
        /// Remueve el vinculo entre padre e hijo, si el padre esta en lista de elimación el 
        /// vinculo se mantiene
        /// </summary>
        /// <param name="parent">El padre que perderá el vinculo con su hijo</param>
        /// <param name="son">El hijo eliminado</param>
        /// <param name="tr">La transacción activa</param>
        private void RemoveLink(RivieraObject parent, RivieraObject son, Transaction tr)
        {
            if (this.IsInQueueToDelete(parent))
            {
                String key = parent.Children.Where(x => x.Value == son.Handle.Value).FirstOrDefault().Key;
                if (key != null)
                {
                    //Cache
                    parent.Children[key] = 0;
                    //Memoria
                    parent.Data.Set(key, tr, "0");
                }
                else
                {
                    Selector.Ed.WriteMessage("Error al borrar {0}, con padre {1}\n[{2}-{3}]", parent, son, son.Start, son.End);
                    App.Riviera.Log.AppendEntry(String.Format("Error al borrar {0}, con padre {1}\n[{2}-{3}]", parent, son, son.Start, son.End), NamelessOld.Libraries.Yggdrasil.Lain.Protocol.Error, "RemoveLink", "MamparaEraser");
                }
            }
        }

        /// <summary>
        /// Checa si la articulación es una I o una L
        /// </summary>
        /// <param name="obj">El objeto a checar</param>
        /// <returns>Verdadero se la articulación es de tipo I o L</returns>
        private bool IsJointLOrI(JointObject obj)
        {
            return obj.Type == JointType.Joint_I || obj.Type == JointType.Joint_L;
        }

        /// <summary>
        /// Checa si la mampara es un nodo terminal
        /// </summary>
        /// <param name="mam">La mampara a validar</param>
        /// <returns>Verdadero si es terminal</returns>
        private bool IsTerminal(Mampara mam)
        {
            int childCount = mam.Children.Where(x => x.Value != 0).Count();
            Boolean hasPanels = mam.Children[FIELD_LEFT_FRONT] != 0 && mam.Children[FIELD_RIGHT_FRONT] != 0;
            if (hasPanels)
                childCount -= 2;
            return childCount == 0 || (childCount == 1 && mam.Remates != null);
        }
        /// <summary>
        /// Checa si el objeto esta en lista de eliminación
        /// </summary>
        /// <param name="obj">El objeto a eliminar</param>
        /// <returns>Verdadero si esta en la lista de eliminación</returns>
        private bool IsInQueueToDelete(RivieraObject obj)
        {
            return obj != null && this.ElementsToDelete.Where(x => x.Handle.Value == obj.Handle.Value).Count() == 0;
        }



        /// <summary>
        /// Se borrará la información del padre a los hijos de la articulación
        /// de tipo mampara
        /// </summary>
        /// <param name="joint">La articulación de mampara</param>
        /// <param name="tr">La transacción activa</param>
        private void DeleteJoint(JointObject joint, Transaction tr)
        {
            //1: Remover la referencia de padre a los hijos
            RivieraObject[] children = this.GetChildren(joint);
            foreach (RivieraObject child in children)
                this.RemoveParent(child, tr);
            joint.Delete(tr);
        }
        /// <summary>
        /// Se actualiza la información del padre a los hijos de la articulación
        /// de tipo mampara
        /// </summary>
        /// <param name="joint">La articulación de mampara</param>
        /// <param name="son">El hijo que se esta removiendo</param>
        /// <param name="tr">La transacción activa</param>
        private void UpdateJoint(JointObject joint, Mampara son, Transaction tr)
        {
            if (!IsJointLOrI(joint))
                this.RemoveLink(joint, son, tr);
            //Si se tiene una L, que es la mampara mínima se hace un downgrade a I
            if (joint.Type == JointType.Joint_L)
            {
                String key = joint.Children.Where(x => x.Value != 0).FirstOrDefault().Key;
                if (key == FIELD_FRONT && joint is MamparaJoint)
                    (joint as MamparaJoint).Downgrade(tr);
            }
        }

        /// <summary>
        /// Verificamos el contenido de la unión
        /// Inicial, final, o intermedia
        /// </summary>
        /// <param name="joint">La mampara a eliminar</param>
        /// <param name="tr">La transacción activa</param>
        private void DeleteJointAsNode(JointObject joint, Transaction tr)
        {
            RivieraObject parent = App.DB[joint.Parent];
            //Se borra la referencia del padre
            String key = parent.Children.Where(x => x.Value == joint.Handle.Value).FirstOrDefault().Key;
            //Cache
            parent.Children[key] = 0;
            //Memoria
            parent.Data.Set(key, tr, "0");
            //Se borra la union en caso de ser I o L
            DeleteJoint(joint, tr);
        }

        /// <summary>
        /// Actualizamos la información del hijo y se evalua si el hijo debe borrarse
        /// </summary>
        /// <param name="child">El hijo analizar</param>
        private void UpdateMamparaSon(RivieraObject child, Transaction tr)
        {
            if (child is JointObject)//Union de mampara, solo se elimina en L y en I
            {
                JointObject joint = child as JointObject;
                if (joint.Type == JointType.Joint_L || joint.Type == JointType.Joint_I)
                    this.DeleteJoint(joint, tr);
                else
                {
                    //Cache
                    joint.Parent = 0;
                    //Memoria
                    joint.Data.Set(FIELD_PARENT, tr, "0");
                }
            }
            else if (child is MamparaRemateFinal)//Siempre se elimina el remate final
                this.DeleteRemate(tr, child as MamparaRemateFinal);
            else if (child is RivieraPanelStack)
                child.Delete(tr);
        }
        /// <summary>
        /// Un remate no tiene hijos, siempre se borra si su padre se selecciono
        /// </summary>
        /// <param name="remate">Los remates finales de la mampara</param>
        /// <param name="tr">La transacción activa</param>
        private void DeleteRemate(Transaction tr, params MamparaRemateFinal[] remates)
        {
            for (int i = 0; i < remates.Length; i++)
                remates[i].Delete(tr);
        }


        /// <summary>
        /// Remueve la referencia de padre a los hijos que tenga
        /// </summary>
        /// <param name="obj">El elemento seleccionada</param>
        private void RemoveParent(RivieraObject obj, Transaction tr)
        {
            //Cache
            obj.Parent = 0;
            //Memoria
            obj.Data.Set(FIELD_PARENT, tr, "0");
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
    }
}
