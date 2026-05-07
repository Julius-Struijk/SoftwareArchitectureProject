namespace CMGTSA.Player
{
    public struct AttackCooldown
    {
        private float readyAt;

        // Returns true and stamps next-ready time if cooldown has elapsed; false otherwise.
        public bool TryFire(float now, float interval)
        {
            if (now < readyAt) return false;
            readyAt = now + interval;
            return true;
        }
    }
}
