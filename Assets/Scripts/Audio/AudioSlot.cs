namespace CMGTSA.Audio
{
    /// <summary>
    /// Slice-7 polish: enum keys for the sounds the game can play. Adding a new sound is
    /// one new enum value + one new row in AudioCatalog.asset — zero audio-code edits.
    /// </summary>
    public enum AudioSlot
    {
        AttackWhoosh,
        PlayerHit,
        EnemyHit,
        EnemyDeath,
        PlayerDeath,
        LevelUp,
        SkillCast,
        BossStinger,
        UIClick,
    }
}
