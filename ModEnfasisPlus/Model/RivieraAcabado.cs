using NamelessOld.Libraries.Yggdrasil.Lilith;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DaSoft.Riviera.OldModulador.Model
{
    public class RivieraAcabado
    {
        /// <summary>
        /// El código del acabado para el elemento seleccionado
        /// </summary>
        public String Code;
        /// <summary>
        /// La colección de acabados,
        /// Códogo-Descripción
        /// </summary>
        public List<Tuple<String, String>> Acabados;
        /// <summary>
        /// Crea una lista de acabados asociados a un código
        /// </summary>
        /// <param name="code">El código de acabado</param>
        public RivieraAcabado(String code)
        {
            this.Code = code;
            this.Acabados = new List<Tuple<String, String>>();
        }
        /// <summary>
        /// Realiza el parseo de acabados leidos de la BD
        /// </summary>
        /// <param name="rows">La colección de filas</param>
        /// <param name="acabados">La lista de acabados</param>
        public static void ParseAcabados(List<string> rows, ref List<RivieraAcabado> acabados)
        {
            String[] cell;
            foreach (String row in rows)
            {
                cell = row.Split(LilithConstants.ESCAPECHAR);
                if (acabados.Count(x => x.Code == cell[0]) > 0)
                    acabados.Where(x => x.Code == cell[0]).FirstOrDefault().Acabados.Add(new Tuple<string, string>(cell[1], cell[2]));
                else
                {
                    acabados.Add(new RivieraAcabado(cell[0]));
                    acabados.LastOrDefault().Acabados.Add(new Tuple<string, string>(cell[1], cell[2]));
                }
            }
        }
        /// <summary>
        /// Imprime el código al que tiene asociado los acabados
        /// </summary>
        /// <returns>la colección de acabados.</returns>
        public override string ToString()
        {
            return this.Code;
        }
    }
}
