using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyWideIO.API.Models.DB_Models;
using MyWideIO.API.Models.Dto_Models;
using MyWideIO.API.Services.Interfaces;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Security.Claims;
using WideIO.API.Attributes;

namespace MyWideIO.API.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Route("subscriptions")]
    [Authorize]
    public class SubscriptionsApiController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;

        public SubscriptionsApiController(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }
        /// <summary>
        /// Subscribe to another user
        /// </summary>
        /// <param name="creatorId"></param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Not found</response>

        [HttpPost]
        [ValidateModelState]
        [SwaggerOperation("AddSubscription")]
        public virtual async Task<IActionResult> AddSubscription([FromQuery(Name = "creatorId")][Required()] Guid creatorId)
        {
            var viewerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            await _subscriptionService.SubscribeAsync(viewerId, creatorId);
            return Ok();
        }

        /// <summary>
        /// Unsubscribe from another user
        /// </summary>
        /// <param name="subId">Subscribed user ID</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Not found</response>

        [HttpDelete]
        [ValidateModelState]
        [SwaggerOperation("DeleteSubscription")]
        public virtual async Task<IActionResult> DeleteSubscription([FromQuery(Name = "subId")][Required()] Guid subId)
        {
            var viewerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            await _subscriptionService.UnsubscribeAsync(viewerId, subId);
            return Ok();
        }

        /// <summary>
        /// Get a user&#39;s subscriptions
        /// </summary>
        /// <param name="id">User ID</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Not found</response>

        [HttpGet]
        [ValidateModelState]
        [SwaggerOperation("GetSubscriptions")]
        [SwaggerResponse(statusCode: 200, type: typeof(UserSubscriptionListDto), description: "OK")]
        [AllowAnonymous]
        public virtual async Task<IActionResult> GetSubscriptions([FromQuery(Name = "id")][Required()] Guid id)
        {
            UserSubscriptionListDto subs = await _subscriptionService.GetSubscriptionsAsync(id);
            return Ok(subs);
        }
    }
}
