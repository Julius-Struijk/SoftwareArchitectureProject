namespace CMGTSA.Core
{
    /// <summary>
    /// Marker interface implemented by every event published through <see cref="EventBus{T}"/>.
    /// Constrains the bus to known event types and gives reviewers a single place to find every event.
    /// </summary>
    public interface IGameEvent { }
}
