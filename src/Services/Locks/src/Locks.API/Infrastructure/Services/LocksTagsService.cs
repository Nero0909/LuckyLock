using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Client;
using Locks.Entities;
using Locks.EventPublishing;
using Locks.Repository;

namespace Locks.API.Infrastructure.Services
{
    public class LocksTagsService : ILocksTagsService
    {
        private readonly ILocksTagsRepository _locksTagsRepository;
        private readonly IGateway _gateway;
        private readonly ITokensClient _tokensClient;
        private readonly IEventPublisher _eventPublisher;

        public LocksTagsService(
            ILocksTagsRepository locksTagsRepository, 
            IGateway gateway, 
            ITokensClient tokensClient,
            IEventPublisher eventPublisher)
        {
            _locksTagsRepository = locksTagsRepository;
            _gateway = gateway;
            _tokensClient = tokensClient;
            _eventPublisher = eventPublisher;
        }

        public async Task<bool> CheckTagExistence(Guid tagId, string userId)
        {
            var token = await _tokensClient.GetTokenForTags().ConfigureAwait(false);

            if (token.IsError)
            {
                throw new InvalidOperationException(token.HttpErrorReason);
            }

            var tag = await _gateway.Tags.GetByIdAsync(tagId, token.AccessToken).ConfigureAwait(false);

            return tag != null;
        }

        public async Task<LockTag> CreateLink(LockTag link, string userId)
        {
            var entity = await _locksTagsRepository.TryCreateAsync(link, userId).ConfigureAwait(false);
            if (entity != null)
            {
                await _eventPublisher.SendTagLinkedMessageAsync(entity, userId).ConfigureAwait(false);
            }

            return entity;
        }

        public async Task<bool> DeleteLink(LockTag link, string userId)
        {
            var deleted = await _locksTagsRepository.DeleteLinkAsync(link, userId).ConfigureAwait(false);
            if (deleted)
            {
                await _eventPublisher.SendTagUnlinkedMessageAsync(link, userId).ConfigureAwait(false);
            }

            return deleted;
        }

        public Task<IEnumerable<Guid>> GetLinkedTagsByLockAsync(Guid lockId)
        {
            return _locksTagsRepository.GetLinkedTagsByLockAsync(lockId);
        }

        public Task<IEnumerable<Guid>> GetLinkedLocksByTagAsync(Guid tagId, string userId)
        {
            return _locksTagsRepository.GetLinkedLocksByTagIdAsync(tagId, userId);
        }

        public Task<bool> CheckLinkedLocksExistence(Guid tagId, string userId)
        {
            return _locksTagsRepository.CheckLinkedLocksExistence(tagId, userId);
        }
    }
}
