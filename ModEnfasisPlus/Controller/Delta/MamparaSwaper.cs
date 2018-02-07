using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using DaSoft.Riviera.OldModulador.Model;
using DaSoft.Riviera.OldModulador.Model.Delta;
using DaSoft.Riviera.OldModulador.Runtime;
using DaSoft.Riviera.OldModulador.UI.Delta;
using NamelessOld.Libraries.HoukagoTeaTime.Ritsu;
using NamelessOld.Libraries.HoukagoTeaTime.Runtime;
using NamelessOld.Libraries.HoukagoTeaTime.Yui;
using System;
using System.Linq;
using static DaSoft.Riviera.OldModulador.Assets.RIVIERA_CONST;
using static DaSoft.Riviera.OldModulador.Assets.Strings;
namespace DaSoft.Riviera.OldModulador.Controller.Delta
{
    public class MamparaSwaper
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
        /// Las flechas que permiten reliza la selección al usuario
        /// </summary>
        public Riviera2DArrow Arrow;
        /// <summary>
        /// El controlador activo de código y tamaño de mampara
        /// </summary>
        public Ctrl_Mampara Controller;
        /// <summary>
        /// Crea un nuevo administrador de cambios de mampara
        /// </summary>
        /// <param name="ctrl">El contralador de tamaño y código de mampara</param>
        public MamparaSwaper(Ctrl_Mampara ctrl)
        {
            this.Controller = ctrl;
            this.Size = ctrl.Size;
            this.Code = ctrl.Code;
        }
        /// <summary>
        /// Realiza la selección debe ser una mampara
        /// </summary>
        /// <returns>Verdadero si se realizá la selección</returns>
        public bool Pick(out Mampara mampara)
        {
            mampara = null;
            SelectionFilterBuilder fb = new SelectionFilterBuilder(typeof(BlockReference), typeof(Polyline));
            if (Selector.ObjectId(MSG_SEL_OBJ, out SelectedId))
            {
                //Se tiene que definir una mampara definida con la aplicación
                RivieraObject obj = App.DB[this.SelectedId];
                if (obj != null && obj is Mampara)
                {
                    mampara = obj as Mampara;
                    mampara.CheckRemate();
                }
            }
            return mampara != null;
        }

        /// <summary>
        /// Realiza el intercambio de un tipo de mampara hacia otro tipo de mampara
        /// </summary>
        public void Swap(Mampara origin)
        {
            if (origin.HasDoublePanels)
            {
                var j = origin.JointDoublePanels;
                new FastTransactionWrapper((Document doc, Transaction tr) => { j.PanelArray.Clean(j, tr); }).Run();
                j.PanelArray = null;
            }
            //Si tienen el mismo frente no es necesario mover, solo cambiar el bloque
            if (origin.Size.Split('X')[0] == this.Size.ToString().Split('X')[0])
            {
                FastTransactionWrapper trW = new FastTransactionWrapper(
               delegate (Document doc, Transaction tr)
               {
                   origin.Change(this.Size, this.Code, origin.Start, origin.End, tr);
                   RemovePanels(origin, tr);
               });
                trW.Run();
            }
            else if (origin.Size != this.Size.ToString())
            {
                ArrowDirection dir;
                double windowSize = origin.Frente;
                if (App.Riviera.Units == DaNTeUnits.Imperial)
                    windowSize = windowSize.ConvertUnits(Unit_Type.m, Unit_Type.inches);
                ZoomWindow zWin = new ZoomWindow(origin.Start.MiddlePointTo(origin.End).ToPoint3d(), windowSize, windowSize);
                zWin.SetView(1);
                Selector.Ed.Regen();
                //Revisamos si la mampara esa en un estado, inicial, final o intermedio.
                if ((origin.Parent == 0 && !origin.HasChildren) ||//Inicial sin hijos.
                    (origin.Parent != 0 && !origin.HasChildren) ||//Final sin hijos
                    (origin.Parent == 0 && origin.HasChildren) ||
                    (origin.Parent != 0 &&
                    origin.Children[FIELD_FRONT] == 0 && origin.Children[FIELD_BACK] == 0)) //Final con hijos
                {
                    dir = this.PickDirection(origin, this.Size);
                    if (dir != ArrowDirection.None)
                        JustSwap(origin, this.Size, this.Code, dir);
                }
                else//Nodo Intermedio
                {
                    dir = this.PickDirection(origin, this.Size, true);//ToDo
                    Matrix3d transform;
                    FastTransactionWrapper moveTr;
                    if (dir != ArrowDirection.None)
                    {
                        Swap(origin, this.Size, this.Code, dir, out transform);
                        //Revisamos si en la es hacia el padre o hacia el hijo
                        if (origin.Children[dir.GetString()] != 0)//Se selecciono hacia su descendencia
                        {
                            moveTr =
                                new FastTransactionWrapper(
                                    delegate (Document doc, Transaction tr)
                                    {
                                        RemovePanels(origin, tr);
                                        foreach (long childId in origin.Children.Values.Where(x => x != 0))
                                            App.DB[childId]?.Transform(transform, tr);
                                        //if (origin.BiomboId != 0)
                                        //    App.DB[origin.BiomboId]?.Transform(transform, tr);

                                    });
                            moveTr.Run();
                        }
                        else
                        {
                            moveTr = new FastTransactionWrapper(
                                delegate (Document doc, Transaction tr)
                                {
                                    RemovePanels(origin, tr);
                                    this.Move(origin.Parent, origin, transform, tr);
                                });
                            moveTr.Run();
                        }
                    }
                }
            }
            else
                Selector.Ed.WriteMessage(MSG_SAME_SIZE);
        }

        private void RemovePanels(Mampara origin, Transaction tr)
        {
            long left = origin.Children[FIELD_LEFT_FRONT],
                 right = origin.Children[FIELD_RIGHT_FRONT];
            if (left != 0 && right != 0)
            {
                App.DB[left].Delete(tr);
                App.DB[right].Delete(tr);
                //Cache
                origin.Children[FIELD_LEFT_FRONT] = 0;
                origin.Children[FIELD_RIGHT_FRONT] = 0;
                //Memoria
                origin.Data.Set(FIELD_LEFT_FRONT, tr, "0");
                origin.Data.Set(FIELD_RIGHT_FRONT, tr, "0");
            }
            if (origin.BiomboId != 0)
            {
                if (App.DB.Objects.Count(x => x.Handle.Value == origin.BiomboId) > 0)
                    App.DB[origin.BiomboId].Delete(tr);
                origin.BiomboId = 0;
                origin.Data.Set(FIELD_EXTRA, tr, origin.Extra);
            }
        }

        /// <summary>
        /// Realiza el movimiento de la mampara en dirección ascendente
        /// </summary>
        /// <param name="parent">El id del padre</param>
        /// <param name="obj">El objeto actual</param>
        /// <param name="transform">La transformada aplicar</param>
        /// <param name="tr">La transacción activa</param>
        private void Move(long parentId, RivieraObject obj, Matrix3d transform, Transaction tr)
        {
            RivieraObject parent = App.DB[parentId];
            if (parent != null)
            {
                parent.Transform(transform, tr, false);
                Move(parent.Parent, parent, transform, tr);

                foreach (long childId in parent.Children.Values.Where(x => x != 0 && x != obj.Handle.Value))
                    App.DB[childId]?.Transform(transform, tr);
            }
        }



        /// <summary>
        /// Pick a direction that decides where to move the element
        /// </summary>
        /// <param name="mampara">La mampara original</param>
        /// <param name="size">El nuevo tamaño de la mampara</param>
        /// <returns>The direction picked by the user</returns>
        private ArrowDirection PickDirection(Mampara mampara, RivieraSize size, Boolean forceShow = false)
        {
            ArrowDirection result;
            double ancho = size.Ancho;
            if (App.Riviera.Units == DaNTeUnits.Imperial)
                ancho = ancho.ConvertUnits(Unit_Type.m, Unit_Type.inches);
            Point2d insPt = mampara.Start.MiddlePointTo(mampara.End).ToPoint2dByPolar(ancho * 1.5d, mampara.Angle + Math.PI / 2);
            Riviera2DArrow arrFront = new Riviera2DArrow(insPt.ToPoint2dByPolar(ancho / 2, mampara.Start.GetVectorTo(mampara.End).Angle), ARROW_SIZE, ArrowDirection.Front),
                           arrBack = new Riviera2DArrow(insPt.ToPoint2dByPolar(ancho / 2, mampara.End.GetVectorTo(mampara.Start).Angle), ARROW_SIZE, ArrowDirection.Back);
            FastTransactionWrapper trW =
                new FastTransactionWrapper(delegate (Document doc, Transaction tr)
                {
                    if (!mampara.CollideFrom(size, ArrowDirection.Front) || forceShow)
                        arrFront.Draw(tr, mampara.Start.GetVectorTo(mampara.End).Angle);
                    if (!mampara.CollideFrom(size, ArrowDirection.Back) || forceShow)
                        arrBack.Draw(tr, mampara.End.GetVectorTo(mampara.Start).Angle);
                });
            trW.Run();
            Selector.Ed.Regen();
            ObjectId id;
            if (arrFront.Id.Count > 0 && arrBack.Id.Count == 0)
                result = ArrowDirection.Front;
            else if (arrFront.Id.Count == 0 && arrBack.Id.Count > 0)
                result = ArrowDirection.Back;
            else if (Selector.ObjectId<Polyline>(SEL_ARROW_DIR, out id))
            {
                if (arrFront.Id.Contains(id))
                    result = ArrowDirection.Front;
                else if (arrBack.Id.Contains(id))
                    result = ArrowDirection.Back;
                else
                    result = ArrowDirection.None;
            }
            else
                result = ArrowDirection.None;
            trW =
            new FastTransactionWrapper(delegate (Document doc, Transaction tr)
            {
                if (arrFront != null)
                    arrFront.Erase(tr);
                if (arrBack != null)
                    arrBack.Erase(tr);
            });
            trW.Run();
            return result;
        }

        /// <summary>
        /// Se realiza el intercamnio de entidad sin que importe de este
        /// el estado de la mampara.
        /// </summary>
        /// <param name="mampara">La mampara original</param>
        /// <param name="size">El nuevo tamaño de la mampara</param>
        /// <param name="code">El nuevo código de la mampara</param>
        private void JustSwap(Mampara mampara, RivieraSize size, string code, ArrowDirection dir)
        {
            FastTransactionWrapper trW = new FastTransactionWrapper(
                delegate (Document doc, Transaction tr)
            {
                RemovePanels(mampara, tr);
                Point2d mvPt;
                Double frente = size.Frente;

                if (App.Riviera.Units == DaNTeUnits.Imperial)
                    frente = frente.ConvertUnits(Unit_Type.m, Unit_Type.inches);

                if (dir == ArrowDirection.Front)
                {
                    mvPt = mampara.Start.ToPoint2dByPolar(frente, mampara.Angle);
                    mampara.Change(size, code, mampara.Start, mvPt, tr);
                }
                else
                {
                    mvPt = mampara.End.ToPoint2dByPolar(frente, mampara.Angle + Math.PI);
                    mampara.Change(size, code, mvPt, mampara.End, tr);
                }

            });
            trW.Run();
        }
        /// <summary>
        /// Se realiza el intercamnio de entidad sin que importe de este
        /// el estado de la mampara.
        /// </summary>
        /// <param name="mampara">La mampara original</param>
        /// <param name="size">El nuevo tamaño de la mampara</param>
        /// <param name="code">El nuevo código de la mampara</param>
        private void Swap(Mampara mampara, RivieraSize size, string code, ArrowDirection dir, out Matrix3d transform)
        {
            transform = Matrix3d.Displacement(new Vector3d(0, 0, 0));
            BlankTransactionWrapper<Matrix3d> trW = new BlankTransactionWrapper<Matrix3d>(
            delegate (Document doc, Transaction tr)
            {
                Matrix3d t;
                Point2d mvPt;
                double frente = size.Frente;
                if (App.Riviera.Units == DaNTeUnits.Imperial)
                    frente = frente.ConvertUnits(Unit_Type.m, Unit_Type.inches);
                if (dir == ArrowDirection.Front)
                {
                    mvPt = mampara.Start.ToPoint2dByPolar(frente, mampara.Angle);
                    t = Matrix3d.Displacement(mampara.End.ToPoint3d().GetVectorTo(mvPt.ToPoint3d()));
                    mampara.Change(size, code, mampara.Start, mvPt, tr);
                }
                else
                {
                    mvPt = mampara.End.ToPoint2dByPolar(frente, mampara.Angle + Math.PI);
                    t = Matrix3d.Displacement(mampara.Start.ToPoint3d().GetVectorTo(mvPt.ToPoint3d()));
                    mampara.Change(size, code, mvPt, mampara.End, tr);
                }
                return t;
            });
            transform = trW.Run();
        }
    }
}
