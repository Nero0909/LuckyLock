using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Client;
using Locks.API.Infrastructure.Services;
using Tags.Entities;
using Tags.Repositories;

namespace Tags.API.Infrastructure.Services
{
    public class TagsService : ITagsService
    {
        private readonly ITagsRepository _tagsRepository;
        private readonly IGateway _gateway;
        private readonly ITokensClient _tokensClient;

        public TagsService(ITagsRepository tagsRepository, IGateway gateway, ITokensClient tokensClient)
        {
            _tagsRepository = tagsRepository;
            _gateway = gateway;
            _tokensClient = tokensClient;
        }

        public Task<Tag> TryCreateAsync(Tag tag, string userId)
        {
            return _tagsRepository.TryCreateAsync(tag, userId);
        }

        public Task<IEnumerable<Tag>> GetByUserAsync(string userId)
        {
            return _tagsRepository.GetByUserAsync(userId);
        }

        public Task<Tag> GetByIdAsync(Guid id, string userId)
        {
            return _tagsRepository.GetByIdAsync(id, userId);
        }

        public async Task<bool> CheckLinkedLocksExistence(Guid tagId, string userId)
        {
            var response = await _tokensClient.GetTokenForLocks().ConfigureAwait(false);
            return await _gateway.Locks.CheckLinkedLocksExistence(tagId, response.AccessToken).ConfigureAwait(false);
        }

        public Task<bool> DeleteAsync(Guid id, string userId)
        {
            return _tagsRepository.DeleteAsync(id, userId);
        }
    }
}
