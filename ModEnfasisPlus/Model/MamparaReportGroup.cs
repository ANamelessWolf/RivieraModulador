using DaSoft.Riviera.OldModulador.Controller;
using System;
using System.Linq;
using System.Text;

namespace DaSoft.Riviera.OldModulador.Model
{
    public class MamparaReportGroup
    {
        /// <summary>
        /// El lado a de la mampara
        /// </summary>
        public MamparaReportItem LadoA;
        /// <summary>
        /// El lado b de la mampara
        /// </summary>
        public MamparaReportItem LadoB;
        /// <summary>
        /// El número de mamparas con estas caracteristicas
        /// </summary>
        public int Count;
        /// <summary>
        /// Imprime la clave de la mampara
        /// </summary>
        /// <returns>La cadena seleccionada</returns>
        public override string ToString()
        {
            String str = String.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append(LadoA.Mampara);
            String paneles = String.Empty;
            foreach (String panel in LadoA.Panels.Union(LadoB.Panels).Select(x => x.Value.Code.AddAcabado(x.Value)))
                paneles += panel + "@";
            if (paneles.Length > 0)
                paneles= paneles.Substring(0, paneles.Length - 1);
            sb.Append('|');
            sb.Append(paneles);
            str = String.Format("{0}|{1}", sb.ToString(), LadoA.Biombo != null ? LadoA.Biombo.Code.AddAcabado(LadoA.Biombo) : String.Empty);
            if (str[str.Length - 1] == '|')
                str = str.Substring(0, str.Length - 1);
            return str;
        }
    }
}
