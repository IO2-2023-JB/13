using Microsoft.AspNetCore.Mvc;
using MyWideIO.API.Models.Dto_Models;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using MyWideIO.API.Services.Interfaces;
using WideIO.API.Attributes;
using MyWideIO.API.Models.Enums;

namespace MyWideIO.API.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Route("ticket")]
    [Authorize]
    public class TicketApiController : ControllerBase
    {
        private readonly ITicketService _ticketService;
        public TicketApiController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }
        /// <summary>
        /// Get ticket details
        /// </summary>
        /// <param name="id">Ticket ID</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        [HttpGet]
        [ValidateModelState]
        [SwaggerOperation("GetTicket")]
        [SwaggerResponse(statusCode: 200, type: typeof(GetTicketDto), description: "OK")]
        public async Task<IActionResult> GetTicket([FromQuery(Name = "id")][Required()] Guid id, CancellationToken cancellationToken)
        {
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            return Ok(await _ticketService.GetTicketAsync(id, userId, cancellationToken));
        }

        /// <summary>
        /// Get all tickets related to the calling user
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        [HttpGet("list")]
        [ValidateModelState]
        [SwaggerOperation("GetTicketList")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<GetTicketDto>), description: "OK")]
        public async Task<IActionResult> GetTicketList(CancellationToken cancellationToken)
        {
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            return Ok(await _ticketService.GetUserTicketsAsync(userId, cancellationToken));
        }

        /// <summary>
        /// Get current ticket status
        /// </summary>
        /// <param name="id">Ticket ID</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        [HttpGet("status")]
        [ValidateModelState]
        [SwaggerOperation("GetTicketStatus")]
        [Produces("application/json")]
        [SwaggerResponse(statusCode: 200, type: typeof(TicketStatusEnum), description: "OK")]
        public async Task<IActionResult> GetTicketStatus([FromQuery(Name = "id")][Required()] Guid id, CancellationToken cancellationToken)
        {
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            return Ok(await _ticketService.GetTicketStatusAsync(id, userId, cancellationToken));
        }

        /// <summary>
        /// Submit a response for a ticket
        /// </summary>
        /// <param name="respondToTicketDto"></param>
        /// <param name="ticketId"></param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        [HttpPut]
        [Consumes("application/json")]
        [ValidateModelState]
        [SwaggerOperation("RespondToTicket")]
        [SwaggerResponse(statusCode: 200, type: typeof(SubmitTicketResponseDto), description: "OK")]
        [Authorize(Roles ="Administrator")]
        public async Task<IActionResult> RespondToTicket([FromBody] RespondToTicketDto respondToTicketDto, [FromQuery(Name = "id")] Guid ticketId)
        {
            return Ok(await _ticketService.AddResponseToTicketAsync(respondToTicketDto, ticketId));
        }

        /// <summary>
        /// Submit a new ticket
        /// </summary>
        /// <param name="submitTicketDto"></param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        [HttpPost]
        [Consumes("application/json")]
        [ValidateModelState]
        [SwaggerOperation("SubmitTicket")]
        [SwaggerResponse(statusCode: 200, type: typeof(SubmitTicketResponseDto), description: "OK")]
        public async Task<IActionResult> SubmitTicket([FromBody] SubmitTicketDto submitTicketDto)
        {
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            return Ok(await _ticketService.CreateTicketAsync(submitTicketDto, userId));
        }
    }
}
