using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using DaSoft.Riviera.OldModulador.Model;
using DaSoft.Riviera.OldModulador.Model.Delta;
using DaSoft.Riviera.OldModulador.Runtime;
using NamelessOld.Libraries.HoukagoTeaTime.Ritsu;
using System;
using System.Collections.Generic;
using System.Linq;
using static DaSoft.Riviera.OldModulador.Assets.RIVIERA_CODE;
using static DaSoft.Riviera.OldModulador.Assets.RIVIERA_CONST;
using static DaSoft.Riviera.OldModulador.Assets.Strings;
namespace DaSoft.Riviera.OldModulador.Controller.Delta
{
    public class MamparaValidator
    {
        /// <summary>
        /// La colección de mamparas que tienen remate
        /// </summary>
        public IEnumerable<Mampara> MamparaWithRemate;
        /// <summary>
        /// La colección de mamparas que no tienen remate pero pueden 
        /// tenerlo
        /// </summary>
        public IEnumerable<Mampara> MamparaWithoutRemate;
        /// <summary>
        /// La colección de mamparas que no tienen remate pero pueden 
        /// tenerlo
        /// </summary>
        public IEnumerable<MamparaJoint> MamparaJoints;
        /// <summary>
        /// Ejecuta el proceso de validar que elementos necesitan remates finales y que remates deben ser removidos
        /// </summary>
        public MamparaValidator()
        {
            IEnumerable<Mampara> mamparas = App.DB.Objects.Where(x => x is Mampara).Select<RivieraObject, Mampara>(y => y as Mampara);
            this.MamparaWithRemate = mamparas.Where(x => x.Remates != null);
            //Los candidatos para remate son aquellos que no tienen padre o no
            //tienen hijos
            this.MamparaWithoutRemate = mamparas.Where(x => !(
            (x.Parent != 0 && x.Children[FIELD_BACK] == 0 && x.Children[FIELD_FRONT] != 0) ||
            (x.Parent != 0 && x.Children[FIELD_BACK] != 0 && x.Children[FIELD_FRONT] == 0)));

            //La lista de Mampara Joints
            this.MamparaJoints = App.DB.Objects.Where(x => x is MamparaJoint).Select<RivieraObject, MamparaJoint>(y => y as MamparaJoint);
        }
        /// <summary>
        /// Se realiza la validación
        /// </summary>
        public void Validate(Transaction tr)
        {
            ValidateJoints(tr);
            ValidateRemates(tr);
        }

        private void ValidateJoints(Transaction tr)
        {
            MamparaJoint[] coll = this.MamparaJoints.ToArray();
            foreach (MamparaJoint joint in coll)
                joint.UpdateTag(tr);
        }

        private void ValidateRemates(Transaction tr)
        {
            Mampara[] coll = this.MamparaWithoutRemate.ToArray();
            //1: Se dibujan los remates
            foreach (Mampara mampara in coll)
            {
                ValidateRemate(tr, mampara);
            }
        }

        public static void ValidateRemate(Transaction tr, Mampara mampara)
        {
            MamparaSize size;
            RivieraSize rSize;
            String code;
            ArrowDirection dir;
            size = App.DB.GetSize(mampara.Size);
            code = String.Format("{0}{1}", DT_REMATE_FINAL, size.Nominal.Alto);
            rSize = size.Real.ConvertUnits(Unit_Type.mm, Unit_Type.m);
            rSize.Frente = REMATE_FRENTE_M;
            rSize.Ancho = ANCHO_M;
            if (mampara.Parent == 0 && mampara.Children[FIELD_FRONT] == 0 && mampara.Children[FIELD_BACK] == 0)
            {
                MamparaRemateFinal[] remates
                    = new MamparaRemateFinal[]
                    {
                            AddRemate(tr, mampara, rSize, code, out dir, ArrowDirection.Front),
                            AddRemate(tr, mampara, rSize, code, out dir, ArrowDirection.Back)
                    };
                //Cache
                mampara.Children[FIELD_FRONT] = remates[0].Handle.Value;
                mampara.Children[FIELD_BACK] = remates[1].Handle.Value;
                //Memoria
                mampara.Data.Set(FIELD_FRONT, tr, remates[0].Handle.Value.ToString());
                mampara.Data.Set(FIELD_BACK, tr, remates[1].Handle.Value.ToString());

            }
            else
            {
                MamparaRemateFinal remate;
                remate = AddRemate(tr, mampara, rSize, code, out dir);
                if (remate != null && dir == ArrowDirection.Front)
                {
                    //Cache
                    mampara.Children[FIELD_FRONT] = remate.Handle.Value;
                    //Memoria
                    mampara.Data.Set(FIELD_FRONT, tr, remate.Handle.Value.ToString());
                }
                else if (remate != null && dir == ArrowDirection.Back)
                {
                    //Cache
                    mampara.Children[FIELD_BACK] = remate.Handle.Value;
                    //Memoria
                    mampara.Data.Set(FIELD_BACK, tr, remate.Handle.Value.ToString());
                }
            }
        }

        public static MamparaRemateFinal AddRemate(Transaction tr, Mampara mampara, RivieraSize rSize, string code,
            out ArrowDirection dir, ArrowDirection dftlDir = ArrowDirection.None)
        {
            MamparaRemateFinal remate;
            Point2d pt0, ptf;
            if (dftlDir == ArrowDirection.None)
                SelectPoints(mampara, out pt0, out ptf, out dir);
            else
            {
                dir = dftlDir;
                if (dir == ArrowDirection.Front)
                {
                    pt0 = mampara.End;
                    ptf = pt0.ToPoint2dByPolar(REMATE_FRENTE_M, mampara.Angle);
                }
                else
                {
                    pt0 = mampara.Start;
                    ptf = pt0.ToPoint2dByPolar(REMATE_FRENTE_M, mampara.Angle + Math.PI);
                }
            }

            if ((dir == ArrowDirection.Front && mampara.Children[FIELD_FRONT] == 0) ||
                (dir == ArrowDirection.Back && mampara.Children[FIELD_BACK] == 0))
            {

                remate = new MamparaRemateFinal(mampara, pt0, ptf, rSize, code);
                remate.Draw(tr, null);
                remate.Parent = mampara.Handle.Value;
                remate.Save(tr);
                App.DB.Objects.Add(remate);
            }
            else
                remate = null;
            return remate;
        }

        static void SelectPoints(Mampara mampara, out Point2d pt0, out Point2d ptf, out ArrowDirection dir)
        {
            //Inicial
            if (mampara.Parent == 0)
            {
                //Si no hay nada conectado ponemos en la parte trasera el remate
                if ((mampara.Children[FIELD_FRONT] == 0 && mampara.Children[FIELD_BACK] == 0) ||
                    (mampara.Children[FIELD_FRONT] != 0 && mampara.Children[FIELD_BACK] == 0))
                {
                    pt0 = mampara.Start;
                    ptf = pt0.ToPoint2dByPolar(REMATE_FRENTE_M, mampara.Angle + Math.PI);
                    dir = ArrowDirection.Back;
                }
                else
                {
                    pt0 = mampara.End;
                    ptf = pt0.ToPoint2dByPolar(REMATE_FRENTE_M, mampara.Angle);
                    dir = ArrowDirection.Front;
                }
            }
            else//Final
            {
                pt0 = mampara.End;
                ptf = pt0.ToPoint2dByPolar(REMATE_FRENTE_M, mampara.Angle);
                dir = ArrowDirection.Front;
            }
        }
    }
}
