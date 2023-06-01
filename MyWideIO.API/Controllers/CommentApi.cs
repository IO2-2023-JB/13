using Microsoft.AspNetCore.Mvc;
using MyWideIO.API.Models.Dto_Models;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using WideIO.API.Attributes;
using System.Net.Http;
using MyWideIO.API.Services.Interfaces;

namespace MyWideIO.API.Controllers
{

    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Route("comment")]
    public class CommentApiController : ControllerBase
    {

        private readonly ICommentService _commentService;

        public CommentApiController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        /// <summary>
        /// Add comment to video
        /// </summary>
        /// <param name="id">Video ID to which you add comment</param>
        /// <param name="body"></param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        [HttpPost]
        [Consumes("application/json")]
        [ValidateModelState]
        [SwaggerOperation("AddCommentToVideo")]
        public virtual async Task<IActionResult> AddCommentToVideo([FromQuery(Name = "id")][Required()] Guid id, [FromBody] string body)
        {
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            await _commentService.AddNewComment(id, body, userId);
            return Ok();
        }

        /// <summary>
        /// Add response to comment
        /// </summary>
        /// <param name="id">Comment ID to which you add response</param>
        /// <param name="body"></param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        [HttpPost("response")]
        [Consumes("application/json")]
        [ValidateModelState]
        [SwaggerOperation("AddResponseToComment")]
        public virtual async Task<IActionResult> AddResponseToComment([FromQuery(Name = "id")][Required()] Guid id, [FromBody] string body)
        {
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            await _commentService.AddResponseToComment(id, body, userId);
            return Ok();
        }

        /// <summary>
        /// Comment removal
        /// </summary>
        /// <param name="id">Comment ID</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        [HttpDelete]
        [ValidateModelState]
        [SwaggerOperation("DeleteComment")]
        [SwaggerResponse(statusCode: 404, description: "Not Found")]
        public virtual async Task<IActionResult> DeleteComment([FromQuery(Name = "id")][Required()] Guid id)
        {
            await _commentService.DeleteComment(id);
            return Ok();
        }

        /// <summary>
        /// Get all comments of video
        /// </summary>
        /// <param name="id">Video ID</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [ValidateModelState]
        [SwaggerOperation("GetComments")]
        [SwaggerResponse(statusCode: 200, type: typeof(CommentListDto), description: "OK")]
        [SwaggerResponse(statusCode: 404, description: "Not Found")]
        public virtual async Task<IActionResult> GetComments([FromQuery(Name = "id")][Required()] Guid id)
        {
            return Ok(await _commentService.GetVideoComments(id));
        }

        /// <summary>
        /// Response data
        /// </summary>
        /// <param name="id">Comment ID</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet("response")]
        [ValidateModelState]
        [SwaggerOperation("GetResponseData")]
        [SwaggerResponse(statusCode: 200, type: typeof(CommentListDto), description: "OK")]
        [SwaggerResponse(statusCode: 404, description: "Not Found")]
        public virtual async Task<IActionResult> GetResponseData([FromQuery(Name = "id")][Required()] Guid id)
        {
            return Ok(await _commentService.GetCommentResponses(id));
        }


        [HttpGet("commentById")]
        [ValidateModelState]
        [SwaggerOperation("GetCommentById")]
        [SwaggerResponse(statusCode: 200, type: typeof(CommentDto), description: "OK")]
        public async Task<IActionResult> GetCommentById([FromQuery(Name = "id")][Required()] Guid id)
        {
            return Ok(await _commentService.GetCommentById(id));
        }


        [HttpGet("responseById")]
        [ValidateModelState]
        [SwaggerOperation("getCommentResponseById")]
        [SwaggerResponse(statusCode: 200, type: typeof(CommentDto), description: "OK")]
        public async Task<IActionResult> GetCommentResponseById([FromQuery(Name = "id")][Required()] Guid id)
        {
            return Ok(await _commentService.GetCommentResponseById(id));
        }
    }
}

