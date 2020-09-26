using _3.RabbitMq.Service.Bus.CommandBus;
using _3.RabbitMq.Service.Bus.Events.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _3.RabbitMq.Service.Bus.Abstractions
{
    public partial class EventBusSubscriptionsManager : IEventBusSubscriptionsManager
    {
        private readonly Dictionary<string, List<SubscriptionInfo>> _handlers;
        private readonly List<Type> _eventTypes;

        public event EventHandler<string> OnEventRemoved;

        public EventBusSubscriptionsManager()
        {
            _handlers = new Dictionary<string, List<SubscriptionInfo>>();
            _eventTypes = new List<Type>();
        }

        public bool IsEmpty => !_handlers.Keys.Any();

        public void Clear() => _handlers.Clear();

        public void AddSubscription<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = GetEventKey<T>();
            DoAddSubscription(typeof(TH), eventName, isDynamic: false);
            _eventTypes.Add(typeof(T));
        }

        public void AddSubscription(string eventName, Type typeEvent, Type typeHandler)
        {
            DoAddSubscription(typeHandler, eventName, isDynamic: false);
            _eventTypes.Add(typeEvent);
        }

        private void DoAddSubscription(Type handlerType, string eventName, bool isDynamic)
        {
            if (!HasSubscriptionsForEvent(eventName))
                _handlers.Add(eventName, new List<SubscriptionInfo>());

            if (_handlers[eventName].Any(s => s.HandlerType == handlerType))
                throw new ArgumentException($"Handler Type {handlerType.Name} already registered for '{eventName}'", nameof(handlerType));

            _handlers[eventName].Add(SubscriptionInfo.Subscription(isDynamic, handlerType));
        }

        public void RemoveSubscription<T, TH>()
            where TH : IIntegrationEventHandler<T>
            where T : IntegrationEvent
          => DoRemoveHandler(GetEventKey<T>(), FindSubscriptionToRemove<T, TH>());

        private void DoRemoveHandler(string eventName, SubscriptionInfo subsToRemove)
        {
            if (subsToRemove == null) return;

            _handlers[eventName].Remove(subsToRemove);
            if (!_handlers[eventName].Any())
            {
                _handlers.Remove(eventName);
                var eventType = _eventTypes.SingleOrDefault(e => e.Name == eventName);
                if (eventType != null)
                    _eventTypes.Remove(eventType);
                RaiseOnEventRemoved(eventName);
            }
        }

        public IEnumerable<SubscriptionInfo> GetHandlersForEvent<T>() where T : IntegrationEvent
        => GetHandlersForEvent(GetEventKey<T>());

        public IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName)
        {
            _handlers.TryGetValue(eventName, out List<SubscriptionInfo> result);
            return result;
        }

        private void RaiseOnEventRemoved(string eventName)
        {
            var handler = OnEventRemoved;
            if (handler != null)
                OnEventRemoved(this, eventName);
        }

        private SubscriptionInfo FindSubscriptionToRemove<T, TH>()
             where T : IntegrationEvent
             where TH : IIntegrationEventHandler<T>
        => DoFindSubscriptionToRemove(GetEventKey<T>(), typeof(TH));

        private SubscriptionInfo DoFindSubscriptionToRemove(string eventName, Type handlerType)
        {
            if (!HasSubscriptionsForEvent(eventName))
                return null;
            return _handlers[eventName].SingleOrDefault(s => s.HandlerType == handlerType);
        }

        public bool HasSubscriptionsForEvent<T>() where T : IntegrationEvent => HasSubscriptionsForEvent(GetEventKey<T>());

        public bool HasSubscriptionsForEvent(string eventName) => _handlers.ContainsKey(eventName);

        public Type GetEventTypeByName(string eventName) => _eventTypes.SingleOrDefault(t => t.Name == eventName);

        public string GetEventKey<T>() => typeof(T).Name;
    }

    public class SubscriptionInfo
    {
        public bool IsDynamic { get; private set; }
        public Type HandlerType { get; private set; }

        private SubscriptionInfo(bool isDynamic, Type handlerType)
        {
            IsDynamic = isDynamic;
            HandlerType = handlerType;
        }

        public static SubscriptionInfo Subscription(bool isDynamic, Type handlerType) => new SubscriptionInfo(isDynamic, handlerType);
    }

    public class SubscriptionModel
    {
        public string EventName { get; set; }
        public Type TypeEvent { get; set; }
        public Type TypeHandler { get; set; }
    }
}