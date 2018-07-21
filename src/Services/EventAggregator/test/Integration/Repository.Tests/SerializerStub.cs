using Contracts.Events;
using EventAggregator.Entities;
using EventAggregator.Repository.Serializer;
using Newtonsoft.Json;

namespace Repository.Tests
{
    public class SerializerStub : IEventSerializer
    {
        public DeserializedLockEvent Deserialize(SerializedEvent e)
        {
            return new DeserializedLockEvent
            {
                CreatedDate = e.CreatedDate,
                EventType = e.EventType,
                Id = e.Id,
                AggregateId = e.AggregateId,
                UserId = e.UserId
            };
        }

        public SerializedEvent Serialize(BaseLockMessage message)
        {
            return new SerializedEvent
            {
                Id = message.EventId,
                AggregateId = message.LockId,
                CreatedDate = message.EventCreatedDate,
                EventType = message.GetType().Name,
                UserId = message.UserId,
                Data = JsonConvert.SerializeObject(message)
            };
        }
    }
}
