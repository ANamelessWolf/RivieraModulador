using Autodesk.AutoCAD.DatabaseServices;
using DaSoft.Riviera.OldModulador.Model.Delta;
using NamelessOld.Libraries.HoukagoTeaTime.Tsumugi;
using System;

namespace DaSoft.Riviera.OldModulador.Model
{
    public class RivieraPanel54 : RivieraPanel
    {
        /// <summary>
        /// La información cruda del panel superior
        /// </summary>
        public PanelRaw LowerRaw;
        /// <summary>
        /// La información cruda del panel
        /// </summary>
        public PanelRaw UpperRaw;
        /// <summary>
        /// Regresa la cadena actual del contenido guardado en el stack
        /// </summary>
        public override string PanelData
        {
            get
            {
                return this.Raw.ToRow + this.LowerRaw.ToRow + this.UpperRaw.ToRow;
            }
        }
        /// <summary>
        /// Iniicializa una instancia del panel 54 <see cref="RivieraPanel54"/> class.
        /// </summary>
        /// <param name="mampara">La mampara seleccionada.</param>
        /// <param name="panel">El panel a insertar en la mampara.</param>
        /// <param name="upperPanel">El panel superior.</param>
        /// <param name="location">La ubicación de los paneles.</param>
        public RivieraPanel54(Mampara mampara, PanelRaw panel, PanelRaw upperPanel, RivieraPanelDoubleLocation location) :
            base(mampara, panel, location)
        {
            //Se usa para guardar la información de la vista
            this.Raw = panel;
            //El panel inferior es 41
            this.LowerRaw =
                new PanelRaw()
                {
                    Acabado = this.Raw.Acabado,
                    APiso = this.Raw.APiso,
                    Block = this.Raw.Block,
                    Code = this.Raw.Code.Substring(0, 8) + 41,
                    Direction = this.Raw.Direction,
                    Height = this.Raw.Height,
                    Nivel = "3",
                    Side = this.Raw.Side
                };
            //El Panel superior es de 12
            this.UpperRaw = upperPanel;
            this.UpperRaw.Code = this.UpperRaw.Code.Substring(0, 8) + 12;
            this.UpperRaw.Direction = ArrowDirection.Front;
            this.UpperRaw.Nivel = "1";
        }

        public RivieraPanel54(Mampara mampara, PanelRaw panel, PanelRaw lowerPanel, PanelRaw upperPanel, RivieraPanelDoubleLocation location) :
            base(mampara, panel, location)
        {
            //Se usa para guardar la información de la vista
            this.Raw = panel;
            //El panel inferior es 41
            this.LowerRaw = lowerPanel;
            //El Panel superior es de 12
            this.UpperRaw = upperPanel;
        }

        /// <summary>
        /// Dibuja el contenido de la articulación de la mampara
        /// </summary>
        /// <param name="tr">La transacción activa</param>
        public override void SetAttributes(Transaction tr, AttManager attMan)
        {
            if (this.UpperRaw.Acabado != null && this.UpperRaw.Acabado != String.Empty)
                attMan.SetAttribute("A", this.UpperRaw.Acabado, tr);
            if (this.LowerRaw.Acabado != null && this.LowerRaw.Acabado != String.Empty)
                attMan.SetAttribute("B", this.LowerRaw.Acabado, tr);
        }
    }
}
