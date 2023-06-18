using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyWideIO.API.Exceptions;
using MyWideIO.API.Services.Interfaces;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading;
using WideIO.API.Attributes;

namespace MyWideIO.API.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Route("donate")]
    [Authorize]
    public class DonateApiController : ControllerBase
    {
        private readonly IDonateService _donateService;
        public DonateApiController(IDonateService donateService)
        {
            _donateService = donateService;
        }

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
        public async virtual Task<IActionResult> SendDonation([FromQuery(Name = "id")][Required()] Guid id, [FromQuery(Name = "amount")][Required()] decimal amount)
        {
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (userId == id)
                throw new DonationException("You can't send money to yourself");
            await _donateService.SendDonation(id, amount);
            return Ok();
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
        public async virtual Task<IActionResult> WithdrawFunds([FromQuery(Name = "amount")][Required()] decimal amount)
        {
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            await _donateService.Withdraw(userId, amount);
            return Ok();
        }
    }
}
