using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tags.Entities;

namespace Tags.Repositories
{
    public interface ITagsRepository
    {
        Task<Tag> TryCreateAsync(Tag tag, string userId);

        Task<bool> DeleteAsync(Guid id, string userId);

        Task<Tag> GetByIdAsync(Guid id, string userId);

        Task<IEnumerable<Tag>> GetByUserAsync(string userId);
    }
}
