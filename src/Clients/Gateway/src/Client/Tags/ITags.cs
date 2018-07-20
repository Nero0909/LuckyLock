using System;
using System.Threading.Tasks;
using Client.Tags.Models;

namespace Client.Tags
{
    public interface ITags : IDisposable
    {
        Task<Tag> GetByIdAsync(Guid id, string accessToken);
    }
}