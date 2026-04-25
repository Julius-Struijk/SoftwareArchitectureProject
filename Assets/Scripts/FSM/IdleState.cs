using UnityEngine;
using CMGTSA.Enemies;

namespace CMGTSA.FSM
{
    /// <summary>
    /// Idle for <see cref="Enemy.waitingTime"/> seconds, then a transition can fire.
    /// </summary>
    public class IdleState : State
    {
        private float startTime;

        public IdleState(Enemy pEnemy)
        {
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
            return Time.time > startTime + enemy.waitingTime;
        }
    }
}
