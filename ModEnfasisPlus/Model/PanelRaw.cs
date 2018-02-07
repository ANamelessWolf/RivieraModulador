using DaSoft.Riviera.OldModulador.Controller;
using DaSoft.Riviera.OldModulador.Model.Delta;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DaSoft.Riviera.OldModulador.Model
{
    public class PanelRaw
    {
        /// <summary>
        /// La dirección del panel.
        /// Un panel solo se inserta a la derecha o a la izquierda
        /// </summary>
        public ArrowDirection Direction;
        /// <summary>
        /// El código del panel a insertar
        /// </summary>
        public String Code;
        /// <summary>
        /// La altura del panel a insertar respecto al piso,
        /// en valor real
        /// </summary>
        public double Height;
        /// <summary>
        /// El nombre del bloque que se usa para insertar el panel
        /// </summary>
        public String Block;
        /// <summary>
        /// El tamaño de nivel que ocupa el panel
        /// </summary>
        public String Nivel;
        /// <summary>
        /// Checa si el panel es a piso
        /// </summary>
        public Boolean APiso;
        /// <summary>
        /// Checa si el panel es a piso
        /// </summary>
        public String Acabado;
        /// <summary>
        /// El lado en el que se inserto el panel
        /// </summary>
        public PanelSide Side;
        /// <summary>
        /// Accede al frente del panel
        /// </summary>
        public string Frente
        {
            get
            {
                return this.Code.Length > 8 ? this.Code.Substring(6, 2) : String.Empty;
            }
        }

        /// <summary>
        /// Convierte el formato actual a formato de fila
        /// </summary>
        public String ToRow
        {
            get
            {
                return String.Format("@{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}", this.Code, this.Height, this.Block, this.Nivel, (int)this.Direction, this.APiso, this.Acabado, (int)this.Side);
            }
        }

        /// <summary>
        /// La información actual del panel
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("{0}({1}) z:{2}", this.Code, this.Direction.ToHumanReadableSimple(), this.Height);
        }
        /// <summary>
        /// Realiza el parseo de las filas definidas
        /// </summary>
        /// <param name="data">La información obtenida</param>
        /// <returns>La colleción de paneles definidos</returns>
        public static PanelRaw[] Parse(string data)
        {
            List<PanelRaw> panels = new List<PanelRaw>();
            String[] rows = data.Split('@');
            String[] values;
            PanelRaw panel;
            foreach (String panelRow in rows.Where(x => x.Contains("|")))
            {
                values = panelRow.Split('|');
                panel = new PanelRaw()
                {
                    Code = values[0],
                    Height = double.Parse(values[1]),
                    Block = values[2],
                    Nivel = values[3],
                    Direction = (ArrowDirection)int.Parse(values[4]),
                    APiso = Boolean.Parse(values[5]),
                    Acabado = values[6],
                    Side = values.Length > 7 ? (PanelSide)int.Parse(values[7]) : PanelSide.DontCare,
                };
                panels.Add(panel);
            }
            return panels.ToArray();
        }
        /// <summary>
        /// Gets the panel data.
        /// </summary>
        /// <param name="paneles">The paneles.</param>
        /// <returns></returns>
        internal PanelData GetPanelData(List<PanelData> paneles)
        {
            PanelData data = paneles.Where(x => this.Code.Contains(x.Code)).FirstOrDefault();
            return data;
        }
        /// <summary>
        /// Devuelve el frente doble del panel
        /// </summary>
        /// <param name="mam1">La primer mampara.</param>
        /// <param name="mam2">La segundoa mampara.</param>
        /// <returns></returns>
        public static String GetPanelDoubleFrente(Mampara mam1, Mampara mam2)
        {
            String f1 = mam1.Code.Substring(6, 2),
                   f2 = mam2.Code.Substring(6, 2),
                   code = String.Format("{0:00}{1:00}", f1, f2);
            Tuple<String, String>[] doubleFrentes = new Tuple<String, String>[]
            {
                new Tuple<string, string>("1818","40"),
                new Tuple < String, String >( "2424", "52")
            };
            int index = doubleFrentes.Select(x => x.Item1).ToList().IndexOf(code);
            if (index != -1)
                return doubleFrentes[index].Item2;
            else
                return String.Empty;
        }
    }
}
