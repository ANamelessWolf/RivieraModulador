using DaSoft.Riviera.OldModulador.Model;
using DaSoft.Riviera.OldModulador.Model.Delta;
using System;
using System.Linq;
using static DaSoft.Riviera.OldModulador.Assets.RIVIERA_CODE;
namespace DaSoft.Riviera.OldModulador.Controller
{

    /// <summary>
    /// Se encarga de realizar las cuantificaciones de fantasmas
    /// para el tipo de cuantificación de elemento
    /// </summary>
    public class FantasmaQuantifier
    {
        /// <summary>
        /// La colección de códigos fantasmas
        /// </summary>
        public static String[] Fantasmas = { "DD9010" };
        /// <summary>
        /// El código fantasma
        /// </summary>
        public String Code;
        /// <summary>
        /// EL objeto a cuantificar
        /// </summary>
        public RivieraObject QObject;
        /// <summary>
        /// The quantification zone
        /// </summary>
        public String Zone;
        /// <summary>
        /// Inicializa una nueva instancia de la clase<see cref="FantasmaQuantifier"/>.
        /// </summary>
        /// <param name="obj">El objeto a cuantificar que contiene un código fantasma</param>
        /// <param name="code">El código fantasma</param>
        /// <param name="zone">La zona del elemento a cuantificar</param>
        public FantasmaQuantifier(String code, RivieraObject obj, String zone)
        {
            this.Code = code;
            this.Zone = zone;
            this.QObject = obj;
        }
        /// <summary>
        /// Quantifies the specified item.
        /// </summary>
        public QuantifiableObject[] Quantify()
        {
            if (Code.Substring(0, 6) == Fantasmas[0])//DD9010
                return this.QuantifyDD9010();
            else
                return new QuantifiableObject[0];
        }
        /// <summary>
        /// Regla de cuantificación para mampara fantasma DD9010
        /// </summary>
        /// <returns>El objeto a cuantificar</returns>
        private QuantifiableObject[] QuantifyDD9010()
        {
            QuantifiableObject[] res = new QuantifiableObject[0];
            int num, frente = int.TryParse(this.Code.Substring(6, 2), out num) ? num : 0,
                height = int.TryParse(this.Code.Substring(8, 2), out num) ? num : 0;
            if (frente == 66)
            {
                res = new QuantifiableObject[2];
                res[0] = new QuantifiableObject()
                {
                    Code = String.Format("{0}{1}{2}", DT_MAMPARA, 30, height).AddAcabado(this.QObject),
                    Count = 1,
                    Visibility = true,
                    Zone = this.Zone,
                    Handle = this.QObject.Handle.Value
                };
                res[1] = new QuantifiableObject()
                {
                    Code = String.Format("{0}{1}{2}", DT_MAMPARA, 36, height).AddAcabado(this.QObject),
                    Count = 1,
                    Visibility = true,
                    Zone = this.Zone,
                    Handle = this.QObject.Handle.Value
                };
            }
            else if (frente == 72)
            {
                res = new QuantifiableObject[1];
                res[0] = new QuantifiableObject()
                {
                    Code = String.Format("{0}{1}{2}", DT_MAMPARA, 36, height).AddAcabado(this.QObject),
                    Count = 2,
                    Visibility = true,
                    Zone = this.Zone,
                    Handle = this.QObject.Handle.Value
                };
            }
            return res;

        }

        /// <summary>
        /// Checa si un código generado en la aplicación esta referenciado a un fantasma
        /// </summary>
        /// <param name="obj">El objeto a validar.</param>
        /// <param name="ghostCode">El código fantasma.</param>
        /// <returns>Verdaro si el código fantasma existe</returns>
        internal static bool CheckFantasma(RivieraObject obj, out string ghostCode)
        {
            String code = obj.Code;
            ghostCode = String.Empty;
            if (obj is Mampara)
            {
                String[] frentes = new String[] { "66", "72" };
                String[] heights = new String[] { "48", "54" };
                String frente = code != String.Empty ? code.Substring(6, 2) : String.Empty;
                String height = code != String.Empty ? code.Substring(8, 2) : String.Empty;
                if (frentes.Contains(frente) && heights.Contains(height))
                    ghostCode = String.Format("{0}{1}{2}", DT_MAMPARA_GHOST, frente, height);
            }
            return ghostCode.Length > 0;
        }
    }
}
