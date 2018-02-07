using DaSoft.Riviera.OldModulador.Controller;
using DaSoft.Riviera.OldModulador.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DaSoft.Riviera.OldModulador.Model
{
    public class QuantifiableUnion
    {
        /// <summary>
        /// El tipo de union
        /// </summary>
        public JointType Union;
        /// <summary>
        /// El elemento tipo union
        /// </summary>
        JointObject Object;
        /// <summary>
        /// El acabado del elemento
        /// </summary>
        public String Acabado;
        /// <summary>
        /// Los códigos de los miembros de la union
        /// Ordenados de 0° a 360° de ubicación
        /// </summary>
        public List<String> Members;
        /// <summary>
        /// El total de objetos a cuantificar
        /// </summary>
        public int Count;
        /// <summary>
        /// El nombre de la zona
        /// </summary>
        public String ZoneName;
        /// <summary>
        /// El handle que define a la unión
        /// </summary>
        public long Handle;

        /// <summary>
        /// Crea un objeto cuantificable desde una union
        /// </summary>
        /// <param name="obj">El objeto cuantificable</param>
        /// <param name="acabado">El acabado de la mampara</param>
        public QuantifiableUnion(JointObject obj, String acabado = "S")
        {
            this.Object = obj;
            this.Members = new List<string>();
            this.Union = obj.Type;
            this.Count = 1;
            if (obj.Parent != 0)
                this.Members.Add(App.DB[obj.Parent].Code + acabado);

            foreach (RivieraObject child in
                        obj.Children.Values.Where(x => x != 0 && App.DB[x] != null).
                        Select<long, RivieraObject>(y => App.DB[y]))
                this.Members.Add(child.Code + acabado);
        }

        public QuantifiableUnion Copy()
        {
            QuantifiableUnion union = new QuantifiableUnion(this.Object, this.Acabado) { ZoneName = ZoneName };
            union.Count = this.Count;
            union.Members.Clear();
            foreach (String member in this.Members)
                union.Members.Add(member);
            return union;
        }

        /// <summary>
        /// Crea el formato del elemento en celda
        /// </summary>
        /// <returns>El formato de la celda</returns>
        public string GetCellFormat()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Union: ");
            sb.Append(this.Union.GetLetter());
            for (int i = 0; i < this.Members.Count; i++)
            {
                sb.Append("\n");
                sb.Append(this.Members[i]);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Realiza la comparación de dos uniones,
        /// Sin importar la orientación
        /// </summary>
        /// <param name="union">La union a comparar</param>
        /// <returns>Verdadero si son iguales</returns>
        public Boolean EqualtTo(QuantifiableUnion union, Boolean strict = true)
        {
            Boolean flag = false;
            if (this.Union == union.Union && this.Members.Count == union.Members.Count)
            {
                flag = true;
                if (strict)
                    flag = flag && this.ZoneName == union.ZoneName;
                String[] membersA = this.Members.OrderBy(x => x).ToArray(),
                         membersB = union.Members.OrderBy(x => x).ToArray();
                for (int i = 0; flag && i < this.Members.Count; i++)
                    flag = flag && membersA[i] == membersB[i];

                //if (strict)
                //{
                //    for (int i = 0; i < this.Members.Count; i++)
                //        flag = flag && this.Members[i] == union.Members[i];
                //}
                //else
                //{
                //    for (int i = 0; i < this.Members.Count; i++)
                //        flag = flag &&
                //            this.Members.Where(x => x == this.Members[i]).Count() ==
                //            union.Members.Where(x => x == union.Members[i]).Count();
                //}
            }
            return flag;
        }
    }
}
