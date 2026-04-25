using CMGTSA.Core;

namespace CMGTSA.Enemies
{
    /// <summary>
    /// Published by <see cref="AlertManager"/> whenever the global AlertLevel changes.
    /// Subscribers: every <see cref="EnemyController"/> (writes the level into its Enemy).
    /// </summary>
    public readonly struct AlertLevelChangedEvent : IGameEvent
    {
        public readonly AlertLevel Level;
        public AlertLevelChangedEvent(AlertLevel level) { Level = level; }
    }
}
