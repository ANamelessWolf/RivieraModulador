using DaSoft.Riviera.Modulador.Bordeo.Model;
using DaSoft.Riviera.Modulador.Core.Model;
using Nameless.Libraries.HoukagoTeaTime.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Nameless.Libraries.HoukagoTeaTime.Yui;
using DaSoft.Riviera.Modulador.Bordeo.UI;
using System.Windows.Controls;
using DaSoft.Riviera.Modulador.Bordeo.Model.Enities;
using DaSoft.Riviera.Modulador.Core.Controller;
using Nameless.Libraries.HoukagoTeaTime.Ritsu.Utils;
using System.IO;
using DaSoft.Riviera.Modulador.Core.Runtime;
using static DaSoft.Riviera.Modulador.Bordeo.Assets.Codes;
using static DaSoft.Riviera.Modulador.Bordeo.Assets.Constants;
using BordeoGeometryUtils = DaSoft.Riviera.Modulador.Core.Controller.GeometryUtils;
using Autodesk.AutoCAD.EditorInput;
using Nameless.Libraries.HoukagoTeaTime.Mio.Utils;
using DaSoft.Riviera.Modulador.Core.Controller.Transactions;
using DaSoft.Riviera.Modulador.Bordeo.Controller.Transactions;
using System.Windows.Media;

namespace DaSoft.Riviera.Modulador.Bordeo.Controller
{
    /// <summary>
    /// Defines the Bordeo Sower process
    /// </summary>
    public class BordeoSower : SowerController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BordeoSower"/> class.
        /// </summary>
        public BordeoSower(TabBordeoMenu bordeoMenu, int currentState = 0) :
            base(bordeoMenu, currentState)
        {

        }
        /// <summary>
        /// Defines the transition state matrix
        /// </summary>
        /// <returns>
        /// The transition matrix
        /// </returns>
        protected override SowAutomatonState[] InitTransitionMatrix()
        {
            SowAutomatonState[] automaton = SowAutomatonState.CreateAutomaton(4);
            //S0
            automaton[0].AddTransition(ArrowDirection.FRONT, 1, InsertLinearPanel);
            automaton[0].AddTransition(ArrowDirection.BACK, 1, InsertLinearPanel);
            automaton[0].AddTransition(ArrowDirection.FRONT_LEFT_90, 2, InsertLPanel);
            automaton[0].AddTransition(ArrowDirection.FRONT_LEFT_135, 2, InsertLPanel);
            automaton[0].AddTransition(ArrowDirection.FRONT_RIGHT_90, 2, InsertLPanel);
            automaton[0].AddTransition(ArrowDirection.FRONT_RIGHT_135, 2, InsertLPanel);
            automaton[0].AddTransition(ArrowDirection.BACK_LEFT_90, 2, InsertLPanel);
            automaton[0].AddTransition(ArrowDirection.BACK_LEFT_135, 2, InsertLPanel);
            automaton[0].AddTransition(ArrowDirection.BACK_RIGHT_90, 2, InsertLPanel);
            automaton[0].AddTransition(ArrowDirection.BACK_RIGHT_135, 2, InsertLPanel);
            automaton[0].AddTransition(ArrowDirection.NONE, 3, null);
            //S1
            automaton[1].AddTransition(ArrowDirection.FRONT, 1, InsertLinearPanel);
            automaton[1].AddTransition(ArrowDirection.BACK, 1, InsertLinearPanel);
            automaton[1].AddTransition(ArrowDirection.FRONT_LEFT_90, 2, InsertLPanel);
            automaton[1].AddTransition(ArrowDirection.FRONT_LEFT_135, 2, InsertLPanel);
            automaton[1].AddTransition(ArrowDirection.FRONT_RIGHT_90, 2, InsertLPanel);
            automaton[1].AddTransition(ArrowDirection.FRONT_RIGHT_135, 2, InsertLPanel);
            automaton[1].AddTransition(ArrowDirection.BACK_LEFT_90, 2, InsertLPanel);
            automaton[1].AddTransition(ArrowDirection.BACK_LEFT_135, 2, InsertLPanel);
            automaton[1].AddTransition(ArrowDirection.BACK_RIGHT_90, 2, InsertLPanel);
            automaton[1].AddTransition(ArrowDirection.BACK_RIGHT_135, 2, InsertLPanel);
            automaton[1].AddTransition(ArrowDirection.NONE, 3, null);
            //S2
            automaton[2].AddTransition(ArrowDirection.FRONT, 1, InsertLinearPanel);
            automaton[2].AddTransition(ArrowDirection.BACK, 1, InsertLinearPanel);
            automaton[2].AddTransition(ArrowDirection.NONE, 3, null);
            return automaton;
        }
        /// <summary>
        /// Sows the specified panel.
        /// </summary>
        /// <param name="panel">The panel measure.</param>
        public override void Sow()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            TabBordeoMenu ctrl = this.Menu as TabBordeoMenu;
            var data = ctrl.GetLinearPanels().ToArray();
            BordeoPanelStack panel0 = (BordeoPanelStack)new TransactionWrapper<RivieraMeasure, RivieraObject>(InitSowing).Run(data);
            ed.Regen();
            this.Sow(this.PickArrow(panel0, true, true), panel0);
        }
        /// <summary>
        /// Picks the riviera sizes.
        /// </summary>
        /// <param name="dir">The arrow direction.</param>
        /// <returns>
        /// The Riviera measure
        /// </returns>
        public override RivieraMeasure[] PickSizes(ArrowDirection dir)
        {
            TabBordeoMenu ctrl = this.Menu as TabBordeoMenu;
            RivieraMeasure[] sizes;
            if (dir == ArrowDirection.FRONT || dir == ArrowDirection.BACK)
                sizes = ctrl.GetLinearPanels().ToArray();
            else if (dir.GetArrowDirectionName().Contains("90"))
                sizes = ctrl.GetL90Panels().ToArray();
            else
                sizes = ctrl.GetL135Panels().ToArray();
            return sizes;
        }
        /// <summary>
        /// Picks the arrow.
        /// </summary>
        /// <param name="sowEntity">The sow entity.</param>
        /// <param name="rivObject">The riv object.</param>
        /// <param name="atStartPoint">if set to <c>true</c> [at start point].</param>
        /// <param name="atEndPoint">if set to <c>true</c> [at end point].</param>
        public override ArrowDirection PickArrow(ISowable sowEntity, Boolean atStartPoint = false, Boolean atEndPoint = false)
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            RivieraObject rivObject = (RivieraObject)sowEntity;
            var drewIds = BordeoAutoCADTransactions.DrawArrows(sowEntity, rivObject, atStartPoint, atStartPoint);
            ed.Regen();
            rivObject.ZoomAt(2.5d);
            ArrowDirection direction = AutoCADTransactions.PickArrowDirection(sowEntity, drewIds);
            ed.Regen();
            return direction;
        }
        /// <summary>
        /// Sows as Linear panel.
        /// </summary>
        /// <param name="doc">The active document.</param>
        /// <param name="tr">The active transaction.</param>
        protected override RivieraObject InitSowing(Document doc, Transaction tr, params RivieraMeasure[] sizes)
        {
            Point3d p0, pf;
            if (this.PickStartDirection(out p0, out pf))
            {
                //Se convierten los tamaños a tamaños de bordeo
                IEnumerable<PanelMeasure> pSizes = sizes.Select(x => x as PanelMeasure);
                //El panel inferios se usa como distancia base del arreglo de paneles.
                var first = sizes[0] as PanelMeasure;
                BordeoPanelStack stack = new BordeoPanelStack(p0, pf, first);
                //Se agregan los tamaños superiores
                for (int i = 1; i < sizes.Length; i++)
                    stack.AddPanel(sizes[i] as PanelMeasure);
                //Se dibuja el stack
                stack.Draw(tr);
                return stack;
            }
            else
                return null;
        }
        /// <summary>
        /// Sows as Linear panel.
        /// </summary>
        /// <param name="doc">The active document.</param>
        /// <param name="tr">The active transaction.</param>
        protected RivieraObject InsertLinearPanel(RivieraObject obj, ArrowDirection direction, params RivieraMeasure[] sizes)
        {
            //1: Se calcula la dirección y orientación del panel
            Point3d end, start;
            if (direction == ArrowDirection.FRONT) //Misma dirección
            {
                start = obj.End.ToPoint3d();
                end = start.ToPoint2d().ToPoint2dByPolar(1, obj.Angle).ToPoint3d();
            }
            else //Se invierte la dirección
            {
                start = obj.Start.ToPoint3d();
                end = start.ToPoint2d().ToPoint2dByPolar(1, obj.Angle + Math.PI).ToPoint3d();
            }
            //Se convierten los tamaños a tamaños de bordeo
            IEnumerable<PanelMeasure> pSizes = sizes.Select(x => x as PanelMeasure);
            //El panel inferios se usa como distancia base del arreglo de paneles.
            var first = sizes[0] as PanelMeasure;
            //Se crea el panel
            BordeoPanelStack stack = new BordeoPanelStack(start, end, first);
            //Se agregan los tamaños superiores
            for (int i = 1; i < sizes.Length; i++)
                stack.AddPanel(sizes[i] as PanelMeasure);
            //Se dibuja en el plano
            this.DrawObjects(null, stack);
            obj.Connect(direction, stack);
            //Se regresa el stack como objeto creado
            return stack;
        }
        /// <summary>
        /// Sows as L panel.
        /// </summary>
        /// <param name="doc">The active document.</param>
        /// <param name="tr">The active transaction.</param>
        protected RivieraObject InsertLPanel(RivieraObject obj, ArrowDirection direction, params RivieraMeasure[] sizes)
        {
            //1: Se calcula la dirección y orientación del panel
            Point3d end, start;
            //Siempre se inserta en la posición inicial del bloque insertado
            //Selección de la dirección
            start = obj.Start.ToPoint3d();
            end = direction.IsFront() ? start.ToPoint2d().ToPoint2dByPolar(1, obj.Angle).ToPoint3d() //Misma dirección
                                      : start.ToPoint2d().ToPoint2dByPolar(1, obj.Angle + Math.PI).ToPoint3d();//Dirección Invertida
            //Selección de la rotación
            SweepDirection rotation = direction.IsLeft() ? SweepDirection.Counterclockwise : SweepDirection.Clockwise;
            //Seleccion de tipo de L
            BordeoLPanelAngle lAng = direction.GetArrowDirectionName().Contains("90") ? BordeoLPanelAngle.ANG_90 : BordeoLPanelAngle.ANG_135;
            //Se convierten los tamaños a tamaños de bordeo
            IEnumerable<LPanelMeasure> pSizes = sizes.Select(x => x as LPanelMeasure);
            //El panel inferios se usa como distancia base del arreglo de paneles.
            var first = sizes[0] as LPanelMeasure;
            //Se crea el panel
            BordeoLPanelStack stack = new BordeoLPanelStack(start, end, first, rotation, lAng);
            //Se agregan los tamaños superiores
            for (int i = 1; i < sizes.Length; i++)
                stack.AddPanel(sizes[i] as LPanelMeasure);
            //Se dibuja en el plano y se borra el panel anterior
            RivieraObject objParent = obj.GetParent();
            this.DrawObjects((Document doc, Transaction tr, RivieraObject[] objs) => obj.Delete(tr));
            if (objParent != null)
                objParent.Connect(direction, stack);
            //Se regresa el stack como objeto creado
            return stack;
        }
    }
}
