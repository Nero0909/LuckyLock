using EventAggregator.Entities;
using EventAggregator.Repository.Serializer;

namespace Repository.Tests
{
    public class DeserializerStub : IEventDeserializer
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
    }
}
