using System;
using System.Collections.Generic;
using System.Linq;
using Contracts.Events;
using EventAggregator.Entities;
using Newtonsoft.Json;

namespace EventAggregator.Repository.Serializer
{
    public class EventDeserializer : IEventDeserializer
    {
        private readonly Lazy<List<Type>> _eventTypes;

        public EventDeserializer()
        {
            _eventTypes = new Lazy<List<Type>>(() =>
            {
                var result = new List<Type>();
                foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
                {
                    result.AddRange(a.GetTypes()
                        .Where(t => t.IsClass && !t.IsAbstract && typeof(BaseLockMessage).IsAssignableFrom(t)));
                }
                return result;
            });
        }

        public DeserializedLockEvent Deserialize(SerializedEvent e)
        {
            var type = _eventTypes.Value.FirstOrDefault(x => x.Name.Contains(e.EventType));
            if (type != null)
            {
                var message = (BaseLockMessage)JsonConvert.DeserializeObject(e.Data, type);
                var deserializedLockEvent = new DeserializedLockEvent
                {
                    CreatedDate = e.CreatedDate,
                    Id = e.Id,
                    AggregateId = e.AggregateId,
                    UserId = e.UserId,
                    EventType = e.EventType,
                    Data = message
                };

                return deserializedLockEvent;
            }

            return null;
        }
    }
}