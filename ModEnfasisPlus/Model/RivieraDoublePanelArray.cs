using Autodesk.AutoCAD.DatabaseServices;
using DaSoft.Riviera.OldModulador.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using static DaSoft.Riviera.OldModulador.Assets.Strings;
namespace DaSoft.Riviera.OldModulador.Model
{
    public class RivieraDoublePanelArray
    {
        /// <summary>
        /// El panel izquierdo
        /// </summary>
        public RivieraPanel Left;
        /// <summary>
        /// El biombo asociado a dos mamparas
        /// </summary>
        public RivieraPanel Biombo;
        /// <summary>
        /// El panel de la derecha
        /// </summary>
        public RivieraPanel Right;
        /// <summary>
        /// El panel doble
        /// </summary>
        public RivieraPanel DobleFront;
        /// <summary>
        /// El panel doble
        /// </summary>
        public RivieraPanel DobleBottom;
        /// <summary>
        /// Devuelve la colección de paneles disponibles
        /// </summary>
        /// <value>
        /// La colección de paneles
        /// </value>
        public IEnumerable<RivieraPanel> Panels
        {
            get
            {
                return new RivieraPanel[] { Left, Right, DobleBottom, DobleFront, Biombo }.Where(x => x != null);
            }
        }

        /// <summary>
        /// Devuelve el código que se guardara en la unión.
        /// </summary>
        /// <value>
        /// La información en el formato a guardar.
        /// </value>
        public string Data
        {
            get
            {
                string panelData;
                if (DobleBottom == null)
                    return CreateExtra(DobleBottom == null, this.Biombo != null ? this.Biombo.Handle.Value : 0, this.Left.Handle.Value, this.Right.Handle.Value, this.DobleFront.Handle.Value);
                else
                    return panelData = CreateExtra(DobleBottom == null, this.Biombo != null ? this.Biombo.Handle.Value : 0, this.Left.Handle.Value, this.Right.Handle.Value, this.DobleFront.Handle.Value, this.DobleBottom.Handle.Value);
            }
        }


        public static string CreateExtra(Boolean dobleBottom, long biombo, long left, long right, long front, long bottom = 0)
        {
            string panelData;
            if (dobleBottom)
                panelData = string.Format("{0}|{1}|{2}", left, right, front);
            else
                panelData = string.Format("{0}|{1}|{2}|{3}", left, right, front, bottom);
            if (biombo != 0)
                panelData = string.Format("{0}|{1}", panelData, biombo);
            return panelData;
        }

        /// <summary>
        /// Realiza el proceso de dibujo de los paneles seleccionados
        /// </summary>
        /// <param name="tr">La transacción activa.</param>
        public void Draw(Transaction tr)
        {
            Panels.ToList().ForEach(x => x.Draw(tr, null));
        }
        /// <summary>
        /// Realiza el proceso de guardado de apuntadores de unión
        /// </summary>
        /// <param name="joint">La unión a la que pertenece el arreglo de paneles dobles</param>
        public void Save(JointObject joint, Transaction tr)
        {
            if (this.DobleBottom != null)
            {
                this.DobleBottom.Parent = joint.Handle.Value;
                joint.PanelDoubleBottomId = this.DobleFront.Handle.Value;
                this.DobleBottom.Save(tr);
            }
            joint.PanelDoubleLeftId = this.Left.Handle.Value;
            joint.PanelDoubleRighttId = this.Right.Handle.Value;
            this.Left.Parent = joint.Handle.Value;
            this.Right.Parent = joint.Handle.Value;
            this.Left.Save(tr);
            this.Right.Save(tr);
            joint.PanelDoubleId = this.DobleFront.Handle.Value;
            this.DobleFront.Parent = joint.Handle.Value;
            this.DobleFront.Save(tr);
            if (this.Biombo != null)
            {
                //Biombo
                joint.BiomboId = this.Biombo.Handle.Value;
                this.Biombo.Parent = joint.Handle.Value;
                this.Biombo.Save(tr);
            }

            joint.Data.Set(FIELD_EXTRA, tr, joint.Extra);
            joint.Save(tr);

        }
        /// <summary>
        /// Realiza el proceso de agregar los paneles a la BD de 
        /// la aplicación
        /// </summary>
        public void Add()
        {
            var local = App.DB.Objects;
            if (this.DobleBottom != null)
                local.Add(this.DobleBottom);
            local.Add(this.Left);
            local.Add(this.Right);
            local.Add(this.DobleFront);
            if (this.Biombo != null)
                local.Add(this.Biombo);
        }

        public void Clean(JointObject joint, Transaction tr)
        {
            foreach (RivieraPanel panel in this.Panels)
                panel.Delete(tr);
            joint.HasDoublePanels = false;
            joint.PanelDoubleLeftId = 0;
            joint.PanelDoubleRighttId = 0;
            joint.PanelDoubleBottomId = 0;
            joint.PanelDoubleId = 0;
            joint.BiomboId = 0;
            joint.Save(tr);
        }

    }
}
