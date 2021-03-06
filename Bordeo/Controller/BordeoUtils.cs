﻿using DaSoft.Riviera.Modulador.Bordeo.Model;
using DaSoft.Riviera.Modulador.Bordeo.Runtime;
using DaSoft.Riviera.Modulador.Core.Model;
using DaSoft.Riviera.Modulador.Core.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DaSoft.Riviera.Modulador.Core.Assets.CONST;
using static DaSoft.Riviera.Modulador.Bordeo.Assets.Codes;
using static DaSoft.Riviera.Modulador.Bordeo.Assets.Constants;
using static DaSoft.Riviera.Modulador.Bordeo.Assets.Strings;
using Nameless.Libraries.Yggdrasil.Lilith;
using System.IO;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.DatabaseServices;
using Nameless.Libraries.HoukagoTeaTime.Ritsu.Utils;
using DaSoft.Riviera.Modulador.Bordeo.Model.Enities;

namespace DaSoft.Riviera.Modulador.Bordeo.Controller
{
    public static partial class BordeoUtils
    {
        /// <summary>
        /// Gets the block directory path.
        /// </summary>
        /// <value>
        /// The block directory path.
        /// </value>
        public static String BlockDirectoryPath => Path.Combine(App.Riviera.AppDirectory.FullName, FOLDER_NAME_BLOCKS_BORDEO);
        /// <summary>
        /// Bordeoes the direction keys.
        /// </summary>
        /// <returns></returns>
        public static String[] BordeoDirectionKeys()
        {
            return new String[]
            {
               // KEY_BACK, KEY_FRONT, KEY_LEFT_135, KEY_LEFT_90, KEY_RIGHT_135, KEY_RIGHT_90, KEY_EXTRA
            };
        }
        /// <summary>
        /// Bordeoes the direction keys.
        /// </summary>
        /// <returns></returns>
        public static String[] BordeoLPanelDirectionKeys()
        {
            return new String[]
            {
               KEY_DIR_FRONT, KEY_DIR_BACK
            };
        }
        /// <summary>
        /// Gets the database for the design line bordeo.
        /// </summary>
        /// <returns>The bordeo database</returns>
        public static BordeoDesignDatabase GetDatabase()
        {
            var rivApp = App.Riviera;
            if (rivApp != null && rivApp.Database != null && rivApp.Database.LineDB.ContainsKey(DesignLine.Bordeo))
                return rivApp.Database.LineDB[DesignLine.Bordeo] as BordeoDesignDatabase;
            else
                throw new BordeoException(ERR_DB_NOT_READY);
        }
        /// <summary>
        /// Gets the database for the design line bordeo.
        /// </summary>
        /// <param name="code">The riviera code</param>
        /// <returns>The bordeo database</returns>
        public static RivieraCode GetRivieraCode(this String code)
        {
            try
            {
                //This codes are logic and not defined in the database
                RivieraCode[] logicCodes = new RivieraCode[]
                {
                    new RivieraCode() { Code = CODE_STATION, Description = "Estación de línea bordeo", ElementType = RivieraElementType.Station, Line = DesignLine.Bordeo },
                    new RivieraCode() { Code = CODE_PANEL_STACK, Description = "Pila de paneles rectos", ElementType = RivieraElementType.PanelStack, Line = DesignLine.Bordeo },
                    new RivieraCode() { Code = CODE_DPANEL_STACK, Description = "Pila de paneles dobles", ElementType = RivieraElementType.PanelStack, Line = DesignLine.Bordeo }
                };
                if (logicCodes.Select(x => x.Code).Contains(code))
                    return logicCodes.FirstOrDefault(x => x.Code == code);
                else
                {
                    var db = GetDatabase();
                    var rivCode = db.Codes.FirstOrDefault(x => x.Code == code);
                    if (rivCode == null)
                        throw new BordeoException(String.Format(ERR_CODE_NOT_FOUND, code));
                    return rivCode;
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        /// <summary>
        /// Gets the end point.
        /// </summary>
        /// <param name="geometry">The end point of the geometry.</param>
        /// <returns>The end point of the geometry</returns>
        public static Point2d GetEndPoint(this Line geometry, PanelMeasure size)
        {
            Point2d start = geometry.StartPoint.ToPoint2d(),
                end = geometry.EndPoint.ToPoint2d();
            Double angle = start.GetVectorTo(end).Angle;
            Double distance = size.Frente.Real;
            return start.ToPoint2dByPolar(distance, angle);
        }
        /// <summary>
        /// Gets the heights.
        /// </summary>
        /// <param name="panelHeight">Height of the panel.</param>
        /// <param name="panelHeight27">The panel height27.</param>
        /// <param name="panelHeight15">The panel height15.</param>
        /// <returns>The Riviera Height sizes</returns>
        public static RivieraSize[] GetHeights(this BordeoPanelHeight panelHeight, RivieraSize panelHeight27, RivieraSize panelHeight15)
        {
            RivieraSize[] heights;
            switch (panelHeight)
            {
                case BordeoPanelHeight.NormalMini:
                    heights = new RivieraSize[] { panelHeight27, panelHeight15 };
                    break;
                case BordeoPanelHeight.NormalMiniNormal:
                    heights = new RivieraSize[] { panelHeight27, panelHeight15, panelHeight27 };
                    break;
                case BordeoPanelHeight.NormalThreeMini:
                    heights = new RivieraSize[] { panelHeight27, panelHeight15, panelHeight15, panelHeight15 };
                    break;
                case BordeoPanelHeight.NormalTwoMinis:
                    heights = new RivieraSize[] { panelHeight27, panelHeight15, panelHeight15 };
                    break;
                case BordeoPanelHeight.ThreeNormals:
                    heights = new RivieraSize[] { panelHeight27, panelHeight27, panelHeight27 };
                    break;
                case BordeoPanelHeight.TwoNormalOneMini:
                    heights = new RivieraSize[] { panelHeight27, panelHeight27, panelHeight15 };
                    break;
                case BordeoPanelHeight.TwoNormals:
                    heights = new RivieraSize[] { panelHeight27, panelHeight27 };
                    break;
                default:
                    heights = new RivieraSize[0];
                    break;
            }
            return heights;
        }
        /// <summary>
        /// Moves the objects via a displacement vector
        /// </summary>
        /// <param name="rivObj">The riviera object.</param>
        /// <param name="objs">The connected riviera objects.</param>
        /// <param name="displacementVector">The displacement vector.</param>
        /// <param name="tr">The active transaction.</param>
        public static void MoveObjects(this RivieraObject rivObj, IEnumerable<RivieraObject> objs, Vector3d v, Transaction tr)
        {
            if (!(rivObj is BordeoPanelStack || rivObj is BordeoLPanelStack))
                throw new BordeoException(String.Format(ERR_NOT_BORDERO, "mover"));
            foreach (RivieraObject obj in objs)
            {
                obj.Move(tr, v);
                obj.Start = obj.Start.ToPoint3d().TransformBy(Matrix3d.Displacement(v)).ToPoint2d();
                if (obj is BordeoPanelStack)
                    foreach (var panel in (obj as BordeoPanelStack))
                        panel.Start = obj.Start;
                if (obj is BordeoLPanelStack)
                    foreach (var panel in (obj as BordeoLPanelStack))
                        panel.Start = obj.Start;
            }
        }

    }
}
