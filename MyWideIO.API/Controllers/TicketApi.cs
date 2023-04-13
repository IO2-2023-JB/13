using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using WideIO.API.Attributes;
using WideIO.API.Models;

namespace MyWideIO.API.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    public class TicketApiController : ControllerBase
    {
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
        public virtual IActionResult GetTicket([FromQuery(Name = "id")][Required()] Guid id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get all tickets related to the calling user
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        [HttpGet]
        [ValidateModelState]
        [SwaggerOperation("GetTicketList")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<GetTicketDto>), description: "OK")]
        public virtual IActionResult GetTicketList()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get current ticket status
        /// </summary>
        /// <param name="id">Ticket ID</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        [HttpGet]
        [ValidateModelState]
        [SwaggerOperation("GetTicketStatus")]
        [SwaggerResponse(statusCode: 200, type: typeof(GetTicketStatusDto), description: "OK")]
        public virtual IActionResult GetTicketStatus([FromQuery(Name = "id")][Required()] Guid id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Submit a response for a ticket
        /// </summary>
        /// <param name="respondToTicketDto"></param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        [HttpPut]
        [Consumes("application/json")]
        [ValidateModelState]
        [SwaggerOperation("RespondToTicket")]
        [SwaggerResponse(statusCode: 200, type: typeof(SubmitTicketResponseDto), description: "OK")]
        public virtual IActionResult RespondToTicket([FromBody] RespondToTicketDto respondToTicketDto)
        {
            throw new NotImplementedException();
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
        public virtual IActionResult SubmitTicket([FromBody] SubmitTicketDto submitTicketDto)
        {
            throw new NotImplementedException();
        }
    }
}
