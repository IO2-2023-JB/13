using Microsoft.AspNetCore.Mvc;
using MyWideIO.API.Models.Dto_Models;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using WideIO.API.Attributes;

namespace MyWideIO.API.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Route("subscribtions")]
    public class SubscriptionsApiController : ControllerBase
    {
        /// <summary>
        /// Subscribe to another user
        /// </summary>
        /// <param name="subId">Subscribed user ID</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        [HttpPost]
        [ValidateModelState]
        [SwaggerOperation("AddSubscription")]
        public virtual IActionResult AddSubscription([FromQuery(Name = "subId")][Required()] Guid subId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Unsubscribe from another user
        /// </summary>
        /// <param name="subId">Subscribed user ID</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        [HttpDelete]
        [ValidateModelState]
        [SwaggerOperation("DeleteSubscription")]
        public virtual IActionResult DeleteSubscription([FromQuery(Name = "subId")][Required()] Guid subId)
        {
            throw new NotImplementedException();
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
        public virtual IActionResult GetSubscriptions([FromQuery(Name = "id")][Required()] Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
