using CMGTSA.FSM;
using FsmBase = CMGTSA.FSM.FSM;

namespace CMGTSA.Player
{
    /// <summary>
    /// Top-level player FSM. Wires the five concrete states together. Transition order
    /// is priority-sensitive (NextState returns the first matching transition):
    /// 1. Dead beats everything (HP == 0)
    /// 2. Hurt beats normal flow (a hit was logged this frame)
    /// 3. Attack beats movement (attack input)
    /// 4. Move ↔ Idle natural flow
    /// 5. Hurt → Idle when stun ends
    /// 6. Attack → Idle when interval ends
    /// </summary>
    public class PlayerFSM : FsmBase
    {
        public PlayerIdleState idle;
        public PlayerMoveState move;
        public PlayerAttackState attack;
        public PlayerHurtState hurt;
        public PlayerDeadState dead;

        public PlayerFSM(PlayerController owner)
        {
            idle = new PlayerIdleState(owner);
            move = new PlayerMoveState(owner);
            attack = new PlayerAttackState(owner);
            hurt = new PlayerHurtState(owner);
            dead = new PlayerDeadState(owner);

            // Priority 1: Dead overrides everything.
            idle.transitions.Add(new Transition(() => owner.Stats.CurrentHP == 0, dead));
            move.transitions.Add(new Transition(() => owner.Stats.CurrentHP == 0, dead));
            attack.transitions.Add(new Transition(() => owner.Stats.CurrentHP == 0, dead));
            hurt.transitions.Add(new Transition(() => owner.Stats.CurrentHP == 0, dead));

            // Priority 2: Hurt overrides regular flow.
            idle.transitions.Add(new Transition(owner.ConsumeHurt, hurt));
            move.transitions.Add(new Transition(owner.ConsumeHurt, hurt));
            attack.transitions.Add(new Transition(owner.ConsumeHurt, hurt));

            // Priority 3: Attack overrides movement.
            idle.transitions.Add(new Transition(idle.WantsToAttack, attack));
            move.transitions.Add(new Transition(move.WantsToAttack, attack));

            // Priority 4: Move ↔ Idle.
            idle.transitions.Add(new Transition(idle.WantsToMove, move));
            move.transitions.Add(new Transition(move.IsStill, idle));

            // Priority 5/6: timed exits.
            hurt.transitions.Add(new Transition(hurt.StunEnded, idle));
            attack.transitions.Add(new Transition(attack.Finished, idle));

            currentState = idle;
        }

        public override void Enter()
        {
            base.Enter();
            currentState.Enter();
        }
    }
}
