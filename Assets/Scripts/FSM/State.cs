using System;
using System.Collections.Generic;
using UnityEngine;

namespace CMGTSA.FSM
{
    /// <summary>
    /// Base class for every FSM state. Concrete states own their own actor reference
    /// (e.g. an enemy or a player controller) — the framework stays domain-free.
    /// </summary>
    public abstract class State
    {
        public Action onEnter;
        public Action onExit;

        [SerializeReference]
        public List<Transition> transitions = new List<Transition>();

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
