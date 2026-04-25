using UnityEngine;

namespace CMGTSA.FSM
{
    /// <summary>
    /// A state that idles for a certain period of time.
    /// </summary>
    public class IdleState : State
    {
        private float startTime;

        public IdleState(Enemy pEnemy)
        {
            //blackboard = pBlackboard;
            enemy = pEnemy;
        }

        public override void Enter()
        {
            base.Enter();
            Debug.Log("Entered Idle state.");
            startTime = Time.time;
        }

        public bool IdleTimeReached()
        {
            return Time.time > startTime + blackboard.waitingTime;
        }
    }
}
