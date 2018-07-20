using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Locks.Entities;

namespace Locks.Repository
{
    public interface ILocksTagsRepository
    {
        Task<LockTag> TryCreateAsync(LockTag link, string userId);

        Task<bool> DeleteLinkAsync(LockTag link, string userId);

        Task<IEnumerable<Guid>> GetLinkedTagsByLockAsync(Guid lockId);

        Task<IEnumerable<Guid>> GetLinkedLocksByTagIdAsync(Guid tagId, string userId);

        Task<bool> CheckLinkedLocksExistence(Guid tagId, string userId);
    }
}