using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using DaSoft.Riviera.OldModulador.Model;
using DaSoft.Riviera.OldModulador.Model.Delta;
using DaSoft.Riviera.OldModulador.Runtime;
using DaSoft.Riviera.OldModulador.Runtime.Delta;
using NamelessOld.Libraries.HoukagoTeaTime.Ritsu;
using NamelessOld.Libraries.Yggdrasil.Lain;
using NamelessOld.Libraries.Yggdrasil.Lilith;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static DaSoft.Riviera.OldModulador.Assets.RIVIERA_CODE;
namespace DaSoft.Riviera.OldModulador.Controller.Delta
{
    public class MamparaRematesDrawer : NamelessObject
    {
        /// <summary>
        /// La mampara seleccionada
        /// </summary>
        public JointObject Joint;
        /// <summary>
        /// El alto de la columna en valor nominal
        /// </summary>
        public int AltoColumna;
        /// <summary>
        /// Devuelve el tipo de unión
        /// </summary>
        /// <value>
        /// La unión seleccionada
        /// </value>
        public JointType Union
        {
            get { return this.Joint.Type; }
        }
        /// <summary>
        /// Inicializa una instancia de la clase<see cref="MamparaRematesDrawer"/>.
        /// </summary>
        /// <param name="joint">La unión de la mampara</param>
        public MamparaRematesDrawer(JointObject joint)
        {
            this.Joint = joint;
        }

        /// <summary>
        /// Realiza la selección del remate de la mampara
        /// </summary>
        /// <param name="remateColl">La colleción de remates</param>
        /// <returns>El nombre del remate seleccionado</returns>
        public void SelectRemate(ref List<RemateGeometry> remateColl)
        {
            try
            {
                //Se realiza la selección de las mamparas involucradas
                IEnumerable<Mampara> mamparas = this.GetMamparas();
                if (this.Union == JointType.Joint_I)//Solo en cambios de altura
                {
                    RemateGeometry r = RemateForUnionTypeI(mamparas);
                    if (r != null)
                        remateColl.Add(r);
                }
                else if (this.Union == JointType.Joint_L)
                {
                    RemateGeometry r = RemateForUnionTypeL(mamparas);
                    if (r != null)
                        remateColl.Add(r);
                }
                else if (this.Union == JointType.Joint_T)
                    RemateForUnionTypeT(ref remateColl, mamparas);
                else if (this.Union == JointType.Joint_X)
                    RemateForUnionTypeX(ref remateColl, mamparas);
            }
            catch (Exception exc)
            {
                throw new DeltaException(String.Format("Unión {2}: ({0}), Location: {1}\n{3}", this.Joint.Handle.Value, this.Joint.Start, this.Union.GetLetter(), exc.Message));
            }
        }
        /// <summary>
        /// Realiza el proceso de validación para una unión en I
        /// </summary>
        /// <param name="mamparas">La mampara seleccionada</param>
        /// <returns>El remate para el tipo de unión actual</returns>
        private RemateGeometry RemateForUnionTypeI(IEnumerable<Mampara> mamparas)
        {
            try
            {
                Mampara first = mamparas.FirstOrDefault(),
                        last = mamparas.LastOrDefault();
                if (first != null && last != null)
                {
                    int h1 = this.ParseHeight(first),
                        h2 = this.ParseHeight(last),
                        h;
                    String code;
                    if (h1 != h2)
                    {
                        h = (int)Math.Abs(h1 - h2);
                        code = DT_REMATE_FINAL_D_HEIGHT;
                        this.AltoColumna = h1 < h2 ? h1 : h2;
                        return new RemateGeometry(this.Joint, String.Format("{0}{1:00}", code, h), this.Joint.Start.ToPoint3d(), h1 < h2 ? h1 : h2);
                    }
                    else
                    {
                        this.AltoColumna = h1;
                        return null;
                    }
                }
                else
                    throw new DeltaException("La unión no tiene las mamparas necesarias, para poder seleccionar el remate");
            }
            catch (Exception exc)
            {
                App.Riviera.Log.AppendEntry(String.Format("Error al generar el remate para unión en I, union: {0}", this.Joint.Handle), Protocol.Error, this);
                throw exc;
            }
        }
        /// <summary>
        /// Realiza el proceso de validación para una unión en L
        /// </summary>
        /// <param name="mamparas">La mampara seleccionada</param>
        /// <returns>El remate para el tipo de unión actual</returns>
        private RemateGeometry RemateForUnionTypeL(IEnumerable<Mampara> mamparas)
        {
            try
            {
                Mampara first = mamparas.FirstOrDefault(),
                        last = mamparas.LastOrDefault();
                if (first != null && last != null)
                {
                    int h1 = this.ParseHeight(first),
                    h2 = this.ParseHeight(last),
                    h;
                    Point2d center = this.Joint.Start.MiddlePointTo((this.Joint as MamparaJoint).GetEndPoint()),
                            insPt;
                    String code;
                    Double angle;
                    if (h1 != h2)
                    {
                        h = (int)Math.Abs(h1 - h2);
                        this.AltoColumna = h1 < h2 ? h1 : h2;
                        code = DT_REMATE_FINAL_D_HEIGHT;
                        if (h1 > h2)
                        {
                            this.FindLocation(new Tuple<int, Mampara>(h1, first), center, out insPt, out angle);
                            return new RemateGeometry(this.Joint, String.Format("{0}{1:00}", code, h), insPt.ToPoint3d(), h2, angle);
                        }
                        else
                        {
                            this.FindLocation(new Tuple<int, Mampara>(h2, last), center, out insPt, out angle);
                            return new RemateGeometry(this.Joint, String.Format("{0}{1:00}", code, h), insPt.ToPoint3d(), h1, angle);
                        }
                    }
                    else
                    {
                        this.AltoColumna = h1;
                        return null;
                    }
                }
                else
                    throw new DeltaException("La unión no tiene las mamparas necesarias, para poder seleccionar el remate");
            }
            catch (Exception exc)
            {
                App.Riviera.Log.AppendEntry(String.Format("Error al generar el remate para unión en L, union: {0}", this.Joint.Handle), Protocol.Error, this);
                throw exc;
            }
        }
        /// <summary>
        /// Realiza el proceso de validación para una unión en T
        /// </summary>
        /// <param name="mamparas">La mampara seleccionada</param>
        /// <param name="remateColl">La colección de los remates</param>
        /// <returns>El remate para el tipo de unión actual</returns>
        private void RemateForUnionTypeT(ref List<RemateGeometry> remateColl, IEnumerable<Mampara> mamparas)
        {
            try
            {
                Tuple<int, Mampara>[] mamparasByHeight = this.GroupByHeight(mamparas);
                int h;
                String code;
                if (mamparasByHeight.Length == 3)
                {
                    Boolean sameHeight = mamparasByHeight[0].Item1 == mamparasByHeight[1].Item1 && mamparasByHeight[1].Item1 == mamparasByHeight[2].Item1;
                    if (sameHeight)
                        this.AltoColumna = mamparasByHeight[0].Item1;
                    //Caso 1: Dos alturas iguales
                    else if (mamparasByHeight[0].Item1 == mamparasByHeight[1].Item1 ||
                         mamparasByHeight[0].Item1 == mamparasByHeight[2].Item1 ||
                         mamparasByHeight[1].Item1 == mamparasByHeight[2].Item1)
                    {
                        int hm = mamparasByHeight.Select(x => x.Item1).Min(),
                            hM = mamparasByHeight.Select(x => x.Item1).Max(),
                            hq = mamparasByHeight[0].Item1 == mamparasByHeight[1].Item1 ||
                                 mamparasByHeight[0].Item1 == mamparasByHeight[2].Item1 ? mamparasByHeight[0].Item1 : mamparasByHeight[1].Item1;
                        Point2d center = this.Joint.Start.MiddlePointTo((this.Joint as MamparaJoint).GetEndPoint()),
                                insPt;
                        Double angle;
                        h = (int)(hM - hm);
                        code = DT_REMATE_FINAL_D_HEIGHT;
                        if (hq == hm)
                        {
                            this.FindLocation(mamparasByHeight.Where(x => x.Item1 == hM).FirstOrDefault(), center, out insPt, out angle);
                            remateColl.Add(new RemateGeometry(this.Joint, String.Format("{0}{1:00}", code, h), insPt.ToPoint3d(), hm, angle));
                            this.AltoColumna = hm;
                        }
                        else
                            this.AltoColumna = hM;
                    }
                    //Caso 2: Alturas diferentes
                    else
                    {
                        int hm = mamparasByHeight.Select(x => x.Item1).Min(),
                            hMM = mamparasByHeight.Select(x => x.Item1).Max(),
                            hM = mamparasByHeight.Where(x => x.Item1 != hm && x.Item1 != hMM).FirstOrDefault().Item1;
                        Point2d center = this.Joint.Start.MiddlePointTo((this.Joint as MamparaJoint).GetEndPoint()),
                                insPt;
                        Double angle;
                        code = DT_REMATE_FINAL_D_HEIGHT;
                        h = (int)(hMM - hM);
                        this.FindLocation(mamparasByHeight.Where(x => x.Item1 == hMM).FirstOrDefault(), center, out insPt, out angle);
                        remateColl.Add(new RemateGeometry(this.Joint, String.Format("{0}{1:00}", code, h), insPt.ToPoint3d(), hM, angle));
                        this.AltoColumna = hM;
                    }
                }
                else
                    throw new DeltaException("La unión no tiene las mamparas necesarias, para poder seleccionar el remate");
            }
            catch (Exception exc)
            {
                App.Riviera.Log.AppendEntry(String.Format("Error al generar el remate para unión en T, union: {0}", this.Joint.Handle), Protocol.Error, this);
                throw exc;
            }
        }
        /// <summary>
        /// Agregá un relleno intermedio
        /// </summary>
        /// <param name="joint">La unión intermedia</param>
        /// <param name="left">La mampara izquierda es igual a la derecha</param>
        /// <param name="top">La mampara top que es perpendicular a la izquierda</param>
        internal RemateGeometry CreateRemate(JointObject joint, Mampara left, Mampara top)
        {
            int hm = (int)top.RivSize.Nominal.Alto,
                         hM = (int)left.RivSize.Nominal.Alto;
            Point2d insPt;
            Double angle;
            if (top.Start.GetDistanceTo(joint.Start) < top.End.GetDistanceTo(joint.Start))
            {
                insPt = top.Start;
                angle = top.Start.GetVectorTo(top.End).Angle;
            }
            else
            {
                insPt = top.End;
                angle = top.End.GetVectorTo(top.Start).Angle;
            }
            return new RemateGeometry(joint, String.Format("{0}{1:00}", DT_REMATE_FINAL_D_HEIGHT, hM - hm), insPt.ToPoint3d(), hm, angle);
        }
        /// <summary>
        /// Checa si los paneles de la unión se comportaran de manera extendida,
        /// Solo aplica para un frente de 18 y una diferencia de altura de 6"
        /// </summary>
        /// <param name="joint">La unión intermedia</param>
        /// <param name="left">La mampara izquierda</param>
        /// <param name="right">La mampara derecha</param>
        /// <param name="top">La mampara perpendicular</param>
        /// <returns>Verdadero si cumplen la regla.</returns>
        internal bool AreBackPanelesExtended(JointObject joint, out Mampara left, out Mampara right, out Mampara top)
        {
            RivieraPanelDoubleLocation.FindMampara(joint, out left, out right, out top);
            return left.RivSize.Nominal.Frente == 18 && Math.Abs(left.RivSize.Nominal.Alto - top.RivSize.Nominal.Alto) == 6 && left.RivSize.Nominal.Alto > top.RivSize.Nominal.Alto;
        }



        /// <summary>
        /// Realiza el proceso de validación para una unión en X
        /// </summary>
        /// <param name="mamparas">La mampara seleccionada</param>
        /// <param name="remateColl">La colección de los remates</param>
        /// <returns>El remate para el tipo de unión actual</returns>
        private void RemateForUnionTypeX(ref List<RemateGeometry> remateColl, IEnumerable<Mampara> mamparas)
        {
            try
            {
                Tuple<int, Mampara>[] mamparasByHeight = this.GroupByHeight(mamparas);
                if (mamparasByHeight.Length == 4)
                {
                    int h,
                    hm = mamparasByHeight.Select(x => x.Item1).Min(),
                    hM = mamparasByHeight.Select(x => x.Item1).Max();
                    Point2d center = this.Joint.Start.MiddlePointTo((this.Joint as MamparaJoint).GetEndPoint()),
                            insPt;
                    Double angle;

                    String code = DT_REMATE_FINAL_D_HEIGHT;
                    this.AltoColumna = hm;
                    // Caso 1: Dos alturas iguales
                    //Sin remates se van a la altura mayor
                    if ((mamparasByHeight.Count(x => x.Item1 == hm) == mamparasByHeight.Count(x => x.Item1 == hM)) && hm != hM)
                    {
                        remateColl.Clear();
                        this.AltoColumna = hM;
                    }
                    // Caso 2: Tres alturas siferencres
                    //Sin remates se van a la altura mayor
                    else if ((mamparasByHeight.Count(x => x.Item1 != hm && x.Item1 != hM) > 0) && hm != hM)
                    {
                        this.AltoColumna = mamparasByHeight.Where(x => x.Item1 != hm && x.Item1 != hM).FirstOrDefault().Item1;
                        h = hM - this.AltoColumna;
                        foreach (var mam in mamparasByHeight.Where(x => x.Item1 == hM))
                        {
                            this.FindLocation(mam, center, out insPt, out angle);
                            remateColl.Add(new RemateGeometry(this.Joint, String.Format("{0}{1:00}", code, h), insPt.ToPoint3d(), this.AltoColumna, angle));
                        }
                    }
                    //Caso 3: Tres alturas iguales con altura mayor
                    else if ((mamparasByHeight.Count(x => x.Item1 == hm) == 2) || (mamparasByHeight.Count(x => x.Item1 == hM) == 3))
                    {
                        h = (int)(hM - hm);
                        foreach (var mam in mamparasByHeight.Where(x => x.Item1 == hM))
                        {
                            this.FindLocation(mam, center, out insPt, out angle);
                            remateColl.Add(new RemateGeometry(this.Joint, String.Format("{0}{1:00}", code, h), insPt.ToPoint3d(), hm, angle));
                        }
                    }
                    //Caso 4: Tres alturas iguales con altura menor
                    else if (mamparasByHeight.Count(x => x.Item1 == hm) == 3)
                    {
                        h = (int)(hM - hm);
                        this.FindLocation(mamparasByHeight.Where(x => x.Item1 == hM).FirstOrDefault(), center, out insPt, out angle);
                        remateColl.Add(new RemateGeometry(this.Joint, String.Format("{0}{1:00}", code, h), insPt.ToPoint3d(), hm, angle));
                    }
                    else
                    {
                        foreach (var mam in mamparasByHeight.Where(x => x.Item1 != hm))
                        {
                            h = (int)(mam.Item1 - hm);
                            this.FindLocation(mam, center, out insPt, out angle);
                            remateColl.Add(new RemateGeometry(this.Joint, String.Format("{0}{1:00}", code, h), insPt.ToPoint3d(), hm, angle));
                        }
                    }
                }
                else
                    throw new DeltaException("La unión no tiene las mamparas necesarias, para poder seleccionar el remate");
            }
            catch (Exception exc)
            {
                App.Riviera.Log.AppendEntry(String.Format("Error al generar el remate para unión en T, union: {0}", this.Joint.Handle), Protocol.Error, this);
                throw exc;
            }
        }
        /// <summary>
        /// Agrupa las mamparas por alturas
        /// </summary>
        /// <param name="mamparas">Las mamparas agrupar</param>
        /// <returns>Las mamparas agrupadas</returns>
        private Tuple<int, Mampara>[] GroupByHeight(IEnumerable<Mampara> mamparas)
        {
            Tuple<int, Mampara>[] mamparasByHeight = new Tuple<int, Mampara>[mamparas.Count()];
            int h, index = 0;
            //Calculo de alturas
            foreach (Mampara mam in mamparas)
            {
                h = this.ParseHeight(mam);
                mamparasByHeight[index] = new Tuple<int, Mampara>(h, mam);
                index++;
            }
            return mamparasByHeight;
        }
        /// <summary>
        /// Obtien la ubicación para un remate, con base a una mampara
        /// </summary>
        /// <param name="mam">La mampara base</param>
        /// <param name="insPt">Como salida el punto de inserción del remate</param>
        /// <param name="angle">El ángulo de rotación para el remate</param>
        private void FindLocation(Tuple<int, Mampara> mam, Point2d center, out Point2d insPt, out double angle)
        {

            if (mam.Item2.Start.GetDistanceTo(center) < mam.Item2.End.GetDistanceTo(center))
            {
                insPt = mam.Item2.Start;
                angle = mam.Item2.Start.GetVectorTo(mam.Item2.End).Angle;
            }
            else
            {
                insPt = mam.Item2.End;
                angle = mam.Item2.End.GetVectorTo(mam.Item2.Start).Angle;
            }
        }

        /// <summary>
        /// Realiza el parseo de la altura de una mampara
        /// </summary>
        /// <param name="mam">La altura de la mampara a parsear</param>
        /// <returns>La altura de la mampara</returns>
        private int ParseHeight(Mampara mam)
        {
            int num;
            return int.TryParse(mam.Code.Substring(8, 2), out num) ? num : 0;
        }


        /// <summary>
        /// Realiza la selección de las mampara contenidas en la unión
        /// </summary>
        /// <returns>La colección de las mamparas seleccionadas</returns>
        public IEnumerable<Mampara> GetMamparas()
        {
            try
            {
                IEnumerable<long> childIds;
                //Se realiza la selección de las mamparas involucradas
                if (this.Joint.Parent != 0)
                    childIds = this.Joint.Children.Values.Where(x => x != 0).Union(new long[] { this.Joint.Parent });
                else
                    childIds = this.Joint.Children.Values.Where(x => x != 0);
                return childIds.Select(x => App.DB[x] as Mampara);
            }
            catch (Exception exc)
            {
                App.Riviera.Log.AppendEntry(String.Format("Error al seleccionar las mamparas de la unión: {0}({1})", this.Joint.Type, this.Joint.Handle), Protocol.Error, this);
                throw exc;
            }
        }
        /// <summary>
        /// Realiza el dibujo de la columna
        /// </summary>
        /// <param name="tr">La transacción activa</param>
        /// <returns>El objectId de la columna dibujada</returns>
        public ObjectId CreateColumn(Transaction tr)
        {
            try
            {
                String c = String.Format("{0}{1:00}", DT_RELLENO_L_T, this.AltoColumna);
                FileInfo block = App.Riviera.Delta3D.Where(x => x.Name == (c + ".dwg")).FirstOrDefault();
                Block3D blk = new Block3D(tr, c, block, this.Joint);
                return blk.Id;
            }
            catch (Exception exc)
            {
                App.Riviera.Log.AppendEntry(exc.Message, Protocol.Error, this);
                return new ObjectId();
            }
        }
        /// <summary>
        /// Realiza el dibujo de la columna
        /// </summary>
        /// <param name="tr">La transacción activa</param>
        /// <returns>El objectId de la columna dibujada</returns>
        public ObjectId AddColumnTopper(IEnumerable<Mampara> mamparas, Transaction tr)
        {
            Tuple<int, Mampara>[] mamparasByHeight = this.GroupByHeight(mamparas);
            Mampara left, right, top;
            //Se realiza el proceso de selección de la mampara
            RivieraPanelDoubleLocation.FindMampara(this.Joint, out left, out right, out top);
            String c = "Complemento moldura";
            FileInfo block = App.Riviera.Delta3D.Where(x => x.Name == (c + ".dwg")).FirstOrDefault();
            Block3D blk = new Block3D(tr, c, block, left);
            var blkRef = (blk.Id.GetObject(OpenMode.ForWrite) as BlockReference);
            blkRef.Rotation = left.Start.GetVectorTo(right.Start).Angle;
            blkRef.Position = left.Start.GetDistanceTo(this.Joint.Start) > left.End.GetDistanceTo(this.Joint.End) ? left.End.ToPoint3d(blkRef.Position.Z) : left.Start.ToPoint3d(blkRef.Position.Z);
            Double h = mamparasByHeight[0].Item1 == mamparasByHeight[1].Item1 ? mamparasByHeight[0].Item1 : mamparasByHeight[2].Item1;
            blk.UpdateHeight(tr, h);
            return blk.Id;
        }
        /// <summary>
        /// Realiza el dibujo de los remates
        /// </summary>
        /// <param name="tr">La transacción activa</param>
        /// <returns>El objectId de la columna dibujada</returns>
        public ObjectIdCollection CreateRemates(Transaction tr, List<RemateGeometry> remateColl)
        {
            ObjectIdCollection ids = new ObjectIdCollection();
            try
            {
                foreach (RemateGeometry remate in remateColl)
                {
                    Block3D blk = remate.CreateBlock(tr);
                    ids.Add(blk.Id);
                }
            }
            catch (Exception exc)
            {
                App.Riviera.Log.AppendEntry(exc.Message, Protocol.Error, this);
            }
            return ids;
        }


    }
}
