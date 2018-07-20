using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Locks.Entities;

namespace Locks.Repository
{
    public interface ILocksRepository
    {
        Task<Lock> TryCreateAsync(Lock @lock, string userId);

        Task<bool> DeleteAsync(Guid lockId, string userId);

        Task<Lock> GetByIdAsync(Guid id, string userId);

        Task<IEnumerable<Lock>> GetByUserAsync(string userId);

        Task<Lock> UpdateAsync(Lock @lock, string userId);
    }
}
