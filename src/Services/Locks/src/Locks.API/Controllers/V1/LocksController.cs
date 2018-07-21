using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Locks.API.Controllers.V1.Models;
using Locks.API.Infrastructure.Filters;
using Locks.API.Infrastructure.Services;
using Locks.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Locks.API.Controllers.V1
{
    [Route("v1/locks")]
    [Authorize]
    public class LocksController : Controller
    {
        private readonly ILocksService _locksService;
        private readonly ILocksTagsService _locksTagsService;
        private readonly IIdentityService _identityService;

        public LocksController(ILocksService locksTagsService, IIdentityService identityService, ILocksTagsService locksTagsService1)
        {
            _locksService = locksTagsService;
            _identityService = identityService;
            _locksTagsService = locksTagsService1;
        }

        [HttpPost]
        [ValidateModelFilter]
        public async Task<IActionResult> CreateAsync([FromBody] LockModel model)
        {
            var toCreate = new Lock
            {
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.UtcNow,
                Name = model.Name,
                UniqueNumber = model.UniqueNumer
            };
            var userId = _identityService.GetUserIdentity();
            var @lock = await _locksService.TryCreateAsync(toCreate, userId).ConfigureAwait(false);
            if (@lock == null)
            {
                return Conflict(new ErrorResponse {Error = $"Lock with number {model.UniqueNumer} already exists"});
            }

            return Created(Url.RouteUrl(@lock.Id), @lock.Id);
        }

        [Route("{lockid}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteAsync([FromRoute] string lockid)
        {
            if (!Guid.TryParse(lockid, out var id))
            {
                return BadRequest(new ErrorResponse { Error = "Invalid lock id" });
            }

            var userId = _identityService.GetUserIdentity();
            var @lock = await _locksService.GetByIdAsync(id, userId);
            if (@lock == null)
            {
                return NotFound();
            }

            var links = await _locksTagsService.GetLinkedTagsByLockAsync(id);
            if (links.Any())
            {
                return BadRequest(new ErrorResponse {Error = "Could not delete lock with linked tags"});
            }

            var deleted = await _locksService.DeleteAsync(@lock, userId).ConfigureAwait(false);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }

        [Route("{lockid}/tags")]
        [HttpPost]
        [ValidateModelFilter]
        public async Task<IActionResult> CreateTagLinkAsync([FromBody] TagLinkModel model, [FromRoute] string lockid)
        {
            if (!Guid.TryParse(lockid, out var id))
            {
                return BadRequest(new ErrorResponse { Error = "Invalid lock id" });
            }

            var userId = _identityService.GetUserIdentity();
            var lockExists = await _locksService.CheckLockExistence(id, userId).ConfigureAwait(false);
            if (!lockExists)
            {
                return UnprocessableEntity(new ErrorResponse {Error = "Lock does not exist"});
            }

            var tagExists = await _locksTagsService.CheckTagExistence(model.TagId, userId).ConfigureAwait(false);
            if (!tagExists)
            {
                return UnprocessableEntity(new ErrorResponse { Error = "Tag does not exist" });
            }

            var toCreate = new LockTag
            {
                CreatedDate = DateTime.UtcNow,
                Id = Guid.NewGuid(),
                LockId = id,
                TagId = model.TagId
            };

            var link = await _locksTagsService.CreateLink(toCreate, userId).ConfigureAwait(false);
            if (link == null)
            {
                return Conflict(new ErrorResponse {Error = "Link already exists"});
            }

            return StatusCode((int) HttpStatusCode.Created);
        }

        [Route("{lockid}/tags/{tagid}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteTagLinkAsync([FromRoute] string lockid, [FromRoute] string tagid)
        {
            if (!Guid.TryParse(lockid, out var lockIdParsed))
            {
                return BadRequest(new ErrorResponse { Error = "Invalid lock id" });
            }

            if (!Guid.TryParse(tagid, out var tagIdParsed))
            {
                return BadRequest(new ErrorResponse { Error = "Invalid tag id" });
            }

            var userId = _identityService.GetUserIdentity();
            var lockExists = await _locksService.CheckLockExistence(lockIdParsed, userId).ConfigureAwait(false);
            if (!lockExists)
            {
                return UnprocessableEntity(new ErrorResponse { Error = "Lock does not exist" });
            }

            var tagExists = await _locksTagsService.CheckTagExistence(tagIdParsed, userId).ConfigureAwait(false);
            if (!tagExists)
            {
                return UnprocessableEntity(new ErrorResponse { Error = "Tag does not exist" });
            }

            var toDelete = new LockTag
            {
                LockId = lockIdParsed,
                TagId = tagIdParsed,
            };

            var deleted = await _locksTagsService.DeleteLink(toDelete, userId).ConfigureAwait(false);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }

        [Route("{lockid}/tags")]
        [HttpGet]
        public async Task<IActionResult> GetTagLinksAsync([FromRoute] string lockid)
        {
            if (!Guid.TryParse(lockid, out var id))
            {
                return BadRequest(new ErrorResponse { Error = "Invalid lock id" });
            }

            var userId = _identityService.GetUserIdentity();
            var lockExists = await _locksService.CheckLockExistence(id, userId).ConfigureAwait(false);
            if (!lockExists)
            {
                return UnprocessableEntity(new ErrorResponse { Error = "Lock does not exist" });
            }

            var result = await _locksTagsService.GetLinkedTagsByLockAsync(id).ConfigureAwait(false);

            return Ok(result);
        }

        [Route("{lockid}")]
        [HttpPut]
        [ValidateModelFilter]
        public async Task<IActionResult> UpdateStateAsync([FromBody] LockUpdateModel model, [FromRoute] string lockid)
        {
            if (!Guid.TryParse(lockid, out var id))
            {
                return BadRequest(new ErrorResponse { Error = "Invalid lock id" });
            }

            if (model.State == null || model.State.Value.Equals(LockState.Created))
            {
                return BadRequest(new ErrorResponse {Error = "Invalid state"});
            }

            var userId = _identityService.GetUserIdentity();
            var @lock = await _locksService.ChangeStateAsync(id, userId, model.State.Value).ConfigureAwait(false);

            if (@lock == null)
            {
                return NotFound();
            }

            return NoContent();
        }

        [Route("{lockid}")]
        [HttpGet]
        public async Task<IActionResult> GetByIdAsync([FromRoute] string lockid)
        {
            if (!Guid.TryParse(lockid, out var id))
            {
                return BadRequest(new ErrorResponse {Error = "Invalid lock id"});
            }

            var userId = _identityService.GetUserIdentity();
            var @lock = await _locksService.GetByIdAsync(id, userId).ConfigureAwait(false);
            if (@lock == null)
            {
                return NotFound();
            }

            return Ok(@lock);
        }

        [HttpGet]
        public async Task<IActionResult> GetByUserAsync()
        {
            var userId = _identityService.GetUserIdentity();
            var locks = await _locksService.GetByUserAsync(userId).ConfigureAwait(false);

            return Ok(locks);
        }
    }
}
