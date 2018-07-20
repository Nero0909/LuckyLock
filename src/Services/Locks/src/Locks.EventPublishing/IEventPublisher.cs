using System;
using System.Threading.Tasks;
using Locks.Entities;

namespace Locks.EventPublishing
{
    public interface IEventPublisher
    {
        Task SendLockChangedMessageAsync(Lock @lock, LockState oldState, LockState newState, string userId);

        Task SendLockCreatedMessageAsync(Lock @lock, string userId);

        Task SendLockDeletedMessageAsync(Lock @lock, string userId);

        Task SendTagLinkedMessageAsync(LockTag link, string userId);

        Task SendTagUnlinkedMessageAsync(LockTag link, string userId);
    }
}