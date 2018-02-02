using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using DaSoft.Riviera.Modulador.Core.Model;
using DaSoft.Riviera.Modulador.Core.Runtime;
using Nameless.Libraries.HoukagoTeaTime.Mio.Entities;
using Nameless.Libraries.HoukagoTeaTime.Mio.Utils;
using Nameless.Libraries.HoukagoTeaTime.Ritsu.Utils;
using Nameless.Libraries.HoukagoTeaTime.Yui;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DaSoft.Riviera.Modulador.Core.Assets.CONST;
using static DaSoft.Riviera.Modulador.Core.Assets.Strings;
namespace DaSoft.Riviera.Modulador.Core.Controller
{
    public static class GeometryUtils
    {
        /// <summary>
        /// Regens the CAD object as line object
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="line">The line.</param>
        public static void RegenAsLine(this RivieraObject obj, ref Line line)
        {
            if (obj.CADGeometry == null)
                line = new Line(obj.Start.ToPoint3d(), obj.End.ToPoint3d());
            else
            {
                line.StartPoint = obj.Start.ToPoint3d();
                line.EndPoint = obj.End.ToPoint3d();
            }
        }
        /// <summary>
        /// Get the arrow direction by direction name
        /// </summary>
        /// <param name="directionName">The direction name</param>
        /// <returns>The arrow direction</returns>
        public static ArrowDirection GetArrowDirection(this string directionName)
        {
            ArrowDirection dir;
            switch (directionName)
            {
                case KEY_DIR_BACK:
                    dir = ArrowDirection.BACK;
                    break;
                case KEY_DIR_BACK_LFT:
                    dir = ArrowDirection.BACK_LEFT;
                    break;
                case KEY_DIR_BACK_LFT135:
                    dir = ArrowDirection.BACK_LEFT_135;
                    break;
                case KEY_DIR_BACK_LFT90:
                    dir = ArrowDirection.BACK_LEFT_90;
                    break;
                case KEY_DIR_BACK_RGT:
                    dir = ArrowDirection.BACK_RIGHT;
                    break;
                case KEY_DIR_BACK_RGT135:
                    dir = ArrowDirection.BACK_RIGHT_135;
                    break;
                case KEY_DIR_BACK_RGT90:
                    dir = ArrowDirection.BACK_RIGHT_90;
                    break;
                case KEY_DIR_FRONT:
                    dir = ArrowDirection.FRONT;
                    break;
                case KEY_DIR_FRONT_LFT:
                    dir = ArrowDirection.FRONT_LEFT;
                    break;
                case KEY_DIR_FRONT_LFT135:
                    dir = ArrowDirection.FRONT_LEFT_135;
                    break;
                case KEY_DIR_FRONT_LFT90:
                    dir = ArrowDirection.FRONT_LEFT_90;
                    break;
                case KEY_DIR_FRONT_RGT:
                    dir = ArrowDirection.FRONT_RIGHT;
                    break;
                case KEY_DIR_FRONT_RGT135:
                    dir = ArrowDirection.FRONT_RIGHT_135;
                    break;
                case KEY_DIR_FRONT_RGT90:
                    dir = ArrowDirection.FRONT_RIGHT_90;
                    break;
                case KEY_DIR_LFT:
                    dir = ArrowDirection.LEFT;
                    break;
                case KEY_DIR_RGT:
                    dir = ArrowDirection.RIGHT;
                    break;
                default:
                    dir = ArrowDirection.NONE;
                    break;
            }
            return dir;
        }
        /// <summary>
        /// Get the arrow direction name
        /// </summary>
        /// <param name="direction">The direction</param>
        /// <returns>The arrow direction name</returns>
        public static string GetArrowDirectionName(this ArrowDirection direction)
        {
            string dir;
            switch (direction)
            {
                case ArrowDirection.BACK:
                    dir = KEY_DIR_BACK;
                    break;
                case ArrowDirection.BACK_LEFT:
                    dir = KEY_DIR_BACK_LFT;
                    break;
                case ArrowDirection.BACK_LEFT_135:
                    dir = KEY_DIR_BACK_LFT135;
                    break;
                case ArrowDirection.BACK_LEFT_90:
                    dir = KEY_DIR_BACK_LFT90;
                    break;
                case ArrowDirection.BACK_RIGHT:
                    dir = KEY_DIR_BACK_RGT;
                    break;
                case ArrowDirection.BACK_RIGHT_135:
                    dir = KEY_DIR_BACK_RGT135;
                    break;
                case ArrowDirection.BACK_RIGHT_90:
                    dir = KEY_DIR_BACK_RGT90;
                    break;
                case ArrowDirection.FRONT:
                    dir = KEY_DIR_FRONT;
                    break;
                case ArrowDirection.FRONT_LEFT:
                    dir = KEY_DIR_FRONT_LFT;
                    break;
                case ArrowDirection.FRONT_LEFT_135:
                    dir = KEY_DIR_FRONT_LFT135;
                    break;
                case ArrowDirection.FRONT_LEFT_90:
                    dir = KEY_DIR_FRONT_LFT90;
                    break;
                case ArrowDirection.FRONT_RIGHT:
                    dir = KEY_DIR_FRONT_RGT;
                    break;
                case ArrowDirection.FRONT_RIGHT_135:
                    dir = KEY_DIR_FRONT_RGT135;
                    break;
                case ArrowDirection.FRONT_RIGHT_90:
                    dir = KEY_DIR_FRONT_RGT90;
                    break;
                case ArrowDirection.LEFT:
                    dir = KEY_DIR_LFT;
                    break;
                case ArrowDirection.RIGHT:
                    dir = KEY_DIR_RGT;
                    break;
                default:
                    dir = String.Empty;
                    break;
            }
            return dir;
        }
        /// <summary>
        /// Determines whether this instance is front.
        /// </summary>
        /// <param name="arrow">The arrow.</param>
        /// <returns>
        ///   <c>true</c> if the specified arrow is front; otherwise, <c>false</c>.
        /// </returns>
        public static Boolean IsFront(this ArrowDirection arrow)
        {
            return arrow.GetArrowDirectionName().Contains(KEY_DIR_FRONT);
        }
        /// <summary>
        /// Determines whether this instance is left.
        /// </summary>
        /// <param name="arrow">The arrow.</param>
        /// <returns>
        ///   <c>true</c> if the specified arrow is left; otherwise, <c>false</c>.
        /// </returns>
        public static Boolean IsLeft(this ArrowDirection arrow)
        {
            return arrow.GetArrowDirectionName().Contains(KEY_DIR_TAG_LFT);
        }
        /// <summary>
        /// Determines whether this instance is right.
        /// </summary>
        /// <param name="arrow">The arrow.</param>
        /// <returns>
        ///   <c>true</c> if the specified arrow is right; otherwise, <c>false</c>.
        /// </returns>
        public static Boolean IsRight(this ArrowDirection arrow)
        {
            return arrow.GetArrowDirectionName().Contains(KEY_DIR_TAG_RGT);
        }
        /// <summary>
        /// Determines whether this instance is back.
        /// </summary>
        /// <param name="arrow">The arrow.</param>
        /// <returns>
        ///   <c>true</c> if the specified arrow is back; otherwise, <c>false</c>.
        /// </returns>
        public static Boolean IsBack(this ArrowDirection arrow)
        {
            return arrow.GetArrowDirectionName().Contains(KEY_DIR_BACK);
        }
        /// <summary>
        /// Draws the arrow.
        /// </summary>
        /// <param name="arrow">The arrow direction.</param>
        /// <param name="blockDirPath">The Design line bloick directory path.</param>
        /// <param name="insertionPoint">The insertion point.</param>
        /// <param name="rotation">The arrwo rotation.</param>
        /// <param name="tr">The active transaction.</param>
        /// <returns>The drew arrow</returns>
        public static ObjectId DrawArrow(this ArrowDirection arrow, Point3d insertionPoint, Double rotation, String blockDirPath, Transaction tr)
        {
            //Se realiza la selección del archivo.
            FileInfo[] files = blockDirPath.GetMiscFiles();
            string arrowName = arrow.GetArrowDirectionName();
            FileInfo arrowFile = files.FirstOrDefault(x => x.Name.ToUpper() == String.Format("{0}.DWG", arrowName).ToUpper());
            if (arrowFile != null && arrowFile.Exists)
            {
                AutoCADBlock arrowBlock = new AutoCADBlock(String.Format(SUFFIX_ARROW, arrowName), arrowFile, tr);
                BlockTableRecord currentSpace = (BlockTableRecord)Application.DocumentManager.MdiActiveDocument.Database.CurrentSpaceId.GetObject(OpenMode.ForWrite);
                BlockReference blkRef = arrowBlock.CreateReference(insertionPoint, rotation, 1);
                return blkRef.Draw(currentSpace, tr);
            }
            else
                throw new RivieraException(String.Format(ERR_MISS_ARROW, arrowName, Path.Combine(blockDirPath, FOLDER_MISC)));
        }
        /// <summary>
        /// Creates the arrow.
        /// </summary>
        /// <param name="arrow">The arrow.</param>
        /// <param name="miscFiles">The misc files.</param>
        /// <param name="tr">The active transaction.</param>
        /// <returns>The arrow block</returns>
        public static AutoCADBlock CreateArrowBlock(this ArrowDirection arrow, FileInfo[] miscFiles, Transaction tr)
        {
            String arrowName = arrow.GetArrowDirectionName();
            FileInfo arrowFile = miscFiles.FirstOrDefault(x => x.Name.ToUpper() == String.Format("{0}.DWG", arrowName).ToUpper());
            AutoCADBlock arrowBlock = new AutoCADBlock(String.Format(SUFFIX_ARROW, arrowName), arrowFile, tr);
            return arrowBlock;
        }
        /// <summary>
        /// Gets the misc files.
        /// </summary>
        /// <param name="blockDirPath">The block dir path.</param>
        /// <returns></returns>
        public static FileInfo[] GetMiscFiles(this String blockDirPath)
        {
            //Se realiza la selección del archivo.
            String pth = Path.Combine(blockDirPath, FOLDER_MISC);
            FileInfo[] files;
            if (Directory.Exists(pth))
                files = new DirectoryInfo(pth).GetFiles();
            else
                files = new FileInfo[0];
            return files;
        }

        /// <summary>
        /// Draws the available arrows.
        /// </summary>
        /// <param name="rivieraEntity">The riviera entity.</param>
        /// <param name="arrow">The arrow direction.</param>
        /// <param name="blockDirPath">The Design line bloick directory path.</param>
        /// <param name="insertionPoint">The insertion point.</param>
        /// <param name="rotation">The arrwo rotation.</param>
        /// <param name="tr">The active transaction.</param>
        /// <returns>The drew the available arrows</returns>
        public static ObjectIdCollection DrawArrows(this ISowable rivieraEntity, Func<ArrowDirection, Boolean> filter, Point3d insertionPoint, Double rotation, String blockDirPath, Transaction tr)
        {
            ObjectIdCollection ids = new ObjectIdCollection();
            IEnumerable<ArrowDirection> arrows = rivieraEntity.GetAvailableDirections().Where(x => filter(x));
            arrows.ToList().ForEach(x => ids.Add(x.DrawArrow(insertionPoint, rotation, blockDirPath, tr)));
            return ids;
        }
        /// <summary>
        /// Gets the arrow direction.
        /// </summary>
        /// <returns>The selected arrow direction</returns>
        public static ArrowDirection GetDirection(this ISowable rivieraEntity, Transaction tr)
        {
            ObjectId id;
            if (Picker.ObjectId(MSG_SEL_ARROW_DIR, out id, typeof(BlockReference)))
            {
                BlockReference block = id.Open<BlockReference>(tr);
                string suffix = SUFFIX_ARROW.Replace("{0}", ""),
                    blkName = block.Name.Replace(suffix, "");
                return blkName.GetArrowDirection();
            }
            else
                return ArrowDirection.NONE;
        }
        /// <summary>
        /// Initializes the children data.
        /// </summary>
        /// <param name="supportedDir">The supported directions.</param>
        /// <param name="children">The children data.</param>
        public static void InitChildren(this ArrowDirection[] supportedDir, ref Dictionary<String, long> children)
        {
            string key;
            foreach (var arrow in supportedDir)
            {
                key = arrow.GetArrowDirectionName();
                if (!children.ContainsKey(key))
                    children.Add(key, 0);
            }
        }
    }
}
