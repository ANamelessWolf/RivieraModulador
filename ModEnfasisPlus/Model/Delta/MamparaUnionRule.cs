using DaSoft.Riviera.OldModulador.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using static DaSoft.Riviera.OldModulador.Assets.RIVIERA_CONST;
namespace DaSoft.Riviera.OldModulador.Model.Delta
{
    public class MamparaUnionRule
    {
        /// <summary>
        /// El nombre de la unión
        /// </summary>
        public String UnionName;
        /// <summary>
        /// El tipo de union al que pertence la mampara
        /// </summary>
        public JointType Type;
        /// <summary>
        /// El tipo de mampara asignada al angulo seleccionado
        /// </summary>
        public Dictionary<int, String> MamparaType;
        /// <summary>
        /// La lista de elementos a cuantificar
        /// </summary>
        public List<QuantifiableObject> Quantify;
        /// <summary>
        /// Crea una nueva regla de union de mampara
        /// </summary>
        /// <param name="row">La fila a parsear</param>
        public MamparaUnionRule(string[] row)
            : this()
        {
            int[] ang = new int[] { 0, 90, 180, 270 };
            string[] heights = new String[] { row[0], row[1], row[2], row[3] };
            for (int i = 0; i < ang.Length; i++)
                MamparaType.Add(ang[i], heights[i]);

            int count;
            if (row[4] == UNION_I)
                this.Type = JointType.Joint_I;
            else if (row[4] == UNION_L)
                this.Type = JointType.Joint_L;
            else if (row[4] == UNION_T)
                this.Type = JointType.Joint_T;
            else if (row[4] == UNION_X)
                this.Type = JointType.Joint_X;
            for (int i = 5; i < row.Length - 1; i += 3)
                if (row[i] != String.Empty)
                    Quantify.Add(new QuantifiableObject()
                    {
                        Code = row[i],
                        Count = int.TryParse(row[i + 1], out count) ? count : 0,
                        Visibility = row[i + 2] == "S" ? true : false
                    });
            this.UnionName = row[row.Length - 1];
        }
        /// <summary>
        /// Crea una nueva regla de union de mampara
        /// vacía
        /// </summary>
        public MamparaUnionRule()
        {
            this.MamparaType = new Dictionary<int, string>();
            this.Quantify = new List<QuantifiableObject>();
            this.Type = JointType.None;
        }
        /// <summary>
        /// Realiza una selección de una regla
        /// </summary>
        /// <param name="union">la union a seleccionar</param>
        /// <param name="hAng0">La altura de la mampara a 0°</param>
        /// <param name="hAng1">La altura de la mampara a 90°</param>
        /// <param name="hAng2">La altura de la mampara a 180°</param>
        /// <param name="hAng3">La altura de la mampara a 270°</param>
        public static MamparaUnionRule[] Select(JointType union, string hAng0, string hAng90 = "", string hAng180 = "", string hAng270 = "")
        {
            IEnumerable<MamparaUnionRule> rules =
                App.DB.MamparaUnionRules.Where(x =>
                x.Type == union &&
                x.MamparaType[0] == hAng0 &&
                x.MamparaType[90] == hAng90 &&
                x.MamparaType[180] == hAng180 &&
                x.MamparaType[270] == hAng270);
            return rules.ToArray();
        }
        /// <summary>
        /// Devuelve el código obtenido en la unión
        /// </summary>
        /// <param name="code">El código a editar</param>
        /// <param name="hAng0">La altura ubicado a los 0 grados</param>
        /// <param name="hAng90">La altura ubicada a los 90 grados</param>
        /// <param name="hAng180">La altura ubicada a los 180 grados</param>
        /// <param name="hAng270">La altura ubicada a los 270 grados</param>
        /// <returns>El codigo formateado</returns>
        public string GetCode(string code, double hAng0, double hAng90, double hAng180, double hAng270)
        {
            String prefix, suffix, op;
            if (code.Contains('(') && code.Contains(')'))
            {
                prefix = code.Split('(')[0];
                suffix = code.Split(')')[1];
                op = code.Split('(')[1].Split(')')[0];
                op = op.Replace(M1, hAng0.ToString()).Replace(M2, hAng90.ToString()).Replace(M3, hAng180.ToString()).Replace(M4, hAng270.ToString());
                code = String.Format("{0}{1:00}{2}", prefix, SolveCodeOperation(op), suffix);
            }
            return code;
        }
        /// <summary>
        /// Resuelve la operación del código seleccionado
        /// </summary>
        /// <param name="operation">La operación a resolver</param>
        /// <returns>El resultado de la operación del código</returns>
        private Double SolveCodeOperation(string operation)
        {
            String[] str = new String[0];
            char op = ' ';
            if (operation.Contains('*'))
            {
                str = operation.Split('*');
                op = '*';
            }
            else if (operation.Contains('/'))
            {
                str = operation.Split('/');
                op = '/';
            }
            else if (operation.Contains('-'))
            {
                str = operation.Split('-');
                op = '-';
            }
            else if (operation.Contains('+'))
            {
                str = operation.Split('+');
                op = '+';
            }
            //Operador de concatenación
            else if (operation.Contains(','))
            {
                str = operation.Split(',');
                op = ',';
            }
            else
                str = new String[1] { operation };
            return this.Solve(op, str);
        }
        /// <summary>
        /// Realiza la solución de la operación
        /// </summary>
        /// <param name="str">La cadena a resolver</param>
        private double Solve(Char op, string[] str)
        {
            double num, num2;
            if (str.Length == 1)
                return Double.TryParse(str[0], out num) ? num : 0;
            else if (str.Length == 2)
            {
                num = Double.TryParse(str[0], out num) ? num : 0;
                num2 = Double.TryParse(str[1], out num2) ? num2 : 0;
                return Solve(num, num2, op);
            }
            return 0;

        }
        /// <summary>
        /// Realiza la operación aritmetica
        /// </summary>
        /// <param name="num">El primer número</param>
        /// <param name="num2">El segundo número</param>
        /// <param name="op">La operación del elemento</param>
        /// <returns>El resultado aritmetico</returns>
        private double Solve(double num, double num2, char op)
        {
            if (op == '*')
                return num * num2;
            else if (op == '/')
                return num / num2;
            else if (op == '-')
                return num - num2;
            else if (op == '+')
                return num + num2;
            else if (op == ',')
                return double.Parse(String.Format("{0}{1}", (int)num, (int)num2));
            else
                return 0;
        }


        /// <summary>
        /// Imprime el código del elemento
        /// </summary>
        public override string ToString()
        {
            return String.Format("Union: {0}, Codes: {1}", this.Type, this.Quantify.Count);
        }
    }
}
