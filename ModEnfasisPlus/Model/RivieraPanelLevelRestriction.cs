using System;

namespace DaSoft.Riviera.OldModulador.Model
{
    public class RivieraPanelLevelRestriction
    {
        /// <summary>
        /// El codigo del elemento analizar
        /// </summary>
        public String Code;
        /// <summary>
        /// La restricción de niveles
        /// </summary>
        public Boolean[] Restriction;
        /// <summary>
        /// Checa si el código tiene restricción para el nivel seleccionado
        /// </summary>
        /// <param name="level">El nivel a checar la validación</param>
        /// <returns>Verdadero si el código tiene una restricción para el nivel</returns>
        public Boolean IsRestricted(int level)
        {
            if (level == 0)
                return false;
            else
                return level <= Restriction.Length ? Restriction[level - 1] : false;
        }
        /// <summary>
        /// Crea un nuevo panel de descripción
        /// </summary>
        public RivieraPanelLevelRestriction()
        {
            Restriction = new Boolean[] { false, false, false, false };
        }
    }
}
