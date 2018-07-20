using EventAggregator.Entities;

namespace EventAggregator.Repository.Serializer
{
    public interface IEventDeserializer
    {
        DeserializedLockEvent Deserialize(SerializedEvent e);
    }
}