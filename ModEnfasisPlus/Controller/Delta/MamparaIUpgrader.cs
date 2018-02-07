using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using DaSoft.Riviera.OldModulador.Model;
using DaSoft.Riviera.OldModulador.Model.Delta;
using DaSoft.Riviera.OldModulador.Runtime;
using NamelessOld.Libraries.HoukagoTeaTime.Ritsu;
using NamelessOld.Libraries.HoukagoTeaTime.Runtime;
using NamelessOld.Libraries.HoukagoTeaTime.Yui;
using System;
using System.Collections.Generic;
using System.Linq;
using static DaSoft.Riviera.OldModulador.Assets.RIVIERA_CONST;
using static DaSoft.Riviera.OldModulador.Assets.Strings;
namespace DaSoft.Riviera.OldModulador.Controller.Delta
{
    public class MamparaIUpgrader
    {
        public Mampara Mampara;
        public JointObject Joint;

        public Boolean IsIMampara
        {
            get { return Mampara != null && Joint != null; }
        }

        public MamparaIUpgrader(Mampara mampara)
        {
            this.Mampara = mampara;
            RivieraObject obj = App.DB[this.Mampara.Parent];
            if (obj != null && obj.GetType().Name == typeof(JointObject).Name)
                this.Joint = obj as JointObject;
            else
            {
                IEnumerable<RivieraObject> objs = this.Mampara.Children.Values.Where(x => x != 0).Select<long, RivieraObject>(y => App.DB[y]);
                foreach (RivieraObject o in objs)
                    if (o.GetType().Name == typeof(JointObject).Name)
                    {
                        this.Joint = o as JointObject;
                        break;
                    }
            }
        }

        public void ShowDirections(Transaction tr, out Riviera2DArrow left, out Riviera2DArrow right)
        {
            Double ang = this.Joint.Angle + Math.PI / 2, r = ANCHO_M;

            if (App.Riviera.Units == DaNTeUnits.Imperial)
                r = r.ConvertUnits(Unit_Type.m, Unit_Type.inches);

            Point2d top = this.Joint.Start.ToPoint2dByPolar(r, ang),
                    lft = top.ToPoint2dByPolar(r, this.Joint.Angle + Math.PI),
                    rgt = top.ToPoint2dByPolar(r, this.Joint.Angle);
            left = new Riviera2DArrow(lft, ARROW_SIZE, ArrowDirection.Left_Front);
            right = new Riviera2DArrow(rgt, ARROW_SIZE, ArrowDirection.Right_Front);

            right.Draw(tr, this.Joint.Start.GetVectorTo(this.Joint.End).Angle);
            left.Draw(tr, this.Joint.End.GetVectorTo(this.Joint.Start).Angle);

        }
        /// <summary>
        /// Realiza el proceso de inserción de la I
        /// </summary>
        public void UpgradeIToT()
        {
            Riviera2DArrow left = null, right = null;
            FastTransactionWrapper trWDir =
               new FastTransactionWrapper(delegate (Document doc, Transaction tr)
               {
                   this.ShowDirections(tr, out left, out right);
               });
            trWDir.Run();
            Selector.Ed.Regen();
            //Realizá el proceso de cambiar un joint a una mampara joint
            this.UpgradeJoint(left, right);
        }

        public void InsertT(RivieraSize size, string code)
        {
            FastTransactionWrapper trWDir =
                new FastTransactionWrapper(delegate (Document doc, Transaction tr)
                {
                    this.Joint.ShowDirections(tr);
                    this.Joint.RegenArrows(tr);
                });
            trWDir.Run();
            Selector.Ed.Regen();

            FastTransactionWrapper trW =
               new FastTransactionWrapper(delegate (Document doc, Transaction tr)
               {
                   try
                   {

                       ArrowDirection dir = this.Joint.PickDirection(tr);
                       if (dir == ArrowDirection.None)
                           dir = ArrowDirection.Left_Front;
                       this.AddMampara(tr, dir, size, code);
                   }
                   catch (Exception exc)
                   {
                       Selector.Ed.WriteMessage(exc.Message);
                   }

               });
            trW.Run();
        }

        private void UpgradeJoint(Riviera2DArrow left, Riviera2DArrow right)
        {
            FastTransactionWrapper trW =
                new FastTransactionWrapper(delegate (Document doc, Transaction tr)
                {
                    try
                    {

                        ArrowDirection dir = PickDirection(left, right);
                        if (dir == ArrowDirection.Left_Front)
                            this.Joint = this.Joint.Upgrade(tr, ArrowDirection.Back);
                        else if (dir == ArrowDirection.Right_Front)
                            this.Joint = this.Joint.Upgrade(tr, ArrowDirection.Front);
                    }
                    catch (Exception exc)
                    {
                        Selector.Ed.WriteMessage(exc.Message);
                    }
                    if (left != null)
                        left.Erase(tr);
                    if (right != null)
                        right.Erase(tr);

                });
            trW.Run();
        }

        private ArrowDirection PickDirection(Riviera2DArrow left, Riviera2DArrow right)
        {
            ArrowDirection dir = ArrowDirection.None;
            ObjectId aId;
            if (Selector.ObjectId<Polyline>(SEL_ARROW_DIR, out aId))
                dir = aId == left.Id[0] ? ArrowDirection.Left_Front : aId == right.Id[0] ? ArrowDirection.Right_Front : ArrowDirection.None;
            return dir;
        }

        /// <summary>
        /// Se agrega una nueva mampara apartir de la union
        /// </summary>
        /// <param name="tr">La transacción activa</param>
        /// <param name="dir">La dirección es la misma a la de la mampara</param>
        /// <param name="start">El punto inicial de la mampara</param>
        /// <param name="ang">El angulo de dirección</param>
        private void AddMampara(Transaction tr, ArrowDirection dir, RivieraSize size, string code)
        {
            Mampara tmp = new Mampara(new Point2d(0, 0), new Point2d(10, 0), size, code);
            Double ang;
            Point3d pt = this.Joint.NextPoint(dir, out ang),
                    end = pt.ToPoint2d().ToPoint2dByPolar(tmp.Frente, ang).ToPoint3d();
            this.Mampara = new Mampara(pt.ToPoint2d(), end.ToPoint2d(), size, code);
            this.Mampara.Draw(tr, null);
            this.Mampara.Parent = this.Joint.Handle.Value;
            this.Joint.AddChildren(dir, this.Mampara.Handle, tr);
            this.Mampara.Save(tr);
            Selector.Ed.WriteMessage(MSG_MAM_ADD, code, dir.ToHumanReadable());
            PanelBuilder.AddPaneles(this.Mampara);
            App.DB.Objects.Add(this.Mampara);
            (this.Joint as MamparaJoint).UpdateTag(tr);
            Point2d min, max;
            PointExtender.GeometricExtents(out min, out max, this.Joint.GeometricExtents, this.Mampara.GeometricExtents);
            new ZoomWindow(min, max).SetView(ZOOM);
        }



    }
}
