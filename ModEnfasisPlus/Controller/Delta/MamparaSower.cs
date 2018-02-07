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
    public class MamparaSower
    {
        /// <summary>
        /// La mampara actual
        /// </summary>
        public Mampara Mampara;
        /// <summary>
        /// La articulación activa
        /// </summary>
        public JointObject Joint;
        /// <summary>
        /// El tamaño actual de la mampara
        /// </summary>
        RivieraSize Size;
        /// <summary>
        /// El código de la mampara a insertar
        /// </summary>
        String Code;
        /// <summary>
        /// La dirección actual
        /// </summary>
        ArrowDirection Direction;
        /// <summary>
        /// El administrador de la transacción
        /// </summary>
        BlankTransactionWrapper<ArrowDirection> TrW;
        /// <summary>
        /// El controlador activo de código y tamaño de mampara
        /// </summary>
        public Ctrl_Mampara Controller;

        /// <summary>
        /// Esta clasa se encarga de sembrar mamparas
        /// </summary>
        /// <param name="ctrl">El contralador de tamaño y código de mampara</param>
        /// <param name="mampara">La mampara base para sembrar los demas elementos</param>
        public MamparaSower(Ctrl_Mampara ctrl, Mampara mampara)
        {
            this.Controller = ctrl;
            this.Size = ctrl.Size;
            this.Code = ctrl.Code;
            this.Mampara = mampara;
            this.Joint = null;
        }
        /// <summary>
        /// Inicia el proceso de sembrado
        /// </summary>
        public void Sow()
        {
            do
            {
                TrW = new BlankTransactionWrapper<ArrowDirection>(SowTransaction);
                try
                {
                    this.Direction = TrW.Run();
                }
                catch (Exception exc)
                {
                    Selector.Ed.WriteMessage("\n{0}", exc.Message);
                    new FastTransactionWrapper(CleanMessTransaction).Run();
                }
            } while (this.Direction != ArrowDirection.None);
            //Fast Transaction
            FastTransactionWrapper fT = new FastTransactionWrapper(
                delegate (Document doc, Transaction tr)
                {
                    MamparaValidator val = new MamparaValidator();
                    val.Validate(tr);
                });
            fT.Run();
        }
        /// <summary>
        /// Se encarga de elimar los últimos cambios que generaron el error
        /// </summary>
        /// <param name="doc">El documento actual</param>
        /// <param name="tr">La transacción activa de AutoCAD</param>
        private void CleanMessTransaction(Document doc, Transaction tr)
        {
            //Borra las flechas
            this.EraseArrows(tr, this.Mampara, this.Joint);
            if (this.Mampara != null)
                this.Mampara.Delete(tr);
            else if (this.Joint != null)
                this.Joint.Delete(tr);
        }

        /// <summary>
        /// La transacción que realiza el sembrado de las mamparas
        /// </summary>
        /// <param name="doc">El documento activo de AutoCAD</param>
        /// <param name="tr">La transacción activa de AutoCAD</param>
        /// <returns>La dirección para la siguiente mampara</returns>
        private ArrowDirection SowTransaction(Document doc, Transaction tr)
        {
            SowCase opt;
            ArrowDirection dir = PickDirection(tr, this.Mampara, this.Joint, out opt);
            this.Size = this.Controller.Size;
            this.Code = this.Controller.Code;
            switch (opt)
            {
                case SowCase.Mampara_Joint:
                    SowJointMampara(tr, dir);
                    break;
                case SowCase.MamparaFromJoint:
                    SowMampara(tr, dir);
                    break;
            }
            return dir;
        }
        /// <summary>
        /// Se realiza el sembrado de la segunda mampara
        /// </summary>
        /// <param name="dir">La dirección en la que se realizará el sembrado de la segunda mampara</param>
        private void SowJointMampara(Transaction tr, ArrowDirection dir)
        {
            if (dir == ArrowDirection.None)
                return;
            //Dirección frontal o trasero se inserta a hueso.
            //Se crean dos elementos una articulación y una nueva mampara con la dirección seleccionada.
            //Para otro caso se realiza la inserción de la mampara y la articulación mampara Joint, esta articulación
            //es similar a un poste
            Double ang = this.Mampara.Direction.Angle, jointAngle;
            jointAngle = ang;
            //1: Calculo de punto de inserción
            Point2d insPt;
            Point3d startPt;
            //Se inserta en el punto final si la dirección es frontal
            if (new ArrowDirection[] { ArrowDirection.Front, ArrowDirection.Left_Front, ArrowDirection.Right_Front }.Contains(dir))
                insPt = this.Mampara.End;
            else
                insPt = this.Mampara.Start;
            //2: Creación de la articulación
            if (dir == ArrowDirection.Front)
                this.Joint = new JointObject(insPt, ang);
            else if (dir == ArrowDirection.Back)
                this.Joint = new JointObject(insPt, ang + Math.PI);
            else if (new ArrowDirection[] { ArrowDirection.Front, ArrowDirection.Left_Front, ArrowDirection.Right_Front }.Contains(dir))
                this.Joint = new MamparaJoint(insPt, ang);
            else
            {
                this.Joint = new MamparaJoint(insPt, ang + Math.PI);
                jointAngle = ang + Math.PI;
            }
            //3: Se agregá la articulación
            startPt = this.AddJoint(tr, dir, jointAngle);
            //4: Se inserta la mampara
            this.AddMampara(tr, dir, startPt.ToPoint2d(), ang);
            //5: Agrego los nuevos elementos al proyecto
            App.DB.Objects.Add(this.Joint);
            App.DB.Objects.Add(this.Mampara);
            //6: Se muestran direcciones de mampara y articulación
            this.Mampara.ShowDirections(tr);
            if (Joint is MamparaJoint)
            {
                this.Joint.ShowDirections(tr);
                if (this.Joint is MamparaJoint)
                    (this.Joint as MamparaJoint).UpdateTag(tr);
            }
            else
                this.Joint = null;
            this.Zoom();
        }
        /// <summary>
        /// Se realiza el sembrado de la segunda mampara
        /// </summary>
        /// <param name="dir">La dirección en la que se realizará el sembrado de la segunda mampara</param>
        private void SowMampara(Transaction tr, ArrowDirection dir)
        {
            //Solo se inserta una mampara apartir de una articulación
            Double ang = this.Joint.DirectionAngle(dir);
            //1: Calculo de punto de inserción de la mampara
            Point3d startPt;
            startPt = this.Joint.NextPoint(dir, out ang);
            //2: Se inserta la mampara
            this.AddMampara(tr, dir, startPt.ToPoint2d(), ang);
            //4: Agrego los nuevos elementos al proyecto
            App.DB.Objects.Add(this.Mampara);
            //6: Se muestran direcciones de mampara y articulación
            this.Mampara.ShowDirections(tr, dir);
            if (Joint.Type != JointType.Joint_X && Joint.Type != JointType.None)
            {
                if (this.Joint is MamparaJoint)
                    (this.Joint as MamparaJoint).UpdateTag(tr);
                this.Joint.ShowDirections(tr);
            }
            else
            {
                if (this.Joint != null && this.Joint is MamparaJoint)
                    (this.Joint as MamparaJoint).UpdateTag(tr);
                this.Joint = null;
            }
            this.Zoom();
        }

        private void Zoom()
        {
            if (this.Joint != null)
            {
                Point2d min, max;
                PointExtender.GeometricExtents(out min, out max, this.Joint.GeometricExtents, this.Mampara.GeometricExtents);
                new ZoomWindow(min, max).SetView(ZOOM);
            }
            else
            {
                Point2d[] geoExt = this.Mampara.GeometricExtents;
                new ZoomWindow(geoExt[0], geoExt[1]).SetView(ZOOM);
            }
        }

        /// <summary>
        /// Se agrega una nueva mampara apartir de otra mampara
        /// </summary>
        /// <param name="tr">La transacción activa</param>
        /// <param name="dir">La dirección es la misma a la de la mampara</param>
        /// <param name="start">El punto inicial de la mampara</param>
        /// <param name="ang">El angulo de dirección</param>
        private void AddMampara(Transaction tr, ArrowDirection dir, Point2d start, double ang)
        {
            ang = this.Joint.DirectionAngle(dir);
            Point2d end = start.ToPoint2dByPolar(this.Mampara.Frente, ang);
            this.Mampara = new Mampara(start, end, this.Size, this.Code);
            this.Mampara.Draw(tr, null);
            this.Mampara.Parent = this.Joint.Handle.Value;
            this.Joint.AddChildren(dir, this.Mampara.Handle, tr);
            this.Mampara.Save(tr);
            Selector.Ed.WriteMessage(MSG_MAM_ADD, this.Code, dir.ToHumanReadable());
            PanelBuilder.AddPaneles(this.Mampara);
        }
        /// <summary>
        /// Se agregá una articulació apartir de una mampara
        /// </summary>
        /// <param name="tr">La transacción activa</param>
        /// <param name="dir">La dirección es la misma a la de la mampara</param>
        /// <param name="ang">El angulo de dirección</param>
        /// <returns>El punto inicial de la mampara</returns>
        private Point3d AddJoint(Transaction tr, ArrowDirection dir, Double ang)
        {
            this.Joint.Draw(tr, null);
            this.Joint.Parent = this.Mampara.Handle.Value;
            //La articulación siempre esta atras o al frente
            ArrowDirection jointDir = dir != ArrowDirection.None ? new ArrowDirection[] { ArrowDirection.Front, ArrowDirection.Left_Front, ArrowDirection.Right_Front }.Contains(dir) ? ArrowDirection.Front : ArrowDirection.Back : ArrowDirection.None;
            this.Mampara.AddChildren(jointDir, this.Joint.Line.Handle, tr);
            this.Joint.Save(tr);
            return this.Joint.NextPoint(dir, out ang);
        }

        /// <summary>
        /// Selecciona una dirección que permite determinar el proximo elemento en el sembrado
        /// </summary>
        /// <param name="tr">La transacción activa</param>
        /// <param name="mampara">La mampara seleccionada</param>
        /// <param name="joint">La articulación activa</param>
        /// <param name="opt">La opción actual de selección</param>
        /// <returns>La dirección actual</returns>
        private ArrowDirection PickDirection(Transaction tr, Mampara mampara, JointObject joint, out SowCase opt)
        {
            ObjectId aId;
            Riviera2DArrow arr;
            opt = SowCase.None;
            ArrowDirection dir = ArrowDirection.None;
            if (mampara != null && joint == null)//Se seleccionan solo líneas de la mampara
            {
                opt = SowCase.Mampara_Joint;
                mampara.RegenArrows(tr);
                if (mampara.Parent != 0)
                {
                    //Hide I
                    if (App.DB[mampara.Parent]?.Children[DaSoft.Riviera.OldModulador.Assets.Strings.FIELD_BACK] == 0)
                    {
                        mampara.Arrows[3]?.Erase(tr);
                        mampara.Arrows[4]?.Erase(tr);
                    }
                    else if (App.DB[mampara.Parent]?.Children[DaSoft.Riviera.OldModulador.Assets.Strings.FIELD_FRONT] == 0)
                    {
                        mampara.Arrows[0]?.Erase(tr);
                        mampara.Arrows[1]?.Erase(tr);
                    }
                }
                else
                {
                    if (mampara.Children[FIELD_FRONT] != 0)
                    {
                        mampara.Arrows[0]?.Erase(tr);
                        mampara.Arrows[1]?.Erase(tr);
                    }
                    else if (mampara.Children[FIELD_BACK] != 0)
                    {
                        mampara.Arrows[3]?.Erase(tr);
                        mampara.Arrows[4]?.Erase(tr);
                    }
                }
                Selector.Ed.Regen();
                if (mampara.Arrows == null)
                    dir = ArrowDirection.None;
                else
                    dir = mampara.PickDirection(tr);

            }
            else if (mampara != null && joint != null)//Se pueden seleccionar líneas de mampara o Joint
            {
                mampara.RegenArrows(tr);
                //Hide I
                if (App.DB[mampara.Parent]?.Children[DaSoft.Riviera.OldModulador.Assets.Strings.FIELD_BACK] == 0)
                {
                    mampara.Arrows[3]?.Erase(tr);
                    mampara.Arrows[4]?.Erase(tr);
                }
                else if (App.DB[mampara.Parent]?.Children[DaSoft.Riviera.OldModulador.Assets.Strings.FIELD_FRONT] == 0)
                {
                    mampara.Arrows[0]?.Erase(tr);
                    mampara.Arrows[1]?.Erase(tr);
                }
                joint.RegenArrows(tr);
                Selector.Ed.Regen();
                if (Selector.ObjectId<Polyline>(SEL_ARROW_DIR, out aId))
                {
                    //Primero se revisa si se escogio una flecha de mampara
                    arr = mampara != null && mampara.Arrows != null ? mampara.Arrows.Where(y => y != null).Where(x => x.Id.Count > 0 && x.Id[0] == aId).FirstOrDefault() : null;
                    if (arr != null)
                    {
                        opt = SowCase.Mampara_Joint;
                        dir = arr.Direction;
                    }
                    else
                    {
                        //Se revisa si se escogio una flecha de articulación
                        arr = joint != null && joint.Arrows != null ? joint.Arrows.Where(y => y != null).Where(x => x.Id.Count > 0 && x.Id[0] == aId).FirstOrDefault() : null;
                        if (arr != null)
                        {
                            opt = SowCase.MamparaFromJoint;
                            dir = arr.Direction;
                        }
                    }
                }
            }
            //Borra las flechas
            this.EraseArrows(tr, mampara, joint);
            return dir;
        }

        private void EraseArrows(Transaction tr, Mampara mampara, JointObject joint)
        {
            //Elimina las mamparas dibujadas
            if (mampara != null && mampara.Arrows != null)
            {
                foreach (Riviera2DArrow a in mampara.Arrows.Where(x => x != null))
                    a.Erase(tr);
                mampara.Arrows = null;
            }
            if (joint != null && joint.Arrows != null)
            {
                foreach (Riviera2DArrow a in joint.Arrows.Where(x => x != null))
                    a.Erase(tr);
                joint.Arrows = null;
            }
        }

        /// <summary>
        /// Realiza el proceso de inserción de una mampara proporcionando un
        /// tamaño y un código
        /// </summary>
        /// <param name="mam_size">El tamaño de la mampara</param>
        /// <param name="mam_code">El código de la mampara</param>
        /// <param name="mam">Como parámetro de salida se tiene la mampara insertada</param>
        public static Boolean InsertMampara(RivieraSize mam_size, String mam_code, out Mampara mam)
        {
            Point3d pt0, ptf;
            Boolean flag = false;
            mam = null;
            if (Selector.Point("Selecciona un punto", out pt0) && Selector.Point("Selecciona un punto", pt0, out ptf))
            {
                TransactionWrapper<Object, Mampara> trW = new TransactionWrapper<Object, Mampara>(delegate (Document doc, Transaction tr, object[] data)
                {
                    Point2d start = (Point2d)data[0];
                    Point2d end = (Point2d)data[1];
                    RivieraSize size = (RivieraSize)data[2];
                    String code = (String)data[3];
                    Mampara mampara = new Mampara(start, end, size, code);
                    mampara.Draw(tr, null);
                    mampara.Save(tr);
                    mampara.ShowDirections(tr);
                    Point2d[] geoExt = mampara.GeometricExtents;
                    new ZoomWindow(geoExt[0], geoExt[1]).SetView(1.5d);
                    App.DB.Objects.Add(mampara);
                    return mampara;
                });
                mam = trW.Run(pt0.ToPoint2d(), ptf.ToPoint2d(), mam_size, mam_code);
                PanelBuilder.AddPaneles(mam);
                flag = true;
            }
            return flag;
        }
    }

}

