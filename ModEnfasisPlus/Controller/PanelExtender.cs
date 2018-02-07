using DaSoft.Riviera.OldModulador.Model;
using DaSoft.Riviera.OldModulador.UI;
using System;

namespace DaSoft.Riviera.OldModulador.Controller
{
    public static class PanelExtender
    {
        /// <summary>
        /// Selecciona el acabado para un elemento de tipo biombo
        /// </summary>
        /// <param name="biomboKind">El elemento de tipo biombo</param>
        /// <param name="acabado">Como parámetro de salida el acabado seleccionado</param>
        /// <param name="code">Como parámetro de salida el código seleccionado</param>
        public static Boolean SelectAcabado(this IBiomboable biomboKind, out String acabado, out String code)
        {
            PanelData data = (biomboKind.Data as PanelData);
            Boolean flag;
            String itemCode = data.Code;
            Dialog_AcabadoSelector sel = new Dialog_AcabadoSelector(itemCode);
            sel.ShowDialog();
            flag = sel.DialogResult.Value;
            if (flag)
            {
                acabado = sel.SelectedAcabado;
                code = biomboKind.GetCode() + acabado;
            }
            else
            {
                acabado = String.Empty;
                code = String.Empty;
            }
            return flag;
        }
        /// <summary>
        /// Obtienen el código para un elemento de tipo biombo.
        /// Aplica pichoneras, cajoneras y biombos
        /// </summary>
        /// <param name="biomboKind">El tipo de biombo</param>
        /// <returns>El código del elemento</returns>
        public static string GetCode(this IBiomboable biomboKind)
        {
            PanelData data = (biomboKind.Data as PanelData);
            double level = biomboKind.Height / Assets.RIVIERA_CONST.BIOMBO_SIZE_PX;
            String nivel = level.NivelToString();
            Double height = 0;
            if (data.Heights.ContainsKey(nivel))
                height = data.Heights[nivel];
            return string.Format("{0}{1:00}{2:00}", data.Code, data.Host_Front, height);
        }
        /// <summary>
        /// Selecciona el acabado para un elemento de tipo panel
        /// </summary>
        /// <param name="panelItem">El elemento de tipo panel</param>
        /// <param name="acabado">Como parámetro de salida el acabado seleccionado</param>
        /// <param name="code">Como parámetro de salida el código seleccionado</param>
        public static Boolean SelectAcabado(this Ctrl_PanelItem panelItem, out String acabado, out String code)
        {
            PanelData data = (panelItem.Data as PanelData);
            Boolean flag;
            String itemCode = data.Code;
            Dialog_AcabadoSelector sel = new Dialog_AcabadoSelector(itemCode);
            sel.ShowDialog();
            flag = sel.DialogResult.Value;
            if (flag)
            {
                acabado = sel.SelectedAcabado;
                code = panelItem.GetCode() + acabado;
            }
            else
            {
                acabado = String.Empty;
                code = String.Empty;
            }
            return flag;
        }
        /// <summary>
        /// Selecciona el acabado para un elemento de tipo panel
        /// </summary>
        /// <param name="panelItem">El elemento de tipo panel</param>
        /// <param name="acabado">Como parámetro de salida el acabado seleccionado</param>
        public static Boolean SelectAcabado(this Ctrl_PanelDataItem panelItem, out String acabado)
        {
            PanelData data = (panelItem.Data as PanelData);
            Boolean flag;
            String itemCode = data.Code;
            Dialog_AcabadoSelector sel = new Dialog_AcabadoSelector(itemCode);
            sel.ShowDialog();
            flag = sel.DialogResult.Value;
            if (flag)
                acabado = sel.SelectedAcabado;
            else
                acabado = String.Empty;
            return flag;
        }
        /// <summary>
        /// Obtienen el código para un elemento de tipo panel.
        /// </summary>
        /// <param name="panelItem">El elemento de tipo panel</param>
        /// <returns>El código del elemento</returns>
        public static string GetCode(this Ctrl_PanelItem panelItem)
        {
            PanelData data = (panelItem.Data as PanelData);
            double level = panelItem.Height / Assets.RIVIERA_CONST.NIVEL_SIZE_PX;
            String nivel = level.NivelToString();
            Double height;
            if (data.Heights.ContainsKey(nivel))
                height = data.Heights[nivel];
            height = data.Heights[nivel];
            if (panelItem.Text.Length == 12)
                return string.Format("{0}{1:00}{2:00}", data.Code, panelItem.Text.Substring(6, 4), height);
            else
                return string.Format("{0}{1:00}{2:00}", data.Code, data.Host_Front, height);

        }
    }
}
