using System;
using System.Collections.Generic;
using UnityEngine;
using CMGTSA.Enemies;

namespace CMGTSA.FSM
{
    /// <summary>
    /// Base class for every FSM state. Concrete states override Enter / Step / Exit
    /// and add public methods to be used as transition predicates.
    /// </summary>
    public abstract class State
    {
        public Action onEnter;
        public Action onExit;

        [SerializeReference]
        public List<Transition> transitions = new List<Transition>();

        protected Enemy enemy;

        public virtual void Enter()
        {
            onEnter?.Invoke();
        }

        public State NextState()
        {
            for (int i = 0; i < transitions.Count; i++)
            {
                if (transitions[i].condition())
                {
                    return transitions[i].nextState;
                }
            }
            return null;
        }

        public virtual void Exit()
        {
            onExit?.Invoke();
        }

        public virtual void Step()
        {
        }
    }
}
