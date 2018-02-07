using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using NamelessOld.Libraries.HoukagoTeaTime.Assets;
using NamelessOld.Libraries.Yggdrasil.Lilith;
using System;
using System.Linq;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Application;
namespace NamelessOld.Libraries.HoukagoTeaTime.Yui
{
    public class Selector : NamelessObject
    {
        /// <summary>
        /// Access the AutoCAD current editor
        /// </summary>
        public static Editor Ed { get { return AcadApp.DocumentManager.MdiActiveDocument.Editor; } }
        /// Format the msg prompt
        /// </summary>
        /// <param name="msg">The message to format</param>
        /// <returns>The msg formatted</returns>
        static String MsgFormat(String msg)
        {
            return String.Format("\n{0}", msg);
        }

        /// <summary>
        /// Select the objectId of an entity in the current drawing.
        /// </summary>
        /// <param name="msg">The selection message.</param>
        /// <param name="objId">The output of the selection.</param>
        /// <returns>True if the selection contains at least one objectId.</returns>
        public static bool ObjectId(String msg, out ObjectId objId)
        {
            bool flag = false;
            objId = new ObjectId();
            PromptEntityOptions opt = new PromptEntityOptions(MsgFormat(msg));
            opt.AllowNone = false;
            PromptEntityResult res = Ed.GetEntity(opt);
            if (res.Status == PromptStatus.OK)
            {
                objId = res.ObjectId;
                flag = true;
            }
            return flag;
        }
        /// <summary>
        /// Select the objectId of an entity in the current drawing.
        /// </summary>
        /// <param name="msg">The selection message.</param>
        /// <param name="objId">The output of the selection.</param>
        /// <returns>True if the selection contains at least one objectId.</returns>
        public static bool ObjectId<T>(String msg, out ObjectId objId) where T : Entity
        {
            bool flag = false;
            objId = new ObjectId();
            PromptEntityOptions opt = new PromptEntityOptions(MsgFormat(msg));
            opt.AllowNone = false;
            opt.SetRejectMessage(String.Format(Notices.RejectEntity, RXClass.GetClass(typeof(T)).DxfName));
            opt.AddAllowedClass(typeof(T), true);
            PromptEntityResult res = Ed.GetEntity(opt);
            if (res.Status == PromptStatus.OK)
            {
                objId = res.ObjectId;
                flag = true;
            }
            return flag;
        }
        /// <summary>
        /// Select the objectId of an entity in the current drawing.
        /// </summary>
        /// <param name="msg">The selection message.</param>
        /// <param name="objId">The output of the selection.</param>
        /// <returns>True if the selection contains at least one objectId.</returns>
        public static bool ObjectId(String msg, out ObjectId objId, params Type[] allowedTypes)
        {
            bool flag = false;
            objId = new ObjectId();
            PromptEntityOptions opt = new PromptEntityOptions(MsgFormat(msg));
            opt.AllowNone = false;
            String tpNames = String.Empty;
            allowedTypes.ToList().ForEach(x => tpNames += RXClass.GetClass(x).DxfName + ", ");
            tpNames = "[ " + tpNames.Substring(0, tpNames.Length - 2) + " ]";
            opt.SetRejectMessage(String.Format(Notices.RejectEntity, tpNames));
            allowedTypes.ToList().ForEach(x => opt.AddAllowedClass(x, true));
            PromptEntityResult res = Ed.GetEntity(opt);
            if (res.Status == PromptStatus.OK)
            {
                objId = res.ObjectId;
                flag = true;
            }
            return flag;
        }

        /// <summary>
        /// Select a group of objectIds in the current drawing.
        /// </summary>
        /// <param name="msg">The selection message.</param>
        /// <param name="ids">The output of the selection.</param>
        /// <returns>True if the selection contains at least one objectId.</returns>
        public static bool ObjectIds(String msg, out ObjectIdCollection ids)
        {
            PromptSelectionOptions opt = new PromptSelectionOptions();
            opt.MessageForAdding = msg;
            opt.MessageForRemoval = msg;
            PromptSelectionResult res = Ed.GetSelection(opt);
            ids = new ObjectIdCollection();
            if (res.Status == PromptStatus.OK)
                ids = new ObjectIdCollection(res.Value.GetObjectIds());
            return ids.Count > 0;
        }

        /// <summary>
        /// Invokes the command.
        /// </summary>
        /// <param name="cmdString">The command string.</param>
        /// <param name="echoCmd">if set to <c>true</c> [echo command].</param>
        /// <param name="wrapUpInactiveDocument">if set to <c>true</c> [wrap up inactive document].</param>
        public static void InvokeCMD(string cmdString, Boolean echoCmd = false, Boolean wrapUpInactiveDocument = false)
        {
            Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            if (doc != null)
                doc.SendStringToExecute(cmdString + " ", true, wrapUpInactiveDocument, echoCmd);
        }

        /// <summary>
        /// Select a group of objectIds in the current drawing.
        /// </summary>
        /// <param name="msg">The selection message.</param>
        /// <param name="ids">The output of the selection.</param>
        /// <param name="filter">The selection filter</param>
        /// <returns>True if the selection contains at least one objectId.</returns>
        public static bool ObjectIds(String msg, out ObjectIdCollection ids, SelectionFilter filter)
        {
            PromptSelectionOptions opt = new PromptSelectionOptions();
            opt.MessageForAdding = msg;
            opt.MessageForRemoval = msg;
            PromptSelectionResult res = Ed.GetSelection(opt, filter);
            ids = new ObjectIdCollection();
            if (res.Status == PromptStatus.OK)
                ids = new ObjectIdCollection(res.Value.GetObjectIds());
            return ids.Count > 0;
        }

        /// <summary>
        /// Select a point in the current drawing.
        /// </summary>
        /// <param name="msg">The selection message.</param>
        /// <param name="pt3D">The output of the current selection.</param>
        /// <param name="allowNone">Allow null inputs</param>
        /// <returns>True if the function return a value.</returns>
        public static bool Point(String msg, out Point3d pt3D, bool allowNone = false)
        {
            bool flag = false;
            pt3D = new Point3d();
            Editor ed = AcadApp.DocumentManager.MdiActiveDocument.Editor;
            PromptPointOptions opt = new PromptPointOptions(MsgFormat(msg));
            opt.AllowNone = allowNone;
            PromptPointResult res = ed.GetPoint(opt);
            if (res.Status == PromptStatus.OK)
            {
                pt3D = res.Value;
                flag = true;
            }
            return flag;
        }
        /// <summary>
        /// Select a point in the current drawing.
        /// </summary>
        /// <param name="msg">The selection message.</param>
        /// <param name="basePt">El punto base</param>
        /// <param name="pt3D">The output of the current selection.</param>
        /// <param name="allowNone">Allow null inputs</param>
        /// <returns>True if the function return a value.</returns>
        public static bool Point(String msg, Point3d basePoint, out Point3d pt3D, bool allowNone = false)
        {
            bool flag = false;
            pt3D = new Point3d();
            Editor ed = AcadApp.DocumentManager.MdiActiveDocument.Editor;
            PromptPointOptions opt = new PromptPointOptions(MsgFormat(msg));
            opt.AllowNone = allowNone;
            opt.BasePoint = basePoint;
            opt.UseBasePoint = true;
            opt.UseDashedLine = true;
            PromptPointResult res = ed.GetPoint(opt);
            if (res.Status == PromptStatus.OK)
            {
                pt3D = res.Value;
                flag = true;
            }
            return flag;
        }
    }
}
