using DaSoft.Riviera.OldModulador.Model;
using DaSoft.Riviera.OldModulador.UI;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using static DaSoft.Riviera.OldModulador.Assets.Strings;

namespace DaSoft.Riviera.OldModulador.Runtime.Delta
{
    public class MamparaPanelRules : RivieraPanelRules
    {
        /// <summary>
        /// Crea un conjunto de reglas para validar si un panel es valido en la colección
        /// </summary>
        /// <param name="size">El tamaño actual del panel</param>
        /// <param name="height">La altura que dispone la mampara</param>
        /// <param name="panelColl">La colleción de paneles insertados</param>
        public MamparaPanelRules(Double size, StackPanel panelColl, Double height, String code) :
            base(size, panelColl, height)
        {
            this.Code = code;
        }
        /// <summary>
        /// Checa si la información de la mampara es válida
        /// </summary>
        /// <returns>Verdadero si la información es válida</returns>
        public override bool IsValid()
        {
            Boolean flag = false;
            if (this.Size != 0)
            {
                RivieraPanelLevelRestriction pR = App.DB.Panel_Restriction.Where(x => x.Code == this.Code).FirstOrDefault();
                Double diff = AvailableHeight - this.Size;
                Boolean isZeroDiff = diff == 0 || diff == DaSoft.Riviera.OldModulador.Assets.RIVIERA_CONST.FLOOR_OFFSET_PX,
                        isIntermedio = this.Size / DaSoft.Riviera.OldModulador.Assets.RIVIERA_CONST.NIVEL_SIZE_PX % 1 != 0,
                        isZoclo = this.Size == 20,
                        hasIntermedios = this.Stack.Children.OfType<Ctrl_PanelItem>().Count(x => x.Height == (DaSoft.Riviera.OldModulador.Assets.RIVIERA_CONST.NIVEL_SIZE_PX / 2)) > 0;

                if (AvailableHeight < this.Size)
                    Dialog_MessageBox.Show(String.Format(WAR_PAN_BAD_SIZE, this.Code), MessageBoxButton.OK, MessageBoxImage.Warning);
                else if (!isZeroDiff && hasIntermedios && !isZoclo)
                    Dialog_MessageBox.Show(WAR_PAN_LAST_HALF, MessageBoxButton.OK, MessageBoxImage.Warning);
                else if (pR != null)
                {
                    flag = !pR.IsRestricted(this.Level);
                    if (!flag)
                        Dialog_MessageBox.Show(String.Format(WARNING_LEVEL_RULE, this.Code, this.Level), MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                    flag = true;
            }
            return flag;
        }



    }
}
