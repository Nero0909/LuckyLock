using System;
using System.Threading.Tasks;

namespace Client.Locks
{
    public interface ILocks : IDisposable
    {
        Task<bool> CheckLinkedLocksExistence(Guid tagId, string accessToken);
    }
}
