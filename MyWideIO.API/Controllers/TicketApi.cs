/*
 * VideIO API
 *
 * VideIO project API specification.
 *
 * The version of the OpenAPI document: 1.0.0
 * 
 * Generated by: https://openapi-generator.tech
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;
using Newtonsoft.Json;
using WideIO.API.Attributes;
using WideIO.API.Models;

namespace WideIO.API.Controllers
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
        [Route("/zagorskim/VideIO/1.0.0/ticket")]
        [ValidateModelState]
        [SwaggerOperation("GetTicket")]
        [SwaggerResponse(statusCode: 200, type: typeof(GetTicketDto), description: "OK")]
        public virtual IActionResult GetTicket([FromQuery (Name = "id")][Required()]Guid id)
        {

            //TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(200, default(GetTicketDto));
            //TODO: Uncomment the next line to return response 400 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(400);
            string exampleJson = null;
            exampleJson = "Custom MIME type example not yet supported: application:json";
            
            var example = exampleJson != null
            ? JsonConvert.DeserializeObject<GetTicketDto>(exampleJson)
            : default(GetTicketDto);
            //TODO: Change the data returned
            return new ObjectResult(example);
        }

        /// <summary>
        /// Get current ticket status
        /// </summary>
        /// <param name="id">Ticket ID</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        [HttpGet]
        [Route("/zagorskim/VideIO/1.0.0/ticket/status")]
        [ValidateModelState]
        [SwaggerOperation("GetTicketStatus")]
        [SwaggerResponse(statusCode: 200, type: typeof(GetTicketStatusDto), description: "OK")]
        public virtual IActionResult GetTicketStatus([FromQuery (Name = "id")][Required()]Guid id)
        {

            //TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(200, default(GetTicketStatusDto));
            //TODO: Uncomment the next line to return response 400 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(400);
            string exampleJson = null;
            exampleJson = "Custom MIME type example not yet supported: application:json";
            
            var example = exampleJson != null
            ? JsonConvert.DeserializeObject<GetTicketStatusDto>(exampleJson)
            : default(GetTicketStatusDto);
            //TODO: Change the data returned
            return new ObjectResult(example);
        }

        /// <summary>
        /// Submit a new ticket
        /// </summary>
        /// <param name="submitTicketDto"></param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        [HttpPost]
        [Route("/zagorskim/VideIO/1.0.0/ticket")]
        [Consumes("application/json")]
        [ValidateModelState]
        [SwaggerOperation("SubmitTicket")]
        [SwaggerResponse(statusCode: 200, type: typeof(SubmitTicketResponseDto), description: "OK")]
        public virtual IActionResult SubmitTicket([FromBody]SubmitTicketDto submitTicketDto)
        {

            //TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(200, default(SubmitTicketResponseDto));
            //TODO: Uncomment the next line to return response 400 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(400);
            string exampleJson = null;
            exampleJson = "{\r\n  \"id\" : \"123e4567-e89b-12d3-a456-426614174000\"\r\n}";
            
            var example = exampleJson != null
            ? JsonConvert.DeserializeObject<SubmitTicketResponseDto>(exampleJson)
            : default(SubmitTicketResponseDto);
            //TODO: Change the data returned
            return new ObjectResult(example);
        }
    }
}