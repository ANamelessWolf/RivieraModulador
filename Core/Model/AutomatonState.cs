using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaSoft.Riviera.Modulador.Core.Model
{
    /// <summary>
    /// Defines an automaton state
    /// </summary>
    public abstract class AutomatonState<TransitionTask>
    {
        /// <summary>
        /// The automaton transition
        /// </summary>
        public int StateIndex;
        /// <summary>
        /// Gets a value indicating whether this instance is an end state.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is end state; otherwise, <c>false</c>.
        /// </value>
        public Boolean IsEndState
        {
            get { return this.Connections.Count == 0; }
        }
        /// <summary>
        /// The Automaton Paths
        /// </summary>
        protected Dictionary<ArrowDirection, TransitionTask> Transitions;
        /// <summary>
        /// The Automaton connected states
        /// </summary>
        protected Dictionary<ArrowDirection, int> Connections;
        /// <summary>
        /// Adds the transition.
        /// </summary>
        /// <param name="dir">The arrow direction.</param>
        /// <param name="nextState">The next state.</param>
        /// <param name="task">The transition task.</param>
        public abstract void AddTransition(ArrowDirection dir, int nextState, TransitionTask task);
        /// <summary>
        /// Initializes a new instance of the <see cref="AutomatonState{TransitionTask}"/> class.
        /// </summary>
        /// <param name="stateIndex">Index of the state.</param>
        public AutomatonState(int stateIndex)
        {
            this.StateIndex = stateIndex;
            this.Connections = new Dictionary<ArrowDirection, int>();
        }
    }
}
