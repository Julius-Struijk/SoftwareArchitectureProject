using NUnit.Framework;
using CMGTSA.Core;

namespace CMGTSA.Tests
{
    public class EventBusTests
    {
        private struct TestEvent : IGameEvent { public int Value; }

        [TearDown]
        public void TearDown()
        {
            EventBus<TestEvent>.Clear();
        }

        [Test]
        public void Subscribe_then_Publish_invokes_handler()
        {
            int received = 0;
            void Handler(TestEvent e) => received = e.Value;

            EventBus<TestEvent>.Subscribe(Handler);
            EventBus<TestEvent>.Publish(new TestEvent { Value = 42 });

            Assert.AreEqual(42, received);
        }

        [Test]
        public void Unsubscribe_stops_invocation()
        {
            int calls = 0;
            void Handler(TestEvent e) => calls++;

            EventBus<TestEvent>.Subscribe(Handler);
            EventBus<TestEvent>.Unsubscribe(Handler);
            EventBus<TestEvent>.Publish(new TestEvent { Value = 1 });

            Assert.AreEqual(0, calls);
        }

        [Test]
        public void Multiple_subscribers_all_receive()
        {
            int a = 0, b = 0;
            EventBus<TestEvent>.Subscribe(e => a = e.Value);
            EventBus<TestEvent>.Subscribe(e => b = e.Value);

            EventBus<TestEvent>.Publish(new TestEvent { Value = 7 });

            Assert.AreEqual(7, a);
            Assert.AreEqual(7, b);
        }

        [Test]
        public void Unsubscribe_during_publish_does_not_throw()
        {
            int otherCalls = 0;
            System.Action<TestEvent> selfRemoving = null;
            selfRemoving = (e) => EventBus<TestEvent>.Unsubscribe(selfRemoving);
            void Other(TestEvent e) => otherCalls++;

            EventBus<TestEvent>.Subscribe(selfRemoving);
            EventBus<TestEvent>.Subscribe(Other);

            Assert.DoesNotThrow(() => EventBus<TestEvent>.Publish(new TestEvent { Value = 1 }));
            Assert.AreEqual(1, otherCalls);
        }

        [Test]
        public void Unsubscribe_unknown_handler_is_noop()
        {
            void Handler(TestEvent e) { }
            Assert.DoesNotThrow(() => EventBus<TestEvent>.Unsubscribe(Handler));
        }

        [Test]
        public void Subscribe_null_is_ignored()
        {
            Assert.DoesNotThrow(() => EventBus<TestEvent>.Subscribe(null));
            Assert.DoesNotThrow(() => EventBus<TestEvent>.Publish(new TestEvent { Value = 0 }));
        }
    }
}
