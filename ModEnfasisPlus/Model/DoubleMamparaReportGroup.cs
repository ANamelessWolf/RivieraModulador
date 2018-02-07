using DaSoft.Riviera.OldModulador.Controller;
using System;
using System.Linq;
using System.Text;

namespace DaSoft.Riviera.OldModulador.Model
{
    public class DoubleMamparaReportGroup : MamparaReportGroup
    {
        public Double Frente;
        /// <summary>
        /// El lado b de la mampara
        /// </summary>
        public MamparaReportItem LadoB_Left;
        /// <summary>
        /// El lado b de la mampara
        /// </summary>
        public MamparaReportItem LadoB_Right;
        public override string ToString()
        {
            String str = String.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append(LadoA.Mampara);
            String paneles = String.Empty;
            foreach (String panel in LadoA.Panels.Union(LadoB_Left.Panels).Union(LadoB_Right.Panels).Select(x => x.Value.Code.AddAcabado(x.Value)))
                paneles += panel + "@";
            if (paneles.Length > 0)
                paneles = paneles.Substring(0, paneles.Length - 1);
            sb.Append('|');
            sb.Append(paneles);
            str = String.Format("{0}|{1}", sb.ToString(), LadoA.Biombo != null ? LadoA.Biombo.Code.AddAcabado(LadoA.Biombo) : String.Empty);
            if (str[str.Length - 1] == '|')
                str = str.Substring(0, str.Length - 1);
            return str;
        }
    }
}