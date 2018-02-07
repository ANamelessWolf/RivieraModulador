using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using DaSoft.Riviera.OldModulador.Controller;
using DaSoft.Riviera.OldModulador.Runtime;
using NamelessOld.Libraries.HoukagoTeaTime.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DaSoft.Riviera.OldModulador.Model
{
    public class MamparaReportItem
    {
        /// <summary>
        /// El nombre del lado
        /// </summary>
        public String Lado;
        /// <summary>
        /// El código de la mampara
        /// </summary>
        public String Mampara;
        /// <summary>
        /// La información de paneles
        /// </summary>
        public Dictionary<Double, PanelRaw> Panels;
        /// <summary>
        /// La información del biombo
        /// </summary>
        public RivieraBiombo Biombo;

        /// <summary>
        /// La clave del elemento de mampara 
        /// </summary>
        /// <returns>La clave del elemento</returns>
        public override string ToString()
        {
            String paneles = String.Empty;
            foreach (String panel in Panels.Values.Select(x => x.Code.AddAcabado(x)))
                paneles += panel + "@";
            if (paneles.Length > 0)
                paneles.Substring(0, paneles.Length - 1);
            return String.Format("{0}|{1}|{2}", Mampara, paneles, Biombo != null ? Biombo.Code.AddAcabado(Biombo) : String.Empty);
        }
        /// <summary>
        /// Carga los códigos de los paneles
        /// </summary>
        /// <param name="panelId">El id de la colección de paneles a cargar</param>
        /// <param name="sideA">El lado A</param>
        /// <returns>La lista de paneles a cargar</returns>
        public static Dictionary<Double, PanelRaw> LoaPaneles(long panelId)
        {
            Dictionary<Double, PanelRaw> paneles = new Dictionary<Double, PanelRaw>();
            if (panelId != 0)
            {
                var panels = (App.DB[panelId] as RivieraPanelStack).Collection;
                foreach (PanelRaw raw in panels)
                    if (!paneles.ContainsKey(raw.Height))
                        paneles.Add(raw.Height, raw);
                    else if (raw.Height == 0)
                    {
                        raw.Height = 0.128;
                        paneles.Add(raw.Height, raw);
                        new FastTransactionWrapper((Document doc, Transaction tr) => { (App.DB[panelId] as RivieraPanelStack).Save(tr); }).Run();
                    }
            }
            return paneles;
        }
    }
}
