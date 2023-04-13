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
    [Route("comment")]
    public class CommentApiController : ControllerBase
    {
        /// <summary>
        /// Add comment to video
        /// </summary>
        /// <param name="id">Video ID to which you add comment</param>
        /// <param name="body"></param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        [HttpPost]
        [Consumes("text/plain")]
        [ValidateModelState]
        [SwaggerOperation("AddCommentToVideo")]
        public virtual IActionResult AddCommentToVideo([FromQuery(Name = "id")][Required()] Guid id, [FromBody] string body)
        {
            throw new NotImplementedException();
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
        [Consumes("text/plain")]
        [ValidateModelState]
        [SwaggerOperation("AddResponseToComment")]
        public virtual IActionResult AddResponseToComment([FromQuery(Name = "id")][Required()] Guid id, [FromBody] string body)
        {
            throw new NotImplementedException();
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
        public virtual IActionResult DeleteComment([FromQuery(Name = "id")][Required()] Guid id)
        {
            throw new NotImplementedException();
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
        public virtual IActionResult GetComments([FromQuery(Name = "id")][Required()] Guid id)
        {
            throw new NotImplementedException();
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
        public virtual IActionResult GetResponseData([FromQuery(Name = "id")][Required()] Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
