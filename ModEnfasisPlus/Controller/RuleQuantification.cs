using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using DaSoft.Riviera.OldModulador.Model;
using DaSoft.Riviera.OldModulador.Model.Delta;
using DaSoft.Riviera.OldModulador.Runtime;
using NamelessOld.Libraries.HoukagoTeaTime.Mio;
using NamelessOld.Libraries.HoukagoTeaTime.Mio.Entities;
using NamelessOld.Libraries.HoukagoTeaTime.Mio.Model;
using NamelessOld.Libraries.HoukagoTeaTime.Ritsu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DaSoft.Riviera.OldModulador.Assets.RIVIERA_CODE;
using static DaSoft.Riviera.OldModulador.Assets.RIVIERA_CONST;
using static DaSoft.Riviera.OldModulador.Assets.Strings;
namespace DaSoft.Riviera.OldModulador.Controller
{
    /// <summary>
    /// Realiza el proceso de cuantificación mediante el uso de reglas de la aplicación
    /// </summary>
    public class RuleQuantification
    {
        /// <summary>
        /// Define las reglas de cuantificación.
        /// </summary>
        public MamparaUnionRule[] Rules;
        /// <summary>
        /// La unión a cuantificar.
        /// </summary>
        public JointObject Joint;
        /// <summary>
        /// Obtiene el tipo de unión a cuantificar.
        /// </summary>
        /// <value>
        /// El tipo de unión.
        /// </value>
        public JointType Type { get { return this.Joint.Type; } }
        /// <summary>
        /// La información de cuantificación de la unión.
        /// </summary>
        public QuantifiableUnion QuantificationInfo;
        /// <summary>
        /// El nombre de la zona a cuantificar
        /// </summary>
        public String Zone;
        /// <summary>
        /// Las alturas involucradas en la regla de cuantificación
        /// </summary>
        public Double[] Heights;
        /// <summary>
        /// La colección de mamparas guardadas por su ángulo de orientación
        /// [0], [90], [180], [270]
        /// </summary>
        Dictionary<int, Mampara> MamparasByAngle;
        /// <summary>
        /// La colección de alturas guardadas por su ángulo de orientación
        /// [0], [90], [180], [270]
        /// </summary>
        Dictionary<int, String> HeightsByAngle;
        /// <summary>
        /// La colección de alturas guardadas por su ángulo de orientación
        /// [0], [90], [180], [270]
        /// </summary>
        Dictionary<int, double> NominalHeightsByAngle;
        /// <summary>
        /// Inicializa una instancia de la clase <see cref="RuleQuantification"/>.
        /// </summary>
        /// <param name="joint">La unión a cuantificar.</param>
        /// <param name="zone">El nombre de la zona a cuantificar.</param>
        public RuleQuantification(JointObject joint, String zone)
        {
            this.Joint = joint;
            this.Zone = zone;
            this.QuantificationInfo = this.Joint.QuantifyableUnion();
            this.QuantificationInfo.ZoneName = zone;
            var heights = this.QuantificationInfo.Members.Select(x => Double.Parse(x.Substring(x.Length - 3, 2)));
            if (this.Type == JointType.Joint_I || this.Type == JointType.Joint_L)
                this.Heights = heights.OrderByDescending(x => x).ToArray();
            else
                this.Heights = heights.ToArray();
            this.HeightsByAngle = new Dictionary<int, string>();
            this.NominalHeightsByAngle = new Dictionary<int, double>();
            this.HeightsByAngle.Add(0, String.Empty);
            this.HeightsByAngle.Add(90, String.Empty);
            this.HeightsByAngle.Add(180, String.Empty);
            this.HeightsByAngle.Add(270, String.Empty);
            this.NominalHeightsByAngle.Add(0, 0d);
            this.NominalHeightsByAngle.Add(90, 0d);
            this.NominalHeightsByAngle.Add(180, 0d);
            this.NominalHeightsByAngle.Add(270, 0d);
            if (this.Type == JointType.Joint_I)
                this.Rules = this.GetJointIRules();
            else if (this.Type == JointType.Joint_L)
                this.Rules = this.GetJointLRules();
            else if (this.Type == JointType.Joint_T)
                this.Rules = this.GetJointTRules();
            else if (this.Type == JointType.Joint_X)
                this.Rules = this.GetJointXRules();
        }
        /// <summary>
        /// Realiza el proceso de cuantificación
        /// </summary>
        public void Quantify()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            if (this.Rules.Length == 0)
            {
                ed.WriteMessage("\n{0}", String.Format(WAR_RULE_NOT_FOUND, this.Type.GetLetter(), HeightsByAngle[0], HeightsByAngle[90], HeightsByAngle[180], HeightsByAngle[270]));
                this.InsertRuleTag(null);
            }
            else
            {
                MamparaUnionRule rule = this.Rules.FirstOrDefault();
                foreach (QuantifiableObject item in rule.Quantify)
                    for (int i = 1; i <= item.Count; i++)
                        this.QuantificationInfo.Members.Add(rule.GetCode(item.Code, NominalHeightsByAngle[0], NominalHeightsByAngle[90], NominalHeightsByAngle[180], NominalHeightsByAngle[270]));
                //Se inserta la etiqueta de unión
                this.InsertRuleTag(rule);
            }
        }
        /// <summary>
        /// Obtiene las reglas de cuantificación para una unión en I.
        /// </summary>
        /// <returns>Las reglas de cuantificación.</returns>
        private MamparaUnionRule[] GetJointIRules()
        {
            this.NominalHeightsByAngle[0] = this.Heights[0];
            this.NominalHeightsByAngle[180] = this.Heights[1];
            this.HeightsByAngle[0] = "A";
            //Alturas iguales
            if (this.Heights[0] == this.Heights[1])
            {
                this.HeightsByAngle[180] = "A";
                return MamparaUnionRule.Select(this.Type, hAng0: H1, hAng180: H1);
            }
            //Alturas diferentes
            else
            {
                this.HeightsByAngle[180] = "AM";
                return MamparaUnionRule.Select(this.Type, hAng0: H1, hAng180: H2);
            }
        }
        /// <summary>
        /// Obtiene las reglas de cuantificación para una unión en L.
        /// </summary>
        /// <returns>Las reglas de cuantificación.</returns>
        private MamparaUnionRule[] GetJointLRules()
        {
            this.NominalHeightsByAngle[0] = this.Heights[0];
            this.NominalHeightsByAngle[90] = this.Heights[1];
            this.HeightsByAngle[0] = "A";
            //Alturas iguales
            if (this.Heights[0] == this.Heights[1])
            {
                this.HeightsByAngle[90] = "A";
                return MamparaUnionRule.Select(this.Type, hAng0: H1, hAng90: H1);
            }
            //Alturas diferentes
            else
            {
                this.HeightsByAngle[90] = "AM";
                return MamparaUnionRule.Select(this.Type, hAng0: H1, hAng90: H2);
            }
        }
        /// <summary>
        /// Obtiene las reglas de cuantificación para una unión en L.
        /// </summary>
        /// <returns>Las reglas de cuantificación.</returns>
        private MamparaUnionRule[] GetJointTRules()
        {
            MamparaUnionRule[] rules;
            //Alturas iguales
            if (this.Heights.Count(x => this.Heights[0] == x) == 3)
            {
                new int[] { 0, 90, 180 }.ToList().ForEach(x =>
                 {
                     this.NominalHeightsByAngle[x] = this.Heights[0];
                     this.HeightsByAngle[x] = "A";
                 });
                rules = MamparaUnionRule.Select(this.Type, hAng0: H1, hAng90: H1, hAng180: H1);
                if (this.Joint.HasDoublePanels)
                    rules = rules.Where(x => x.UnionName.Last() == 'D').ToArray();
                return rules;
            }
            //Alturas diferentes
            else
                return this.SelectRules();
        }
        /// <summary>
        /// Obtiene las reglas de cuantificación para una unión en X.
        /// </summary>
        /// <returns>Las reglas de cuantificación.</returns>
        private MamparaUnionRule[] GetJointXRules()
        {
            //Alturas iguales
            if (this.Heights.Count(x => this.Heights[0] == x) == 4)
            {
                new int[] { 0, 90, 180, 270 }.ToList().ForEach(x =>
                 {
                     this.NominalHeightsByAngle[x] = this.Heights[0];
                     this.HeightsByAngle[x] = "A";
                 });
                return MamparaUnionRule.Select(this.Type, hAng0: H1, hAng90: H1, hAng180: H1, hAng270: H1);
            }
            //Alturas diferentes
            else
                return this.SelectRules();
        }
        /// <summary>
        /// Realiza la selección de las alturas dependiendo de las mamparas seleccionadas.
        /// </summary>
        private MamparaUnionRule[] SelectRules()
        {
            List<Mampara> mam = new List<Mampara>();
            this.MamparasByAngle = new Dictionary<int, Mampara>();
            //1: Seleccionamos las mamparas involucradas en la unión
            if (this.Joint.Parent != 0)
                mam.Add(App.DB[this.Joint.Parent] as Mampara);
            foreach (long childId in this.Joint.Children.Values.Where(x => x != 0))
                mam.Add(App.DB[childId] as Mampara);
            //2: Se ordenan por alturas de mayor a menor
            mam = Sort(mam);
            //3: Se toma como pivote la primera altura la cual es la más grande
            Mampara first = mam.FirstOrDefault();
            Double ang = 0, offset = first.GetAngle(this.Joint);
            //El angulo se mide del punto inicial al punto final de la mampara
            //El desfazamiento es el angulo que tiene girada la mampara con respecto a 0°
            this.MamparasByAngle.Add(0, first);
            foreach (Mampara m in mam.Where(x => x.Handle.Value != first.Handle.Value))
            {
                ang = (m.GetAngle(this.Joint) - offset).ToDegree().To360Degree().ToQuartilDegree();
                if (!this.MamparasByAngle.ContainsKey((int)ang))
                    this.MamparasByAngle.Add((int)(ang), m);
                else//El padre esta invertido
                    this.MamparasByAngle.Add((int)((ang + 180).To360Degree().ToQuartilDegree()), m);
            }
            //4: Se llenan las alturas por angulo
            new int[] { 0, 90, 180, 270 }.ToList().ForEach(x =>
            {
                this.HeightsByAngle[x] = (this.MamparasByAngle.ContainsKey(x) ? GetHeight(mam, this.MamparasByAngle[x]) : String.Empty);
                this.NominalHeightsByAngle[x] = (this.MamparasByAngle.ContainsKey(x) ? Double.Parse(App.DB.GetSize(this.MamparasByAngle[x].Size).Alto) : 0d);
            });
            //5: Se busca una regla válida con la configuración seleccionada.
            MamparaUnionRule[] rules = MamparaUnionRule.Select(this.Type, this.HeightsByAngle[0], this.HeightsByAngle[90], this.HeightsByAngle[180], this.HeightsByAngle[270]);
            //En caso de tener paneles dobles se realiza la selección de las reglas con paneles dobles.
            //Solo la unión en T tiene paneles dobles.
            if (this.Type == JointType.Joint_T && this.Joint.HasDoublePanels)
                rules = rules.Where(x => x.UnionName.Last() == 'D').ToArray();
            return rules;
        }
        /// <summary>
        /// Determines whether [is not valid] [the specified treatment].
        /// </summary>
        /// <param name="treatment">The treatment.</param>
        /// <param name="ents">The ents.</param>
        /// <returns>
        ///   <c>true</c> if [is not valid] [the specified treatment]; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        internal bool IsNotValid(out TratamientoUnion treatment, out List<long> ents)
        {
            Boolean flag = App.DB.TratamientosParaUniones.ContainsKey(this.Rules[0].UnionName);
            ents = new List<long>();
            if (flag)
            {
                treatment = App.DB.TratamientosParaUniones[this.Rules[0].UnionName];
                ents = this.Joint.Children.Values.Union(new long[] { this.Joint.Parent }).Where(id => id != 0).Select(x => App.DB[x].Line.Handle.Value).ToList();
            }
            else
                treatment = TratamientoUnion.NULL;
            return flag;
        }

        /// <summary>
        /// Realiza el proceso de ordenamiento de mamparas de manera descendente
        /// </summary>
        /// <param name="joint">The joint.</param>
        /// <param name="unSortedMamparas">The un sorted mamparas.</param>
        /// <returns></returns>
        private List<Mampara> Sort(List<Mampara> unSortedMamparas)
        {
            List<MamparaAngleAlto> mamparas = new List<MamparaAngleAlto>();
            unSortedMamparas.ForEach(x => mamparas.Add(new MamparaAngleAlto(this.Joint, x)));
            mamparas = mamparas.OrderByDescending(x => x.Alto).ThenBy(y => y.AngleDegree).ToList();
            return mamparas.Select(x => x.Mampara).ToList();

        }
        /// <summary>
        /// Calcula la altura descriptiva en función de las demas alturas de la unión
        /// </summary>
        /// <param name="mamparas">Las mamparas que definen a la unión</param>
        /// <param name="mampara">La mampara a encontrar su altura descriptiva</param>
        /// <returns></returns>
        private string GetHeight(List<Mampara> mamparas, Mampara mampara)
        {
            String Key = "A";
            List<Double> ignoreHeights = new List<Double>();
            foreach (Mampara mam in mamparas)
            {
                if (mampara.Alto < mam.Alto && !ignoreHeights.Contains(mam.Alto))
                {
                    Key += "M";
                    ignoreHeights.Add(mam.Alto);
                }
            }
            return Key;
        }
        /// <summary>
        /// Inserta la etiqueta de unión o una etiqueta "Faltante", cuando se tiene una
        /// combinación de uniones que no estan definidas en la Base de Datos.
        /// </summary>
        /// <param name="rule">La regla a seleccionada.</param>
        private void InsertRuleTag(MamparaUnionRule rule)
        {
            AutoCADLayer layer = new AutoCADLayer(LAYER_UNION);
            Double r = 0.095d, h = ANCHO_M / 4d;
            if (App.Riviera.Units == DaNTeUnits.Imperial)
            {
                r = r.ConvertUnits(Unit_Type.m, Unit_Type.inches);
                h = h.ConvertUnits(Unit_Type.m, Unit_Type.inches);
            }
            Point2d center = this.Joint.Start.MiddlePointTo(this.Joint.End).ToPoint2dByPolar(r, Math.PI / 6);
            ObjectId[] ids = new ObjectId[] { Drawer.Text(rule != null ? rule.UnionName : "Faltante", 0, center.ToPoint3d(), h, new Margin(), "") };
            layer.AddToLayer(new ObjectIdCollection(ids));
        }
    }
}
