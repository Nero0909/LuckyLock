using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Locks.Entities;

namespace Locks.API.Infrastructure.Services
{
    public interface ILocksService
    {
        Task<Lock> TryCreateAsync(Lock @lock, string userId);

        Task<bool> DeleteAsync(Lock @lock, string userId);

        Task<IEnumerable<Lock>> GetByUserAsync(string userId);

        Task<Lock> GetByIdAsync(Guid id, string userId);

        Task<Lock> ChangeStateAsync(Guid id, string userId, LockState newState);

        Task<bool> CheckLockExistence(Guid tagId, string userId);
    }
}