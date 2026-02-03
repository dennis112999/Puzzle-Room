using System;
using System.Collections.Generic;

namespace Services.EventBus
{
    public class EventBusService : IEventBus
    {
        private readonly Dictionary<Type, List<Delegate>> _eventListeners = new();

        public void Subscribe<T>(Action<T> listener)
        {
            Type eventType = typeof(T);

            if (!_eventListeners.ContainsKey(eventType))
            {
                _eventListeners[eventType] = new List<Delegate>();
            }

            _eventListeners[eventType].Add(listener);
        }

        public void Unsubscribe<T>(Action<T> listener)
        {
            Type eventType = typeof(T);

            if (_eventListeners.TryGetValue(eventType, out var listeners))
            {
                listeners.Remove(listener);
                if (listeners.Count == 0)
                {
                    _eventListeners.Remove(eventType);
                }
            }
        }

        public void Publish<T>(T eventData)
        {
            Type eventType = typeof(T);

            if (_eventListeners.TryGetValue(eventType, out var listeners))
            {
                var listenersCopy = new List<Delegate>(listeners);
                foreach (var listener in listenersCopy)
                {
                    (listener as Action<T>)?.Invoke(eventData);
                }
            }
        }

        public void Clear()
        {
            _eventListeners.Clear();
        }
    }
}
