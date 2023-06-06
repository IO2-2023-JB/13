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
    [Route("subscriptions")]
    [Authorize] // jak jest tutaj, to nie trzeba tego pisac przed kazda metoda
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
        // powinno byc jeszcze code 404 

        [HttpPost]
        [Authorize] // nie potrzebne, jest przed calym kontrolerem
        [ValidateModelState]
        [SwaggerOperation("AddSubscription")]
        public virtual async Task<IActionResult> AddSubscription([FromQuery(Name = "creatorId")][Required()] Guid creatorId)
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid viewerId))
                viewerId = Guid.Empty;//  jesli jest Authorize, ale nie ma ClaimTypes.NameIdentifier, to blad jest(raczej nigdy tak nie bedzie)
                                      // czyli Guid.Parse, bez ifa i Guid.Empty

            try // przeciez jest middleware do obslugi wyjatkow, nie trzeba tego robic w controllerze
            {
                await _subscriptionService.SubscribeAsync(viewerId, creatorId);
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
        // powinno byc jeszcze code 404 

        [HttpDelete]
        [Authorize] // nie potrzebne, jest przed calym kontrolerem
        [ValidateModelState]
        [SwaggerOperation("DeleteSubscription")]
        public virtual async Task<IActionResult> DeleteSubscription([FromQuery(Name = "id")][Required()] Guid subId)
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid viewerId))
                viewerId = Guid.Empty; //  jesli jest Authorize, ale nie ma ClaimTypes.NameIdentifier, to blad jest(raczej nigdy tak nie bedzie)
                                       // czyli Guid.Parse, bez ifa i Guid.Empty

            try // przeciez jest middleware do obslugi wyjatkow, nie trzeba tego robic w controllerze
            {
                await _subscriptionService.UnsubscribeAsync(viewerId, subId);
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
        // powinno byc jeszcze code 404

        [HttpGet]
        [ValidateModelState]
        [SwaggerOperation("GetSubscriptions")]
        [SwaggerResponse(statusCode: 200, type: typeof(UserSubscriptionListDto), description: "OK")]
        [AllowAnonymous] // to jest potrzebne, nadpisuje [Authorize] przed kontrolerem
        public virtual async Task<IActionResult> GetSubscriptions([FromQuery(Name = "id")][Required()] Guid id)
        {
            UserSubscriptionListDto subs;
            try // przeciez jest middleware do obslugi wyjatkow, nie trzeba tego robic w controllerze
            {
                subs = await _subscriptionService.GetSubscriptionsAsync(id);
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
