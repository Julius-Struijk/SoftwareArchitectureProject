namespace CMGTSA.Core
{
    /// <summary>
    /// Anything that can take damage. Implemented by <c>PlayerController</c> and
    /// <c>EnemyController</c>. <see cref="TakeDamage"/> returns true if the hit was lethal,
    /// so <c>DamageResolver</c> can flag the resulting <c>DamageDealtEvent</c> as fatal
    /// without having to read the target's HP afterwards.
    /// </summary>
    public interface IDamageable
    {
        /// <summary>
        /// Apply <paramref name="amount"/> HP loss. Returns true iff the hit reduced HP to 0.
        /// Damage-type metadata (slowdown, VFX) is communicated separately via
        /// <c>DamageDealtEvent</c> — <c>IDamageable</c> stays a one-method, zero-domain contract.
        /// </summary>
        bool TakeDamage(int amount);
    }
}
