using NUnit.Framework;
using CMGTSA.FSM;

namespace CMGTSA.Tests
{
    public class FSMTests
    {
        private class TrackingState : State
        {
            public int EnterCount;
            public int ExitCount;
            public int StepCount;

            public override void Enter() { base.Enter(); EnterCount++; }
            public override void Exit()  { base.Exit();  ExitCount++; }
            public override void Step()  { base.Step();  StepCount++; }
        }

        private class TrackingFSM : CMGTSA.FSM.FSM
        {
            public TrackingFSM(State initial) { currentState = initial; }
        }

        [Test]
        public void State_Enter_invokes_onEnter_callback()
        {
            var state = new TrackingState();
            int calls = 0;
            state.onEnter += () => calls++;

            state.Enter();

            Assert.AreEqual(1, calls);
        }

        [Test]
        public void State_Exit_invokes_onExit_callback()
        {
            var state = new TrackingState();
            int calls = 0;
            state.onExit += () => calls++;

            state.Exit();

            Assert.AreEqual(1, calls);
        }

        [Test]
        public void State_NextState_returns_null_when_no_transitions()
        {
            var state = new TrackingState();

            Assert.IsNull(state.NextState());
        }

        [Test]
        public void State_NextState_returns_null_when_condition_false()
        {
            var stateA = new TrackingState();
            var stateB = new TrackingState();
            stateA.transitions.Add(new Transition(() => false, stateB));

            Assert.IsNull(stateA.NextState());
        }

        [Test]
        public void State_NextState_returns_target_when_condition_true()
        {
            var stateA = new TrackingState();
            var stateB = new TrackingState();
            stateA.transitions.Add(new Transition(() => true, stateB));

            Assert.AreSame(stateB, stateA.NextState());
        }

        [Test]
        public void FSM_Step_transitions_when_condition_becomes_true()
        {
            var stateA = new TrackingState();
            var stateB = new TrackingState();
            bool trigger = false;
            stateA.transitions.Add(new Transition(() => trigger, stateB));

            var fsm = new TrackingFSM(stateA);

            fsm.Step();
            Assert.AreEqual(0, stateB.EnterCount, "should not transition yet");

            trigger = true;
            fsm.Step();

            Assert.AreEqual(1, stateA.ExitCount,  "stateA should have exited");
            Assert.AreEqual(1, stateB.EnterCount, "stateB should have entered");
        }

        [Test]
        public void FSM_Step_stays_on_current_state_when_no_condition_fires()
        {
            var stateA = new TrackingState();
            var stateB = new TrackingState();
            stateA.transitions.Add(new Transition(() => false, stateB));

            var fsm = new TrackingFSM(stateA);
            fsm.Step();
            fsm.Step();

            Assert.AreEqual(2, stateA.StepCount,  "stateA should have stepped twice");
            Assert.AreEqual(0, stateB.EnterCount, "stateB should never have entered");
        }
    }
}
