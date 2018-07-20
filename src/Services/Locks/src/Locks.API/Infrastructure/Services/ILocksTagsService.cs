using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Locks.Entities;

namespace Locks.API.Infrastructure.Services
{
    public interface ILocksTagsService
    {
        Task<bool> CheckTagExistence(Guid tagId, string userId);

        Task<bool> CheckLinkedLocksExistence(Guid tagId, string userId);

        Task<LockTag> CreateLink(LockTag link, string userId);

        Task<bool> DeleteLink(LockTag link, string userId);

        Task<IEnumerable<Guid>> GetLinkedTagsByLockAsync(Guid lockId);

        Task<IEnumerable<Guid>> GetLinkedLocksByTagAsync(Guid tagId, string userId);
    }
}
