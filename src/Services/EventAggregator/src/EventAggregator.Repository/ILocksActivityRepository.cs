using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Contracts.Events;
using EventAggregator.Entities;

namespace EventAggregator.Repository
{
    public interface ILocksActivityRepository
    {
        Task<SerializedEvent> CreateAsync(BaseLockMessage @event);

        Task<IEnumerable<DeserializedLockEvent>> GetEventsAsync(Guid aggregateId, string userId);
    }
}
