using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventAggregator.Entities;

namespace EventAggregator.API.Infrastructure.Services
{
    public interface ILocksActivityService
    {
        Task<IEnumerable<LockActivity>> GetLockActivities(Guid lockId, string userId);
    }
}