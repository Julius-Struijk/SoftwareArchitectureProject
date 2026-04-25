namespace CMGTSA.Enemies
{
    /// <summary>
    /// Global alert state. NORMAL = patrolling. ALERT = the player has been detected and
    /// every enemy chases for the duration set on AlertManager.
    /// </summary>
    public enum AlertLevel
    {
        NORMAL = 0,
        ALERT
    }
}
