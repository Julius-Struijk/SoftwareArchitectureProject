using NUnit.Framework;
using CMGTSA.Player;

namespace CMGTSA.Tests
{
    public class AttackCooldownTests
    {
        [Test]
        public void First_call_with_input_fires_when_zero_readyAt()
        {
            var cooldown = new AttackCooldown();

            bool fired = cooldown.TryFire(now: 1.0f, interval: 0.4f);

            Assert.IsTrue(fired, "First call should fire when readyAt is 0.");
        }

        [Test]
        public void Second_call_within_interval_does_not_fire()
        {
            var cooldown = new AttackCooldown();
            cooldown.TryFire(now: 1.0f, interval: 0.4f);

            bool fired = cooldown.TryFire(now: 1.2f, interval: 0.4f);

            Assert.IsFalse(fired, "Second call before interval elapses should not fire.");
        }

        [Test]
        public void Second_call_after_interval_fires()
        {
            var cooldown = new AttackCooldown();
            cooldown.TryFire(now: 1.0f, interval: 0.4f);

            bool fired = cooldown.TryFire(now: 1.5f, interval: 0.4f);

            Assert.IsTrue(fired, "Call after interval has elapsed should fire.");
        }

        [Test]
        public void Equal_now_and_readyAt_fires()
        {
            var cooldown = new AttackCooldown();
            cooldown.TryFire(now: 1.0f, interval: 0.4f);

            // now == readyAt is the boundary: "Time.time >= attackCooldownUntil" so equality must fire.
            bool fired = cooldown.TryFire(now: 1.4f, interval: 0.4f);

            Assert.IsTrue(fired, "now == readyAt is the boundary; must fire.");
        }
    }
}
