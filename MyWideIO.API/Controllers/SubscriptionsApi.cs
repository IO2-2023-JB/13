using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    [Route("subscribtions")]
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
        /// <param name="subId">Subscribed user ID</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        [HttpPost]
        [Authorize]
        [ValidateModelState]
        [SwaggerOperation("AddSubscription")]
        public virtual async Task<IActionResult> AddSubscription([FromQuery(Name = "subId")][Required()] Guid subId)
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid viewerId))
                viewerId = Guid.Empty;
            try
            {
                await _subscriptionService.Subscribe(viewerId, subId);
            }
            catch (DataException exception)
            {
                return NotFound(exception.Message);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
            return Ok();

        }

        /// <summary>
        /// Unsubscribe from another user
        /// </summary>
        /// <param name="subId">Subscribed user ID</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        [HttpDelete]
        [Authorize]
        [ValidateModelState]
        [SwaggerOperation("DeleteSubscription")]
        public virtual async Task<IActionResult> DeleteSubscription([FromQuery(Name = "subId")][Required()] Guid subId)
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid viewerId))
                viewerId = Guid.Empty;
            try
            {
                await _subscriptionService.UnSubscribe(viewerId, subId);
            }
            catch (DataException exception)
            {
                return NotFound(exception.Message);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
            return Ok();
        }

        /// <summary>
        /// Get a user&#39;s subscriptions
        /// </summary>
        /// <param name="id">User ID</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [ValidateModelState]
        [SwaggerOperation("GetSubscriptions")]
        [SwaggerResponse(statusCode: 200, type: typeof(UserSubscriptionListDto), description: "OK")]
        [AllowAnonymous]
        public virtual async Task<IActionResult> GetSubscriptions([FromQuery(Name = "id")][Required()] Guid id)
        {
            UserSubscriptionListDto subs;
            try
            {
                subs = await _subscriptionService.Subscriptions(id);
            }
            catch (DataException exception)
            {   
                return NotFound(exception.Message);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
            return Ok(subs);
        }
    }
}
