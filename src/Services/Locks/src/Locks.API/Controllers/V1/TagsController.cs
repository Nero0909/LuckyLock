using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Locks.API.Controllers.V1.Models;
using Locks.API.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Locks.API.Controllers.V1
{
    [Route("v1")]
    [Authorize]
    public class TagsController : Controller
    {
        private readonly ILocksTagsService _locksTagsService;
        private readonly IIdentityService _identityService;

        public TagsController(ILocksTagsService locksTagsService, IIdentityService identityService)
        {
            _locksTagsService = locksTagsService;
            _identityService = identityService;
        }

        [Route("tags/{tagid}/lockslinks")]
        [HttpGet]
        public async Task<IActionResult> GetLinkedLocksAsync(string tagid)
        {
            if (!Guid.TryParse(tagid, out var id))
            {
                return BadRequest(new ErrorResponse { Error = "Invalid lock id" });
            }

            var userId = _identityService.GetUserIdentity();
            var result = await _locksTagsService.GetLinkedLocksByTagAsync(id, userId).ConfigureAwait(false);

            return Ok(result);
        }

        [Route("tags/{tagid}/lockslinks")]
        [HttpHead]
        public async Task<IActionResult> CheckLinkedLockExistence(string tagid)
        {
            if (!Guid.TryParse(tagid, out var id))
            {
                return BadRequest(new ErrorResponse { Error = "Invalid lock id" });
            }

            var userId = _identityService.GetUserIdentity();
            var exists = await _locksTagsService.CheckLinkedLocksExistence(id, userId).ConfigureAwait(false);

            if (exists)
            {
                return NoContent();
            }

            return NotFound();
        }
    }
}
