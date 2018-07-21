using Contracts.Events;
using EventAggregator.Entities;

namespace EventAggregator.Repository.Serializer
{
    public interface IEventSerializer
    {
        DeserializedLockEvent Deserialize(SerializedEvent e);

        SerializedEvent Serialize(BaseLockMessage message);
    }
}