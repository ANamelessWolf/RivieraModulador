﻿using DaSoft.Riviera.Modulador.Core.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DaSoft.Riviera.Modulador.Core.Model;
using Nameless.Libraries.DB.Mikasa;
using DaSoft.Riviera.Modulador.Bordeo.Model.DB;
using DaSoft.Riviera.Modulador.Core.Model.DB;

namespace DaSoft.Riviera.Modulador.Bordeo.Model
{
    /// <summary>
    /// Defines the Bordeo Design Database
    /// </summary>
    /// <seealso cref="DaSoft.Riviera.Modulador.Core.Runtime.RivieraDesignDatabase" />
    public class BordeoDesignDatabase : RivieraDesignDatabase
    {
        public const string TABLENAME_PANEL_RECTO = "VW_BR_PANEL_RECTO";
        public const string TABLENAME_PANEL_L = "VW_BR_PANEL_L";
        public const string TABLENAME_PASO_LUZ = "VW_BR_PASO_LUZ";
        public const string TABLENAME_PUENTE = "VW_BR_PUENTE";
        /// <summary>
        /// Initializes a new instance of the <see cref="BordeoDesignDatabase"/> class.
        /// </summary>
        public BordeoDesignDatabase() :
            base(DesignLine.Bordeo)
        {
        }
        /// <summary>
        /// Loads the design line data.
        /// </summary>
        /// <param name="conn">The connection.</param>
        /// <returns>
        /// The design line data result
        /// </returns>
        public override object InitDesignDatabase(DB_Connector conn)
        {
            BordeoDatabaseResult res = new BordeoDatabaseResult();
            this.InitSizes(res, conn);
            this.InitCodes(res, conn);
            return res;
        }
        /// <summary>
        /// Initializes the codes.
        /// </summary>
        private void InitCodes(BordeoDatabaseResult res, DB_Connector conn)
        {
            RivieraCode[] codes = new RivieraCode[]
            {
                 new RivieraCode(){ Code="BRSTACK", Description="Pila de paneles rectos", ElementType= RivieraElementType.PanelStack, Line= DesignLine.Bordeo },
                 new RivieraCode(){ Code="BRDSTACK", Description="Pila de paneles dobles", ElementType= RivieraElementType.PanelStack, Line= DesignLine.Bordeo },
                 new RivieraCode(){ Block ="BR2010", Code="BR2010", Description="Panel Recto", ElementType= RivieraElementType.Panel, Line= DesignLine.Bordeo },
                 new RivieraCode(){ Block ="BR2020", Code="BR2020", Description="Panel 90°", ElementType= RivieraElementType.Panel, Line= DesignLine.Bordeo },
                 new RivieraCode(){ Block ="BR2030", Code="BR2030", Description="Panel 135°", ElementType= RivieraElementType.Panel, Line= DesignLine.Bordeo },
                 new RivieraCode("01", "02", "03") { Block = "BR2040", Code = "BR2040", Description = "Puente 90°", ElementType = RivieraElementType.Bridge, Line = DesignLine.Bordeo },
                 new RivieraCode("01", "02", "03") { Block = "BR2050", Code = "BR2050", Description = "Puente 135°", ElementType = RivieraElementType.Bridge, Line = DesignLine.Bordeo },
                 new RivieraCode("01", "02", "03") { Block = "BR2060", Code = "BR2060", Description = "Pazo de Luz", ElementType = RivieraElementType.Pazo_Luz, Line = DesignLine.Bordeo }
            };
            res.Codes = codes.ToList();
        }

        /// <summary>
        /// Initializes the sizes.
        /// </summary>
        /// <param name="res">The resource.</param>
        /// <param name="conn">The connection.</param>
        /// <returns>Initialize the sizes</returns>
        private void InitSizes(BordeoDatabaseResult res, DB_Connector conn)
        {
            var sizes = new Dictionary<String, ElementSizeCollection>();
            var rivSizes = new RivieraMeasureRow[0].Union(
                RivieraMeasureRow.SelectAll<BordeoIPanelRow>(TABLENAME_PANEL_RECTO, conn)).Union(
                RivieraMeasureRow.SelectAll<BordeoLPanelRow>(TABLENAME_PANEL_L, conn)).Union(
                RivieraMeasureRow.SelectAll<BordeoPasoLuzRow>(TABLENAME_PASO_LUZ, conn)).Union(
                RivieraMeasureRow.SelectAll<BordeoPuenteRow>(TABLENAME_PUENTE, conn));
            foreach (var size in rivSizes)
            {
                if (!sizes.ContainsKey(size.Code))
                    sizes.Add(size.Code, new ElementSizeCollection(size.Code));
                sizes[size.Code].Sizes.Add(size.Measure);
            }
            res.Sizes = sizes;
        }
        /// <summary>
        /// Loads the design application model
        /// </summary>
        /// <param name="result"></param>
        public override void LoadDesignModelData(object result)
        {
            BordeoDatabaseResult res = (BordeoDatabaseResult)result;
            this.Sizes = res.Sizes;
            this.Codes = res.Codes;

        }
    }
}
