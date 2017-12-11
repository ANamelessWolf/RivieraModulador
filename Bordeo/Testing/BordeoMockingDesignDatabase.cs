using DaSoft.Riviera.Modulador.Bordeo.Model;
using DaSoft.Riviera.Modulador.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaSoft.Riviera.Modulador.Bordeo.Testing
{
    public class BordeoMockingDesignDatabase : BordeoDesignDatabase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BordeoMockingDesignDatabase"/> class.
        /// </summary>
        public BordeoMockingDesignDatabase()
        {
            this.Sizes = new Dictionary<string, ElementSizeCollection>();
            this.InitSizes();
            this.Codes = new RivieraCode[]
            {
                 new RivieraCode(){ Block ="BR2010", Code="BR2010", Description="Panel Recto", ElementType= RivieraElementType.Panel, Line= DesignLine.Bordeo },
                 new RivieraCode(){ Block ="BR2020", Code="BR2020", Description="Panel 90°", ElementType= RivieraElementType.Panel, Line= DesignLine.Bordeo },
                 new RivieraCode(){ Block ="BR2030", Code="BR2030", Description="Panel 135°", ElementType= RivieraElementType.Panel, Line= DesignLine.Bordeo },
            }.ToList();
        }
        /// <summary>
        /// Initializes the sizes.
        /// </summary>
        private void InitSizes()
        {
            ElementSizeCollection panelRecto = new ElementSizeCollection("BR2010");
            ElementSizeCollection panelL90 = new ElementSizeCollection("BR2020");
            ElementSizeCollection panelL135 = new ElementSizeCollection("BR2030");
            double[] frentes = new double[] { 18d, 24, 30, 36, 42, 48 },
                heights = new double[] { 15, 27 };
            RivieraSize frente, frenteFinal, alto;
            for (int i = 0; i < frentes.Length; i++)
                for (int j = 0; j < heights.Length; j++)
                {
                    frente = new RivieraSize() { Measure = "FRENTE", Nominal = frentes[i], Real = frentes[i] * 0.0254d };
                    alto = new RivieraSize() { Measure = "ALTO", Nominal = heights[j], Real = heights[j] * 0.0254d };
                    panelRecto.Sizes.Add(new PanelMeasure(frente, alto));
                }
            for (int i = 0; i < frentes.Length; i++)
                for (int j = 0; j < frentes.Length; j++)
                    for (int k = 0; k < heights.Length; k++)
                    {
                        frente = new RivieraSize() { Measure = "FRENTE_INICIAL", Nominal = frentes[i], Real = frentes[i] * 0.0254d };
                        frenteFinal = new RivieraSize() { Measure = "FRENTE_FINAL", Nominal = frentes[j], Real = frentes[j] * 0.0254d };
                        alto = new RivieraSize() { Measure = "ALTO", Nominal = heights[k], Real = heights[k] * 0.0254d };
                        panelL90.Sizes.Add(new LPanelMeasure(frente, frenteFinal, alto));
                        panelL135.Sizes.Add(new LPanelMeasure(frente, frenteFinal, alto));
                    }
            this.Sizes.Add(panelRecto.Code, panelRecto);
            this.Sizes.Add(panelL90.Code, panelL90);
            this.Sizes.Add(panelL135.Code, panelL135);
        }
    }
}
