using DaSoft.Riviera.Modulador.Bordeo.Model.Enities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaSoft.Riviera.Modulador.Bordeo.Model.Station
{
    public class StationMoldure
    {
        /// <summary>
        /// La distancia final
        /// </summary>
        const Double EndLDistance = 0.2228d;
        const Double EndIDistance = 0.222d;
        /// <summary>
        /// La longitud de la moldura
        /// </summary>
        Double Length;
        /// <summary>
        /// Inicializa una nueva indtancia de la clase<see cref="StationMoldure"/>.
        /// </summary>
        /// <param name="token">El token de estación de entrada</param>
        public StationMoldure(StationToken token)
        {
            if (token == StationToken.Istart || token == StationToken.Ifinal)
                this.Length = (token.Stack as BordeoPanelStack).PanelGeometry.Length- EndIDistance;
            else if (token == StationToken.L90start || token == StationToken.L90final)
                this.Length = (token.Stack as BordeoL90Panel).PanelGeometry.Length - EndIDistance;
            else if (token == StationToken.L135start || token == StationToken.L135plus)
                this.Length = (token.Stack as BordeoL135Panel).PanelGeometry.Length - EndIDistance;

        }
    }
}
