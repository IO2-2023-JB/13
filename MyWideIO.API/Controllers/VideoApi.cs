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
    public class VideoApiController : ControllerBase
    {
        /// <summary>
        /// Video removal
        /// </summary>
        /// <param name="id">Video ID</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorised</response>
        /// <response code="403">Forbidden</response>
        /// <response code="404">Not found</response>
        [HttpDelete("video")]
        [ValidateModelState]
        [SwaggerOperation("DeleteVideo")]
        public virtual IActionResult DeleteVideo([FromQuery(Name = "id")][Required()] Guid id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get videos uploaded by creators subscribed by logged in user
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet("user/videos/subscribed")]
        [ValidateModelState]
        [SwaggerOperation("GetSubscribedVideos")]
        [SwaggerResponse(statusCode: 200, type: typeof(VideoListDto), description: "OK")]
        public virtual IActionResult GetSubscribedVideos()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get user&#39;s video
        /// </summary>
        /// <param name="id">User ID</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [ValidateModelState]
        [SwaggerOperation("GetUserVideos")]
        [SwaggerResponse(statusCode: 200, type: typeof(VideoListDto), description: "OK")]
        public virtual IActionResult GetUserVideos([FromQuery(Name = "id")][Required()] Guid id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Video file retreival
        /// </summary>
        /// <param name="id">Video ID</param>
        /// <param name="range">VideoFileRange</param>
        /// <response code="200">OK</response>
        /// <response code="206">Partial Content</response>
        /// <response code="401">Unauthorised</response>
        /// <response code="416">Range Not Satisfiable</response>
        [HttpGet]
        [ValidateModelState]
        [SwaggerOperation("GetVideoFile")]
        [SwaggerResponse(statusCode: 200, type: typeof(Stream), description: "OK")]
        [SwaggerResponse(statusCode: 206, type: typeof(Stream), description: "Partial Content")]
        public virtual IActionResult GetVideoFile([FromRoute(Name = "id")][Required] Guid id, [FromHeader] string range)
        {
            throw new NotImplementedException();

        }

        /// <summary>
        /// Video metadata retreival
        /// </summary>
        /// <param name="id">Video ID</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorised</response>
        /// <response code="404">Not found</response>
        [HttpGet]
        [ValidateModelState]
        [SwaggerOperation("GetVideoMetadata")]
        [SwaggerResponse(statusCode: 200, type: typeof(VideoMetadataDto), description: "OK")]
        public virtual IActionResult GetVideoMetadata([FromQuery(Name = "id")][Required()] Guid id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Video reaction retreival
        /// </summary>
        /// <param name="id">Video ID</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="404">Not found</response>
        [HttpGet]
        [ValidateModelState]
        [SwaggerOperation("GetVideoReactions")]
        [SwaggerResponse(statusCode: 200, type: typeof(VideoReactionDto), description: "OK")]
        public virtual IActionResult GetVideoReactions([FromQuery(Name = "id")][Required()] Guid id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Video metadata update
        /// </summary>
        /// <param name="videoUploadDto"></param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorised</response>
        /// <response code="404">Not found</response>
        [HttpPut]
        [Consumes("application/json")]
        [ValidateModelState]
        [SwaggerOperation("UpdateVideoMetadata")]
        public virtual IActionResult UpdateVideoMetadata([FromBody] VideoUploadDto videoUploadDto)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Video reaction update
        /// </summary>
        /// <param name="id">Video ID</param>
        /// <param name="videoReactionUpdateDto"></param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorised</response>
        /// <response code="404">Not found</response>
        [HttpPost]
        [Consumes("application/json")]
        [ValidateModelState]
        [SwaggerOperation("UpdateVideoReaction")]
        public virtual IActionResult UpdateVideoReaction([FromQuery(Name = "id")][Required()] Guid id, [FromBody] VideoReactionUpdateDto videoReactionUpdateDto)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Video upload
        /// </summary>
        /// <param name="videoUploadDto"></param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorised</response>
        [HttpPost]
        [Consumes("application/json")]
        [ValidateModelState]
        [SwaggerOperation("UploadVideo")]
        public virtual IActionResult UploadVideo([FromBody] VideoUploadDto videoUploadDto)
        {
            throw new NotImplementedException();
        }
    }
}
