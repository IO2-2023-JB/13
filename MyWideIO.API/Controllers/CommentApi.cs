using Microsoft.AspNetCore.Mvc;
using MyWideIO.API.Models.Dto_Models;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using WideIO.API.Attributes;
using System.Net.Http;
using MyWideIO.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace MyWideIO.API.Controllers
{

    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Route("comment")]
    [Authorize]
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
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        [HttpPost]
        [Consumes("text/plain")]
        [ValidateModelState]
        [SwaggerOperation("AddCommentToVideo")]
        public virtual async Task<IActionResult> AddCommentToVideo([FromQuery(Name = "id")][Required()] Guid id)
        {
            string claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid userId = Guid.Parse(claim);
            string content;
            using (var reader = new StreamReader(Request.Body))
            {
                content = await reader.ReadToEndAsync();
            }
            await _commentService.AddNewComment(id, content, userId);
            return Ok();
        }

        /// <summary>
        /// Add response to comment
        /// </summary>
        /// <param name="id">Comment ID to which you add response</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        [HttpPost("response")]
        [Consumes("text/plain")]
        [ValidateModelState]
        [SwaggerOperation("AddResponseToComment")]
        public virtual async Task<IActionResult> AddResponseToComment([FromQuery(Name = "id")][Required()] Guid id)
        {
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            string content;
            using (var reader = new StreamReader(Request.Body))
            {
                content = await reader.ReadToEndAsync();
            }
            await _commentService.AddResponseToComment(id, content, userId);
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
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            await _commentService.DeleteComment(id, userId);
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
        public virtual async Task<ActionResult<CommentListDto>> GetComments([FromQuery(Name = "id")][Required()] Guid id)
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
        public virtual async Task<ActionResult<CommentListDto>> GetResponseData([FromQuery(Name = "id")][Required()] Guid id)
        {
            return Ok(await _commentService.GetCommentResponses(id));
        }


        [HttpGet("commentById")]
        [ValidateModelState]
        [SwaggerOperation("GetCommentById")]
        [SwaggerResponse(statusCode: 200, type: typeof(CommentDto), description: "OK")]
        public async Task<ActionResult<CommentDto>> GetCommentById([FromQuery(Name = "id")][Required()] Guid id)
        {
            return Ok(await _commentService.GetCommentById(id));
        }


        [HttpGet("responseById")]
        [ValidateModelState]
        [SwaggerOperation("getCommentResponseById")]
        [SwaggerResponse(statusCode: 200, type: typeof(CommentDto), description: "OK")]
        public async Task<ActionResult<CommentDto>> GetCommentResponseById([FromQuery(Name = "id")][Required()] Guid id)
        {
            return Ok(await _commentService.GetCommentResponseById(id));
        }
    }
}

