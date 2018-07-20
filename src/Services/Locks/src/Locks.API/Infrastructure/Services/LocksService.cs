using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Locks.Entities;
using Locks.EventPublishing;
using Locks.Repository;

namespace Locks.API.Infrastructure.Services
{
    public class LocksService : ILocksService
    {
        private readonly ILocksRepository _locksRepository;
        private readonly IEventPublisher _eventPublisher;

        public LocksService(ILocksRepository locksRepository, IEventPublisher eventPublisher)
        {
            _locksRepository = locksRepository;
            _eventPublisher = eventPublisher;
        }

        public async Task<Lock> TryCreateAsync(Lock @lock, string userId)
        {
            var entity = await _locksRepository.TryCreateAsync(@lock, userId).ConfigureAwait(false);
            if (entity != null)
            {
                await _eventPublisher.SendLockCreatedMessageAsync(entity, userId).ConfigureAwait(false);
            }

            return entity;
        }

        public async Task<bool> DeleteAsync(Lock @lock, string userId)
        {
            var deleted = await _locksRepository.DeleteAsync(@lock.Id, userId);
            if (deleted)
            {
                await _eventPublisher.SendLockDeletedMessageAsync(@lock, userId).ConfigureAwait(false);
            }

            return deleted;
        }

        public Task<IEnumerable<Lock>> GetByUserAsync(string userId)
        {
            return _locksRepository.GetByUserAsync(userId);
        }

        public Task<Lock> GetByIdAsync(Guid id, string userId)
        {
            return _locksRepository.GetByIdAsync(id, userId);
        }

        public async Task<Lock> ChangeStateAsync(Guid id, string userId, LockState newState)
        {
            var entity = await _locksRepository.GetByIdAsync(id, userId).ConfigureAwait(false);
            if (entity == null)
            {
                return null;
            }

            if (entity.State.Equals(newState))
            {
                return entity;
            }

            var oldState = entity.State;
            entity.State = newState;

            var updatedLock = await _locksRepository.UpdateAsync(entity, userId).ConfigureAwait(false);
            if (updatedLock != null)
            {
                await _eventPublisher.SendLockChangedMessageAsync(updatedLock, oldState, newState, userId)
                    .ConfigureAwait(false);
            }

            return updatedLock;
        }

        public async Task<bool> CheckLockExistence(Guid tagId, string userId)
        {
            var tag = await _locksRepository.GetByIdAsync(tagId, userId).ConfigureAwait(false);

            return tag != null;
        }
    }
}
