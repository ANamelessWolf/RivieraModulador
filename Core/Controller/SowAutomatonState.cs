using DaSoft.Riviera.Modulador.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DaSoft.Riviera.Modulador.Core.Controller.SowerController;

namespace DaSoft.Riviera.Modulador.Core.Controller
{
    /// <summary>
    /// Defines an automaton sow state
    /// </summary>
    /// <seealso cref="AutomatonState{SowerController.SowObjectHandler}" />
    public class SowAutomatonState : AutomatonState<SowObjectHandler>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SowAutomatonState"/> class.
        /// </summary>
        /// <param name="stateIndex">Index of the state.</param>
        public SowAutomatonState(int stateIndex) 
            : base(stateIndex)
        {
            this.Transitions = new Dictionary<ArrowDirection, SowObjectHandler>();
        }
        /// <summary>
        /// Adds the transition.
        /// </summary>
        /// <param name="dir">The arrow direction.</param>
        /// <param name="nextState">The next state.</param>
        /// <param name="task">The transition task.</param>
        public override void AddTransition(ArrowDirection dir, int nextState, SowObjectHandler task)
        {
            this.Transitions.Add(dir, task);
            this.Connections.Add(dir, nextState);
        }
        /// <summary>
        /// Translates between nodes with the specified token.
        /// </summary>
        /// <param name="token">The transition token.</param>
        /// <param name="nextState">The transition end state.</param>
        /// <param name="inputObject">The input object.</param>
        /// <param name="sizes">The Riviera given sizes.</param>
        /// <returns>The sowed element</returns>
        public RivieraObject Transition(ArrowDirection token, out int nextState, RivieraObject inputObject, params RivieraMeasure[] sizes)
        {
            RivieraObject obj = null;
            if (Transitions.ContainsKey(token))
            {
                nextState = Connections[token];
                obj = Transitions[token](inputObject, token, sizes);
            }
            else
                nextState = Connections[ArrowDirection.NONE];
            return obj;
        }
        /// <summary>
        /// Creates the automaton with a given size
        /// </summary>
        /// <param name="size">The automaton size.</param>
        /// <returns>The automation size</returns>
        public static SowAutomatonState[] CreateAutomaton(int size)
        {
            SowAutomatonState[] automaton = new SowAutomatonState[size];
            for (int i = 0; i < automaton.Length; i++)
                automaton[i] = new SowAutomatonState(i);
            return automaton;
        }
    }
}
