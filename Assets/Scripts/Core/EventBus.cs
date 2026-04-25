using System;
using System.Collections.Generic;

namespace CMGTSA.Core
{
    /// <summary>
    /// Code-only generic pub/sub bus. Each closed generic type (e.g. EventBus&lt;PlayerHPChangedEvent&gt;)
    /// has its own handler list. Subscribe in OnEnable, Unsubscribe in OnDisable.
    /// </summary>
    public static class EventBus<T> where T : IGameEvent
    {
        private static readonly List<Action<T>> handlers = new List<Action<T>>();

        public static void Subscribe(Action<T> handler)
        {
            if (handler == null) return;
            handlers.Add(handler);
        }

        public static void Unsubscribe(Action<T> handler)
        {
            if (handler == null) return;
            handlers.Remove(handler);
        }

        public static void Publish(T evt)
        {
            // Snapshot the list so a handler that unsubscribes itself (or another handler)
            // mid-publish does not invalidate enumeration.
            var snapshot = handlers.ToArray();
            for (int i = 0; i < snapshot.Length; i++)
            {
                snapshot[i]?.Invoke(evt);
            }
        }

        /// <summary>
        /// Test-only utility: clears every subscriber. Do not call from gameplay code.
        /// </summary>
        public static void Clear()
        {
            handlers.Clear();
        }
    }
}
