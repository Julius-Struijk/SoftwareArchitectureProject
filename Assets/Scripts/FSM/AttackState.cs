using System;
using UnityEngine;
using CMGTSA.Enemies;

namespace CMGTSA.FSM
{
    /// <summary>
    /// Plays the attack tick once and reports completion after <see cref="Enemy.AttackInterval"/>.
    /// </summary>
    public class AttackState : State
    {
        private Transform transform;
        private Transform target;
        public static Action onAttack;
        private float attackStartTime;

        public AttackState(Transform pTransform, Transform pTarget, Enemy pEnemy)
        {
            transform = pTransform;
            target = pTarget;
            enemy = pEnemy;
        }

        public override void Enter()
        {
            base.Enter();
            Debug.Log("Entered Attack state.");
            attackStartTime = Time.time;
            onAttack?.Invoke();
            Debug.Log(transform.gameObject.name + " is attacking " + target.gameObject.name);
        }

        public bool FinishedAttacking()
        {
            return Time.time > attackStartTime + enemy.AttackInterval;
        }
    }
}
