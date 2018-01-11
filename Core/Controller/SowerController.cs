using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using DaSoft.Riviera.Modulador.Core.Model;
using Nameless.Libraries.HoukagoTeaTime.Runtime;
using Nameless.Libraries.HoukagoTeaTime.Yui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using static DaSoft.Riviera.Modulador.Core.Assets.Strings;
namespace DaSoft.Riviera.Modulador.Core.Controller
{
    /// <summary>
    /// Defines the sowing process
    /// </summary>
    public abstract class SowerController
    {
        /// <summary>
        /// Gets or sets the menu.
        /// </summary>
        /// <value>
        /// The menu.
        /// </value>
        public UserControl Menu { get; set; }
        /// <summary>
        /// Gets the transition matrix.
        /// </summary>
        /// <value>
        /// The transition matrix.
        /// </value>
        public SowAutomatonState[] TransitionMatrix;
        /// <summary>
        /// The current state
        /// </summary>
        public int CurrentState;
        /// <summary>
        /// Defines the delegates that sows an element
        /// </summary>
        /// <param name="obj">The object to sow.</param>
        /// <param name="direction">The sowing direction.</param>
        /// <param name="sizes">The new object sizes.</param>
        /// <returns>The new sowed object</returns>
        public delegate RivieraObject SowObjectHandler(RivieraObject obj, ArrowDirection direction, params RivieraMeasure[] sizes);
        /// <summary>
        /// Defines the initial sowing process
        /// </summary>
        /// <param name="doc">The active document.</param>
        /// <param name="tr">The active transaction.</param>
        /// <param name="sizes">The initial transaction sizes.</param>
        /// <returns>The first sow element</returns>
        protected abstract RivieraObject InitSowing(Document doc, Transaction tr, params RivieraMeasure[] sizes);
        /// <summary>
        /// Picks the arrow.
        /// </summary>
        /// <param name="sowEntity">The sow entity.</param>
        /// <param name="atStartPoint">if set to <c>true</c> [at start point].</param>
        /// <param name="atEndPoint">if set to <c>true</c> [at end point].</param>
        public abstract ArrowDirection PickArrow(ISowable sowEntity, Boolean atStartPoint = false, Boolean atEndPoint = false);
        /// <summary>
        /// Initializes the transition matrix.
        /// </summary>
        /// <returns>The transition matrix</returns>
        protected abstract SowAutomatonState[] InitTransitionMatrix();
        /// <summary>
        /// Initializes a new instance of the <see cref="SowerController"/> class.
        /// </summary>
        /// <param name="control">The user control asociated to this sower.</param>
        public SowerController(UserControl control, int currentState = 0)
        {
            this.TransitionMatrix = this.InitTransitionMatrix();
            this.Menu = control;
            this.CurrentState = currentState;
        }
        /// <summary>
        /// Initialize the sowing process
        /// </summary>
        public abstract void Sow();
        /// <summary>
        /// Picks the riviera sizes.
        /// </summary>
        /// <param name="dir">The arrow direction.</param>
        /// <returns>The Riviera measure</returns>
        public abstract RivieraMeasure[] PickSizes(ArrowDirection dir);
        /// <summary>
        /// Picks the start direction.
        /// </summary>
        /// <param name="p0">The initial point.</param>
        /// <param name="pf">The end point.</param>
        /// <returns>True if the direction is selected</returns>
        public Boolean PickStartDirection(out Point3d p0, out Point3d pf)
        {
            Point3d pt0 = new Point3d(),
                    ptf = new Point3d();
            bool flag = Picker.Point(String.Format(MSG_SEL_POINT, CAP_INI), out pt0) &&
                  Picker.Point(String.Format(MSG_SEL_POINT, CAP_LAST), pt0, out ptf);
            p0 = pt0;
            pf = ptf;
            return flag;
        }
        /// <summary>
        /// Sows in the specified direction.
        /// </summary>
        /// <typeparam name="T">The user interface menu type</typeparam>
        /// <param name="dir">The sowing direction.</param>
        /// <param name="menu">The user menu.</param>
        /// <param name="baseObject">The base object.</param>
        /// <param name="startState">The start state.</param>
        public void Sow(ArrowDirection dir, RivieraObject baseObject, int startState = 0) 
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            RivieraObject obj = baseObject;
            this.CurrentState = startState;
            while (dir != ArrowDirection.NONE)
            {
                obj = this.TransitionMatrix[this.CurrentState].Transition(dir, out this.CurrentState, obj, this.PickSizes(dir));
                ed.Regen();
                dir = this.PickArrow(obj as ISowable);
            }
        }
        /// <summary>
        /// Draws the objects.
        /// </summary>
        /// <param name="task">Costume task that occurs after the objects are drawn</param>
        /// <param name="objects">The riviera objects to draw.</param>
        public virtual void DrawObjects(Action<Document, Transaction, RivieraObject[]> task, params RivieraObject[] objects)
        {
            new VoidTransactionWrapper<RivieraObject>(
                (Document doc, Transaction tr, RivieraObject[] objs) =>
                {
                    foreach (var obj in objs)
                        obj.Draw(tr);
                    if (task != null)
                        task(doc, tr, objs);
                }).Run(objects);
        }
    }
}
