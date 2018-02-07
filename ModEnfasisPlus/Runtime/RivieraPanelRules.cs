using DaSoft.Riviera.OldModulador.Model;
using DaSoft.Riviera.OldModulador.UI;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using static DaSoft.Riviera.OldModulador.Assets.RIVIERA_CONST;
namespace DaSoft.Riviera.OldModulador.Runtime
{
    public class RivieraPanelRules
    {
        /// <summary>
        /// El código del panel a validar
        /// </summary>
        public String Code;
        /// <summary>
        /// El tamaño del panel a validar
        /// </summary>
        public Double Size;
        /// <summary>
        /// La pila que contienen alos paneles actuales
        /// </summary>
        public StackPanel Stack;
        /// <summary>
        /// La altura disponible de la mampara
        /// </summary>
        public Double AvailableHeight;
        /// <summary>
        /// El nivel actual de la mampara
        /// </summary>
        public int Level;

        /// <summary>
        /// Crea un conjunto de reglas para validar si un panel es valido en la colección
        /// </summary>
        /// <param name="size">El tamaño actual del panel</param>
        /// <param name="height">La altura que dispone la mampara</param>
        /// <param name="panelColl">La colleción de paneles insertados</param>
        public RivieraPanelRules(Double size, StackPanel panelColl, Double height)
        {
            this.Size = size;
            this.AvailableHeight = height;
            this.Stack = panelColl;
        }
        /// <summary>
        /// Crea un conjunto de reglas vacias
        /// </summary>
        /// <param name="panelColl">La colleción de paneles insertados</param>
        public RivieraPanelRules(StackPanel panelColl)
        {
            this.Stack = panelColl;
        }
        /// <summary>
        /// Obtiene el tamaño en pixeles de un nivel seleccionado
        /// </summary>
        /// <param name="nivel">El nivel seleccionado</param>
        /// <returns>El tamaño del nivel seleccionado</returns>
        public static double GetSize(String nivel)
        {
            Boolean half = nivel.Contains("/");
            Double size = 0;
            if (nivel == "0")
                size = FLOOR_OFFSET_PX;
            else if (half)
                size = nivel.Split(' ').Length == 2 ?
                    (double.Parse(nivel.Split(' ')[0]) + 0.5) * NIVEL_SIZE_PX : NIVEL_SIZE_PX / 2;
            else
                size = double.Parse(nivel) * NIVEL_SIZE_PX;
            return size;
        }

        /// <summary>
        /// Checa si el tamaño seleccionado es válido,
        /// en la colección de mampáras actuales
        /// </summary>
        public virtual Boolean IsValid()
        {
            return true;
        }
        /// <summary>
        /// Cambia el margen de un panel si es insertado desde el suelo
        /// </summary>
        /// <param name="item">El elemento del margen a insertar</param>
        /// <param name="insertFromFloor">Veradero si se inserta desde el suelo</param>
        public virtual void UpdateMargin(Ctrl_PanelItem item, Boolean insertFromFloor)
        {
            //Cambia el margen de un elemento insertado cuando es insertado desde el suelo
            if (this.Stack.Children.Count > 0)
                item.Margin = new Thickness(0, 0, 0, 0);
            else if (this.Stack.Children.Count == 0)
            {
                if (insertFromFloor)
                {
                    item.Margin = new Thickness(0, 0, 0, 0);
                    if (item.Height != FLOOR_OFFSET_PX)
                        item.Height += FLOOR_OFFSET_PX;
                }
                else
                    item.Margin = new Thickness(0, 0, 0, FLOOR_OFFSET_PX);
            }
        }
        /// <summary>
        /// Actualiza el status del elemento insertado
        /// </summary>
        /// <param name="insertFromFloor">Veradero si se inserta desde el suelo</param>
        public virtual void UpdateStatus(Boolean inFrFloor)
        {
            PanelStatus status = AvailableHeight == FLOOR_OFFSET_PX || AvailableHeight == 0 ?
                                 PanelStatus.Ok :
                                 PanelStatus.Error;
            //Resaltamos el elemento y lo seleccionamos como elemento activo
            foreach (Ctrl_PanelItem panel in this.Stack.Children)
                panel.Status = status;
        }
    }
}
