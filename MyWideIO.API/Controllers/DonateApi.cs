using Microsoft.AspNetCore.Mvc;
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
    [Route("donate")]
    public class DonateApiController : ControllerBase
    {
        /// <summary>
        /// Send a donation
        /// </summary>
        /// <param name="id">ID of the recipient of the donation</param>
        /// <param name="amount">Amount of money to send</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        [HttpPost("send")]
        [ValidateModelState]
        [SwaggerOperation("SendDonation")]
        public virtual IActionResult SendDonation([FromQuery(Name = "id")][Required()] Guid id, [FromQuery(Name = "amount")][Required()] decimal amount)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Request a withdrawal from account
        /// </summary>
        /// <param name="amount">Amount of money to send</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        [HttpPost("withdraw")]
        [ValidateModelState]
        [SwaggerOperation("WithdrawFunds")]
        public virtual IActionResult WithdrawFunds([FromQuery(Name = "amount")][Required()] decimal amount)
        {
            throw new NotImplementedException();
        }
    }
}
