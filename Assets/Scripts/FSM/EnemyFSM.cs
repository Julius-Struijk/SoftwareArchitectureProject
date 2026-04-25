using UnityEngine;
using UnityEngine.AI;
using CMGTSA.Enemies;

namespace CMGTSA.FSM
{
    /// <summary>
    /// Top-level enemy FSM: PatrolFSM ↔ SeekAndAttackFSM, switched by the per-enemy AlertLevel.
    /// </summary>
    public class EnemyFSM : FSM
    {
        public PatrolFSM patrolFSM;
        public SeekAndAttackFSM seekAndAttackFSM;

        public EnemyFSM(NavMeshAgent navMeshAgent, Enemy pEnemy)
        {
            enemy = pEnemy;

            patrolFSM = new PatrolFSM(navMeshAgent, enemy);
            seekAndAttackFSM = new SeekAndAttackFSM(navMeshAgent, enemy);

            patrolFSM.transitions.Add(new Transition(() => enemy.alertLevel == AlertLevel.ALERT, seekAndAttackFSM));
            seekAndAttackFSM.transitions.Add(new Transition(() => enemy.alertLevel == AlertLevel.NORMAL, patrolFSM));

            currentState = patrolFSM;
        }

        public override void Enter()
        {
            base.Enter();
            Debug.Log("Enemy FSM is active.");
            currentState.Enter();
        }
    }
}
