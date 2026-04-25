using CMGTSA.Core;

namespace CMGTSA.Enemies
{
    /// <summary>
    /// Published by <see cref="AlertManager"/>'s collider triggers when the player enters or
    /// stays in any enemy's detection volume. AlertManager itself subscribes and uses this to
    /// reset the alert timer.
    /// </summary>
    public readonly struct EnemyDetectedPlayerEvent : IGameEvent { }
}
