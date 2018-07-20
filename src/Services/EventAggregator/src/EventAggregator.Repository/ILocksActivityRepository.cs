using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventAggregator.Entities;

namespace EventAggregator.Repository
{
    public interface ILocksActivityRepository
    {
        Task<SerializedEvent> CreateAsync(SerializedEvent @event);

        Task<IEnumerable<DeserializedLockEvent>> GetEventsAsync(Guid aggregateId, string userId);
    }
}
