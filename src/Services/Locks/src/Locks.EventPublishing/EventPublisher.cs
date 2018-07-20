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
                CreatedDate = @lock.CreatedDate,
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
                CreatedDate = @lock.CreatedDate,
                LockId = @lock.Id,
                Name = @lock.Name,
                UserId = userId,
                EventId = Guid.NewGuid(),
                UniqueNumber = @lock.UniqueNumber
            };

            return _bus.Publish<LockCreated>(message);
        }

        public Task SendLockDeletedMessageAsync(Lock @lock, string userId)
        {
            var message = new LockDeletedMessage
            {
                CreatedDate = @lock.CreatedDate,
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
                CreatedDate = link.CreatedDate,
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
                CreatedDate = link.CreatedDate,
                LockId = link.LockId,
                UserId = userId,
                EventId = Guid.NewGuid(),
                TagId = link.TagId
            };

            return _bus.Publish<TagUnlinked>(message);
        }
    }
}
