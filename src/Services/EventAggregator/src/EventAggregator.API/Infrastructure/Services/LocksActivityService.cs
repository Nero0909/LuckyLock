using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventAggregator.Entities;
using EventAggregator.Repository;

namespace EventAggregator.API.Infrastructure.Services
{
    public class LocksActivityService : ILocksActivityService
    {
        private readonly ILocksActivityRepository _repository;

        public LocksActivityService(ILocksActivityRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<LockActivity>> GetLockActivities(Guid lockId, string userId)
        {
            var events = await _repository.GetEventsAsync(lockId, userId).ConfigureAwait(false);

            return events.Select(CreateActivity).OrderByDescending(x => x.CreatedDate);
        }

        private LockActivity CreateActivity(DeserializedLockEvent e)
        {
            return new LockActivity
            {
                CreatedDate = e.CreatedDate,
                LockId = e.AggregateId,
                Type = e.EventType,
                Data = e.Data
            };
        }
    }
}
