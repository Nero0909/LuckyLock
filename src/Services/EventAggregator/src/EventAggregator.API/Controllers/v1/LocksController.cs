using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventAggregator.API.Controllers.v1.Models;
using EventAggregator.API.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventAggregator.API.Controllers.v1
{
    [Route("v1/locks")]
    [Authorize]
    public class LocksController : Controller
    {
        private readonly ILocksActivityService _activityService;
        private readonly IIdentityService _identityService;

        public LocksController(ILocksActivityService activityService, IIdentityService identityService)
        {
            _activityService = activityService;
            _identityService = identityService;
        }


        [Route("{lockid}/events")]
        [HttpGet]
        public async Task<IActionResult> GetActivitiesAsync([FromRoute] string lockid)
        {
            if (!Guid.TryParse(lockid, out var id))
            {
                return BadRequest(new ErrorResponse { Error = "Invalid lock id" });
            }

            var userId = _identityService.GetUserIdentity();
            var activities = (await _activityService.GetLockActivities(id, userId).ConfigureAwait(false)).ToArray();

            return Ok(activities);
        }
    }
}
