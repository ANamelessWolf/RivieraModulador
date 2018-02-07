using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using DaSoft.Riviera.OldModulador.Model;
using DaSoft.Riviera.OldModulador.Model.Delta;
using DaSoft.Riviera.OldModulador.Runtime;
using NamelessOld.Libraries.HoukagoTeaTime.Mio;
using NamelessOld.Libraries.HoukagoTeaTime.Mio.Entities;
using NamelessOld.Libraries.HoukagoTeaTime.MugiChan;
using NamelessOld.Libraries.HoukagoTeaTime.Runtime;
using NamelessOld.Libraries.Yggdrasil.Lain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static DaSoft.Riviera.OldModulador.Assets.RIVIERA_CONST;
using static DaSoft.Riviera.OldModulador.Assets.Strings;
namespace DaSoft.Riviera.OldModulador.Controller
{
    public static class RivieraExtender
    {
        public static String GetStringDirection(this ArrowDirection dir)
        {
            string d = String.Empty;
            switch (dir)
            {
                case ArrowDirection.Back:
                    d = "B";
                    break;
                case ArrowDirection.Front:
                    d = "F";
                    break;
                case ArrowDirection.Left_Front:
                case ArrowDirection.Left_Back:
                    d = "L";
                    break;
                case ArrowDirection.Right_Front:
                case ArrowDirection.Right_Back:
                    d = "R";
                    break;
            }
            return d;
        }
        /// <summary>
        /// Obtiene la letra segun el tipo de articulación
        /// seleccionada
        /// </summary>
        /// <param name="tp">El tipo de unión seleccionada</param>
        /// <returns>La letra seleccionada</returns>
        public static String GetLetter(this JointType tp)
        {
            string d = String.Empty;
            switch (tp)
            {
                case JointType.Joint_I:
                    d = "I";
                    break;
                case JointType.Joint_L:
                    d = "L";
                    break;
                case JointType.Joint_T:
                    d = "T";
                    break;
                case JointType.Joint_X:
                    d = "X";
                    break;
            }
            return d;
        }
        /// <summary>
        /// Convierte un enumerador de dirección a una cadena
        /// entendible por un mortal
        /// </summary>
        /// <param name="dir">La dirección</param>
        /// <returns>Un mensaje entendible</returns>
        public static String ToHumanReadable(this ArrowDirection dir)
        {
            string d = String.Empty;
            switch (dir)
            {
                case ArrowDirection.Back:
                    d = CAPTION_BACK;
                    break;
                case ArrowDirection.Front:
                    d = CAPTION_FRONT;
                    break;
                case ArrowDirection.Left_Front:
                    d = CAPTION_LEFT_FRONT;
                    break;
                case ArrowDirection.Left_Back:
                    d = CAPTION_LEFT_BACK;
                    break;
                case ArrowDirection.Right_Front:
                    d = CAPTION_RIGHT_FRONT;
                    break;
                case ArrowDirection.Right_Back:
                    d = CAPTION_RIGHT_BACK;
                    break;
            }
            return d;
        }

        public static String ToHumanReadable(this String field_name)
        {
            string d = String.Empty;
            if (FIELD_BACK == field_name)
                d = CAPTION_BACK;
            else if (FIELD_FRONT == field_name)
                d = CAPTION_FRONT;
            else if (FIELD_LEFT_FRONT == field_name)
                d = CAPTION_LEFT_FRONT;
            else if (FIELD_LEFT_BACK == field_name)
                d = CAPTION_LEFT_BACK;
            else if (FIELD_RIGHT_FRONT == field_name)
                d = CAPTION_RIGHT_FRONT;
            else if (FIELD_RIGHT_BACK == field_name)
                d = CAPTION_RIGHT_BACK;
            return d;
        }
        public static String ToHumanReadableSimple(this ArrowDirection dir)
        {
            string d = String.Empty;
            switch (dir)
            {
                case ArrowDirection.Back:
                    d = CAPTION_BACK;
                    break;
                case ArrowDirection.Front:
                    d = CAPTION_FRONT;
                    break;
                case ArrowDirection.Left_Front:
                case ArrowDirection.Left_Back:
                    d = CAPTION_LEFT;
                    break;
                case ArrowDirection.Right_Front:
                case ArrowDirection.Right_Back:
                    d = CAPTION_RIGHT;
                    break;
            }
            return d;
        }


        public static String GetString(this ArrowDirection dir)
        {
            string d = String.Empty;
            switch (dir)
            {
                case ArrowDirection.Back:
                    d = FIELD_BACK;
                    break;
                case ArrowDirection.Front:
                    d = FIELD_FRONT;
                    break;
                case ArrowDirection.Left_Front:
                    d = FIELD_LEFT_FRONT;
                    break;
                case ArrowDirection.Left_Back:
                    d = FIELD_LEFT_BACK;
                    break;
                case ArrowDirection.Right_Front:
                    d = FIELD_RIGHT_FRONT;
                    break;
                case ArrowDirection.Right_Back:
                    d = FIELD_RIGHT_BACK;
                    break;
            }
            return d;
        }

        public static List<PanelData> ExtractPanelData(this IEnumerable<PanelSize> sizes, String hostCode, double hostFront, double hostHeight)
        {
            List<PanelData> result = new List<PanelData>();
            var heighByLevel = App.DB.Alto_Nivel;
            var pDescriptions = App.DB.Description;
            //3: Creamos la lista de paneles
            PanelData panel;
            String desc, tp;
            String lev;
            RivieraCode rCode;
            foreach (PanelSize size in sizes)
            {
                panel = result.Where(x => x.Code == size.Code).FirstOrDefault();
                if (panel == null)
                {
                    rCode = pDescriptions.Where(x => x.Code.Trim() == size.Code.Trim()).FirstOrDefault();
                    desc = rCode.Description;
                    tp = rCode.Tipo;
                    panel = new PanelData()
                    {
                        Code = size.Code.Trim(),
                        Description = desc != null ? desc : CAPTION_NO_DESC,
                        Heights = new Dictionary<string, double>(),
                        HostCode = hostCode,
                        Host_Front = hostFront,
                        Host_Height = hostHeight,
                        Niveles = new List<string>(),
                        FrenteNominal = size.FrenteNominal,
                        Tipo = tp,
                        CanBeDouble = rCode.CanBeDouble
                    };
                    result.Add(panel);
                }
                lev = heighByLevel.Where(x => x.Type == ElementType.Panel && x.Alto == size.Nominal.Alto).FirstOrDefault().Nivel;
                if (lev != null && lev != String.Empty)
                {
                    panel.Niveles.Add(lev);
                    if (!panel.Heights.ContainsKey(lev))
                        panel.Heights.Add(lev, (long)size.Nominal.Alto);
                }
            }
            return result;
        }


        public static Boolean ArrowCollides(this Riviera2DArrow arrow, Transaction tr)
        {
            Boolean flag = false;
            if (arrow.Collides(tr))
            {
                arrow.Erase(tr);
                flag = true;
            }
            return flag;
        }
        #region Parser
        /// <summary>
        /// Realiza un parseo de las dimensión de la mampara leidas de la BD
        /// </summary>
        /// <param name="row">La fila de la BD</param>
        /// <returns>La dimensión de mampara</returns>
        public static MamparaSize ParseAsMampara(this String[] row)
        {
            return new MamparaSize(row);
        }
        /// <summary>
        /// Realiza un parseo de las dimensión del panel leido de la BD
        /// </summary>
        /// <param name="row">La fila de la BD</param>
        /// <returns>La dimensión de panel</returns>
        public static PanelSize ParseAsPanelData(this String[] row)
        {
            return new PanelSize(row);
        }
        /// <summary>
        /// Realiza un parseo de las descripción leidas de la BD
        /// </summary>
        /// <param name="row">La fila de la BD</param>
        /// <returns>La descripción como riviera code</returns>
        public static RivieraCode ParseAsRivieraCode(this String[] row)
        {
            return row.Length >= 5 ? new RivieraCode() { Bloque = row[2], Code = row[0], Description = row[1], Tipo = row[3], CanBeDouble = row[4] == "S" } :
                 new RivieraCode() { Bloque = String.Empty, Code = String.Empty, Description = String.Empty, Tipo = String.Empty, CanBeDouble = false };
        }
        /// <summary>
        /// Realiza un parseo de las restricción de panel-nivel leidas de la BD
        /// </summary>
        /// <param name="row">La fila de la BD</param>
        /// <returns>La restricción de inserción de panel</returns>
        public static RivieraPanelLevelRestriction ParseAsRivieraPanelLevelRestriction(this String[] row)
        {

            if (row.Length >= 5)
            {
                Boolean[] nR = new Boolean[4];
                for (int i = 1; i < row.Length; i++)
                    nR[i - 1] = row[i] == "N";
                return new RivieraPanelLevelRestriction() { Code = row[0], Restriction = nR };
            }
            else
                return new RivieraPanelLevelRestriction() { Code = String.Empty };
        }
        /// <summary>
        /// Realiza un parseo de las reglas de cuantificación de uniones leidas de la BD
        /// </summary>
        /// <param name="row">La fila de la BD</param>
        /// <returns>La restricción de inserción de panel</returns>
        public static MamparaUnionRule ParseAsMamparaUnionRule(this String[] row)
        {
            if (row.Length >= (6 + Query.Query_Uniones.MaxItems * 3))
            {
                return new MamparaUnionRule(row);
            }
            else
                return new MamparaUnionRule();
        }
        /// <summary>
        /// Convierte un angulo en radianes a grados
        /// </summary>
        /// <param name="ang">El angulo en radianes</param>
        /// <returns>El angulo en grados</returns>
        public static Double ToDegree(this Double ang)
        {
            ang = ang * 180 / Math.PI;
            return ang.To360Degree();
        }

        /// <summary>
        /// Convierte un angulo en grados a un valor en grados
        /// que este dentro del rango de [0 a 360)
        /// </summary>
        /// <param name="ang">El angulo en grados</param>
        /// <returns>El angulo en grados</returns>
        public static Double To360Degree(this Double ang)
        {
            while (ang >= 360)
                ang -= 360;
            while (ang < 0)
                ang = 360 + ang;
            return ang;
        }
        /// <summary>
        /// Selecciona el angulo que este más cerca del quartil seleccionado
        /// </summary>
        /// <param name="ang">El angulo obtenido</param>
        /// <returns>El cuartil del angulo seleccionado</returns>
        public static Double ToQuartilDegree(this Double ang)
        {
            Double[] angs = new Double[] { 0, 90, 180, 270 };
            return angs.OrderBy<Double, Double>(x => Math.Abs(x - ang)).FirstOrDefault();
        }
        /// <summary>
        /// Convierte un valor númerico de una unidad especifica a otra unidad
        /// </summary>
        /// <param name="currentUnits">La unidad actual</param>
        /// <param name="destUnits">La unidad destino</param>
        /// <param name="value">El valor númerico</param>
        /// <returns>The valor convertido a las nuevas unidades</returns>
        public static Double ConvertUnits(this Double value, Unit_Type currentUnits, Unit_Type destUnits)
        {
            //Los calculos se realizan siempre con las unidades internas
            value = value.ToInternalUnits(currentUnits);
            if (destUnits == Unit_Type.cm)
                value *= 100d;
            else if (destUnits == Unit_Type.mm)
                value *= 1000d;
            else if (destUnits == Unit_Type.inches)
                value *= IMPERIAL_FACTOR;
            else if (destUnits == Unit_Type.feet)
                value *= 3.2808399d;
            else if (destUnits == Unit_Type.GraphicUnits)
                value *= 100d;
            else if (destUnits == Unit_Type.ft2)
                value *= 10.7639104d;
            return value;
        }
        /// <summary>
        /// Convierte un tamaño de una unidad especifica a otra unidad
        /// </summary>
        /// <param name="currentUnits">La unidad actual</param>
        /// <param name="destUnits">La unidad destino</param>
        /// <param name="value">El valor númerico</param>
        /// <returns>The valor convertido a las nuevas unidades</returns>
        public static RivieraSize ConvertUnits(this RivieraSize value, Unit_Type currentUnits, Unit_Type destUnits)
        {
            //Los calculos se realizan siempre con las unidades internas
            return new RivieraSize()
            {
                Alto = value.Alto.ConvertUnits(currentUnits, destUnits),
                Ancho = value.Ancho.ConvertUnits(currentUnits, destUnits),
                Frente = value.Frente.ConvertUnits(currentUnits, destUnits)
            };
        }
        /// <summary>
        /// Convierte un valor númerico de una unidad especifica a las unidades internas
        /// la unidad interna son metros
        /// </summary>
        /// <param name="currentUnits">La unidad actual</param>
        /// <param name="value">El valor númerico</param>
        /// <returns>The valor convertido a unidades internas</returns>
        public static double ToInternalUnits(this Double value, Unit_Type currentUnits)
        {
            if (currentUnits == Unit_Type.cm || currentUnits == Unit_Type.GraphicUnits)
                value /= 100;
            else if (currentUnits == Unit_Type.mm)
                value /= 1000;
            else if (currentUnits == Unit_Type.inches)
                value /= 39.3700787;
            else if (currentUnits == Unit_Type.feet)
                value /= 3.2808399;
            else if (currentUnits == Unit_Type.ft2)
                value /= 10.7639104;
            else if (currentUnits == Unit_Type.GraphicSquareUnits)
                value /= 10000;
            return value;
        }
        #endregion

        #region Cuantificación
        /// <summary>
        /// Obtiene la zona de un objeto de AutoCAD
        /// </summary>
        /// <param name="obj">El objeto a extraer su zona</param>
        /// <param name="tr">La transacción activa</param>
        /// <returns>Regresa el nombre de la zona</returns>
        public static string GetZone(this Entity ent, Transaction tr)
        {
            String zoneName = String.Empty;
            DBDictionary extDic;
            DManager dMan;
            Xrecord xRec;
            string[] data;
            if (ent.ExtensionDictionary.IsValid)
            {
                extDic = ent.ExtensionDictionary.GetObject(OpenMode.ForRead) as DBDictionary;
                dMan = new DManager(extDic);
                if (dMan.TryGetRegistry(CAPTION_ZONE, out xRec, tr))
                {
                    data = xRec.GetDataAsString(tr);
                    if (data.Length > 0)
                        zoneName = data[0];
                }
            }
            return zoneName;
        }
        /// <summary>
        /// Obtiene la zona de un riviera object
        /// </summary>
        /// <param name="obj">El objeto a extraer su zona</param>
        /// <param name="tr">La transacción activa</param>
        /// <returns>Regresa el nombre de la zona</returns>
        public static string GetZone(this RivieraObject obj, Transaction tr)
        {
            String zoneName = String.Empty;
            Entity ent = obj.Ids[0].GetObject(OpenMode.ForRead) as Entity;
            DBDictionary extDic;
            DManager dMan;
            Xrecord xRec;
            string[] data;
            if (ent.ExtensionDictionary.IsValid)
            {
                extDic = ent.ExtensionDictionary.GetObject(OpenMode.ForRead) as DBDictionary;
                dMan = new DManager(extDic);
                if (dMan.TryGetRegistry(CAPTION_ZONE, out xRec, tr))
                {
                    data = xRec.GetDataAsString(tr);
                    if (data.Length > 0)
                        zoneName = data[0];
                }
            }
            return zoneName;
        }
        #endregion

        public static Point2d ParseAsPoint(this String str)
        {
            String[] coords = str.Substring(1, str.Length - 2).Split(',');
            return new Point2d(Double.Parse(coords[0]), Double.Parse(coords[1]));
        }
        public static RivieraSize ParseAsSize(this String str)
        {
            String[] val = str.Split('X');
            return new RivieraSize() { Frente = Double.Parse(val[0]), Alto = Double.Parse(val[1]), Ancho = Double.Parse(val[2]) };
        }
        /// <summary>
        /// Transforma el valor de un nivel en cadena a un
        /// valor númerico
        /// </summary>
        /// <param name="nivelStr">El nivel en cadena</param>
        /// <returns>El valor númerico del nivel</returns>
        public static Double NivelToValue(this string nivelStr)
        {
            Double num;
            if (nivelStr.Contains("/"))
                num = Double.Parse(nivelStr.Split(' ')[0]) + 0.5;
            else
                num = Double.Parse(nivelStr);
            return num;
        }
        /// <summary>
        /// Transforma el valor de un nivel númerico en cadena
        /// </summary>
        /// <param name="nivel">El nivel en valor númerico</param>
        /// <returns>El valor en formato de cadena</returns>
        public static String NivelToString(this Double nivel)
        {
            String num;

            if (nivel.ToString().Contains("."))
            {
                Boolean isZero = nivel.ToString().Split('.')[0] == "0";
                if (isZero)
                    num = "1/2";
                else
                    num = String.Format("{0} 1/2", nivel.ToString().Split('.'));
            }
            else
                num = nivel.ToString();
            return num;
        }
        /// <summary>
        /// Checa si existe un remate y lo remueve para continuar con el proceso de selección
        /// </summary>
        /// <param name="mampara">La mampara seleccionada</param>
        public static void CheckRemate(this Mampara mampara)
        {
            if (mampara.Remates != null)
            {
                FastTransactionWrapper ft = new FastTransactionWrapper(
                    delegate (Document doc, Transaction tr)
                    {
                        if (mampara.Remates != null)
                        {
                            List<long> rematesIds = mampara.Remates.Select<MamparaRemateFinal, long>(x => x.Handle.Value).ToList();
                            List<String> fields = mampara.Children.Where(x => rematesIds.Contains(x.Value)).Select<KeyValuePair<String, long>, String>(y => y.Key).ToList();
                            mampara.Remates.ForEach(x => x.Delete(tr));
                            mampara.Remates = null;
                            //Cache 
                            fields.ForEach(x => mampara.Children[x] = 0);
                            //Memoria
                            fields.ForEach(x => mampara.Data.Set(x, tr, "0"));
                        }
                    });
                ft.Run();
            }
        }
        /// <summary>
        /// Realiza una copia de un valor de PanelRaw
        /// </summary>
        /// <param name="raw">La información a copiar</param>
        public static PanelRaw Copy(this PanelRaw raw)
        {
            return new PanelRaw()
            {
                Acabado = raw.Acabado,
                APiso = raw.APiso,
                Block = raw.Block,
                Code = raw.Code,
                Direction = raw.Direction,
                Height = raw.Height,
                Nivel = raw.Nivel,
                Side = raw.Side
            };
        }

        /// <summary>
        /// Agregá un acabado a un código seleccionado
        /// </summary>
        /// <param name="code">El codigo seleccionado</param>
        /// <param name="obj">El objeto a cuantificar</param>
        /// <returns>El acabado del elemento seleccionado</returns>
        public static String AddAcabado(this string code, Object obj)
        {
            if (obj is Mampara || obj is MamparaRemateFinal)
                return code + "S";
            else if (obj is PanelRaw && (obj as PanelRaw).Acabado != String.Empty)
                return code + (obj as PanelRaw).Acabado;
            else if (obj is PanelRaw)
                return code + "T";
            else if (obj is RivieraBiombo)
                return code + ((obj as RivieraBiombo).Acabado != null ? (obj as RivieraBiombo).Acabado : "");
            else
                return code;
        }

        #region Geometry
        public static Polyline CreatePolyline(Boolean closed, params Point2d[] pts)
        {
            Polyline pl = new Polyline();
            pl.Closed = closed;
            for (int i = 0, index = 0; i < pts.Length; i++, index++)
                pl.AddVertexAt(index, pts[i], 0, 0, 0);
            return pl;
        }

        public static Point2d[] SubArray(this Point2d[] pts, int startIndex, int endIndex)
        {
            Point2d[] res = new Point2d[(endIndex - startIndex) + 1];
            for (int index = 0, i = startIndex; i <= endIndex; i++, index++)
                res[index] = pts[i];
            return res;
        }

        #endregion


        #region IBlockObject Methods
        /// <summary>
        /// Se encarga de seleccionar el nombre y el bloque activo
        /// El bloque activo cambia con la propiedad Is3DEnabled
        /// </summary>
        /// <param name="blkObject">El objeto con contenido de bloque</param>
        /// <param name="block">El archivo de bloque</param>
        /// <param name="blockName">El nombre del bloque</param>
        public static void ExtractBlockData(this IBlockObject obj, String code, out FileInfo block, out string blockName)
        {
            if (obj.Is3DEnabled)
            {
                block = obj.BlockFile3d;
                blockName = code + "3D";
            }
            else
            {
                block = obj.BlockFile2d;
                blockName = code + "2D";
            }
        }

        /// <summary>
        /// El nombre del bloque es igual al código base
        /// </summary>
        /// <param name="Code">El nombre del código</param>
        public static FileInfo BlockFile2D(this String code)
        {
            return App.Riviera.Delta2D.Where(x => x.Name.ToUpper() == String.Format("{0}.DWG", code).ToUpper()).FirstOrDefault();
        }
        /// <summary>
        /// El nombre del bloque es igual al código base
        /// El bloque 3D
        /// </summary>
        /// <param name="Code">El nombre del código</param>
        public static FileInfo BlockFile3D(this String code)
        {
            return App.Riviera.Delta3D.Where(x => x.Name.ToUpper() == String.Format("{0}.DWG", code).ToUpper()).FirstOrDefault();
        }

        /// <summary>
        /// Crea el contenido del objeto bloque
        /// </summary>
        /// <param name="obj">La interfaz para un bloque de riviera</param>
        /// <param name="code">El código del elemento</param>
        public static Boolean BlockContent(this IBlockObject obj, String code)
        {
            FileInfo block;
            String blockName;
            //1: Se realiza la selección del bloque segun el modo que se encuentre activo
            obj.ExtractBlockData(code, out block, out blockName);
            //2: Se realiza la carga de los bloques
            if (block != null && File.Exists(block.FullName))
            {
                //Este es el bloque cargado en el archivo, se va a dibujar en 
                //el bloque contenedor fantasma, codigo y prefijo SPACE_
                try
                {
                    obj.Block = new AutoCADBlock(blockName, block);
                    return true;
                }
                catch (Exception exc)
                {
                    Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("Error al cargar el bloque {0}\n{1}", blockName, exc.Message);
                    App.Riviera.Log.AppendEntry(exc.Message, Protocol.Error, "BlockContent");
                    return false;
                }

            }
            else
                return false;
        }
        /// <summary>
        /// Dibuja al objeto bloque
        /// </summary>
        /// <param name="code">El código del objeto</param>
        /// <param name="obj">La interfaz para un bloque de riviera</param>
        /// <param name="space">El espacio en donde esta el bloque</param>
        /// <param name="tr">La transacción actual</param>
        public static Boolean DrawBlockContent(this IBlockObject obj, AutoCADBlock space, String code, Transaction tr)
        {
            FileInfo block;
            String blockName;
            //1: Se realiza la selección del bloque segun el modo que se encuentre activo
            obj.ExtractBlockData(code, out block, out blockName);
            //2: Se realiza la carga de los bloques
            if (block != null && File.Exists(block.FullName) && space != null)
            {
                ObjectIdCollection coll = space.ListEntities(tr);
                ObjectId blockRefId;
                if (coll.Count == 0)
                    blockRefId = Drawer.Entity(obj.Block.CreateReference(new Point3d(), 0), obj.Spacename);
                else if (coll.Count > 0 && coll.Count != 1)
                {
                    space.Clear(tr);
                    blockRefId = Drawer.Entity(obj.Block.CreateReference(new Point3d(), 0), obj.Spacename);
                }
                else
                {
                    BlockReference blk = coll[0].OpenEntity(tr) as BlockReference;
                    if (blk.Name != blockName)
                    {
                        space.Clear(tr);
                        blockRefId = Drawer.Entity(obj.Block.CreateReference(new Point3d(), 0), obj.Spacename);
                    }
                }
                return true;
                
            }
            else
                return false;
        }
        #endregion
    }
}
