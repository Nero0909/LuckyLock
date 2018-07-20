using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tags.API.Controllers.V1.Models;
using Tags.API.Infrastructure.Filters;
using Tags.API.Infrastructure.Services;
using Tags.Entities;

namespace Tags.API.Controllers.V1
{
    [Route("v1/tags")]
    [Authorize]
    public class TagsController : Controller
    {
        private readonly ITagsService _tagsService;
        private readonly IIdentityService _identityService;

        public TagsController(ITagsService tagsService, IIdentityService identityService)
        {
            _tagsService = tagsService;
            _identityService = identityService;
        }

        [HttpPost]
        [ValidateModelFilter]
        public async Task<IActionResult> CreateAsync([FromBody] TagModel model)
        {
            var toCreate = new Tag
            {
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.UtcNow,
                Name = model.Name,
                UniqueNumber = model.UniqueNumer,
            };
            var userId = _identityService.GetUserIdentity();
            var tag = await _tagsService.TryCreateAsync(toCreate, userId).ConfigureAwait(false);
            if (tag == null)
            {
                return Conflict(new ErrorResponse { Error = $"Tag with number {model.UniqueNumer} already exists" });
            }

            return Created(Url.RouteUrl(tag.Id), tag.Id);
        }

        [Route("{tagid}")]
        [HttpGet]
        public async Task<IActionResult> GetByIdAsync([FromRoute] string tagId)
        {
            if (!Guid.TryParse(tagId, out var id))
            {
                return BadRequest(new ErrorResponse {Error = "Invalid tag id"});
            }

            var userId = _identityService.GetUserIdentity();
            var tag = await _tagsService.GetByIdAsync(id, userId).ConfigureAwait(false);
            if (tag == null)
            {
                return NotFound();
            }

            return Ok(tag);
        }

        [Route("{tagid}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteAsync([FromRoute] string tagId)
        {
            if (!Guid.TryParse(tagId, out var id))
            {
                return BadRequest(new ErrorResponse { Error = "Invalid tag id" });
            }

            var userId = _identityService.GetUserIdentity();
            var tag = await _tagsService.GetByIdAsync(id, userId).ConfigureAwait(false);
            if (tag == null)
            {
                return NotFound();
            }

            var exist = await _tagsService.CheckLinkedLocksExistence(id, userId).ConfigureAwait(false);
            if (exist)
            {
                return BadRequest(new ErrorResponse{Error = "Could not delete tag wih linked locks"});
            }

            var deleted =  await _tagsService.DeleteAsync(id, userId).ConfigureAwait(false);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> GetByUserAsync()
        {
            var userId = _identityService.GetUserIdentity();
            var tag = await _tagsService.GetByUserAsync(userId).ConfigureAwait(false);

            return Ok(tag);
        }
    }
}
