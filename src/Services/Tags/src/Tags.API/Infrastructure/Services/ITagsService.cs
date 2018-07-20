using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tags.Entities;

namespace Tags.API.Infrastructure.Services
{
    public interface ITagsService
    {
        Task<Tag> TryCreateAsync(Tag tag, string userId);

        Task<IEnumerable<Tag>> GetByUserAsync(string userId);

        Task<Tag> GetByIdAsync(Guid id, string userId);

        Task<bool> CheckLinkedLocksExistence(Guid tagId, string userId);

        Task<bool> DeleteAsync(Guid id, string userId);
    }
}
