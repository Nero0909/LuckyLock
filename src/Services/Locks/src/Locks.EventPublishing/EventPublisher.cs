using System;
using System.Threading.Tasks;
using Contracts.Events;
using Locks.Entities;
using MassTransit;

namespace Locks.EventPublishing
{
    public class EventPublisher : IEventPublisher
    {
        private readonly IBus _bus;

        public EventPublisher(IBus bus)
        {
            _bus = bus;
        }

        public Task SendLockChangedMessageAsync(Lock @lock, LockState oldState, LockState newState, string userId)
        {
            var message = new LockStateChangedMessage
            {
                LockId = @lock.Id,
                EventCreatedDate = DateTime.UtcNow,
                UserId = userId,
                EventId = Guid.NewGuid(),
                PreviousState = oldState.ToString(),
                CurrentState = newState.ToString()
            };

            return _bus.Publish<LockStateChanged>(message);
        }

        public Task SendLockCreatedMessageAsync(Lock @lock, string userId)
        {
            var message = new LockCreatedMessage
            {
                LockCreatedDate = @lock.CreatedDate,
                LockId = @lock.Id,
                Name = @lock.Name,
                UserId = userId,
                EventId = Guid.NewGuid(),
                EventCreatedDate = DateTime.UtcNow,
                UniqueNumber = @lock.UniqueNumber
            };

            return _bus.Publish<LockCreated>(message);
        }

        public Task SendLockDeletedMessageAsync(Lock @lock, string userId)
        {
            var message = new LockDeletedMessage
            {
                EventCreatedDate = DateTime.UtcNow,
                LockId = @lock.Id,
                UserId = userId,
                EventId = Guid.NewGuid()
            };

            return _bus.Publish<LockDeleted>(message);
        }

        public Task SendTagLinkedMessageAsync(LockTag link, string userId)
        {
            var message = new TagLinkedMessage
            {
                EventCreatedDate = DateTime.UtcNow,
                LockId = link.LockId,
                UserId = userId,
                EventId = Guid.NewGuid(),
                TagId = link.TagId
            };

            return _bus.Publish<TagLinked>(message);
        }

        public Task SendTagUnlinkedMessageAsync(LockTag link, string userId)
        {
            var message = new TagUnlinkedMessage
            {
                EventCreatedDate = DateTime.UtcNow,
                LockId = link.LockId,
                UserId = userId,
                EventId = Guid.NewGuid(),
                TagId = link.TagId
            };

            return _bus.Publish<TagUnlinked>(message);
        }
    }
}
