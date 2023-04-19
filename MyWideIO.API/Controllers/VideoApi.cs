using Microsoft.AspNetCore.Mvc;
using MyWideIO.API.Services.Interfaces;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using WideIO.API.Attributes;
using WideIO.API.Models;

namespace MyWideIO.API.Controllers
{


    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Route("video")]
    public class VideoApiController : ControllerBase
    {
        private readonly IVideoService _videoService;

        public VideoApiController(IVideoService videoService)
        {
            _videoService = videoService;
        }

        /// <summary>
        /// Video removal
        /// </summary>
        /// <param name="id">Video ID</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorised</response>
        /// <response code="403">Forbidden</response>
        /// <response code="404">Not found</response>
        [HttpDelete]
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
        [HttpGet("/user/videos/subscribed")]
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
        [HttpGet("/user/videos")]
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
        [HttpGet("{id}")]
        [ValidateModelState]
        [SwaggerOperation("GetVideoFile")]
        [SwaggerResponse(statusCode: 200, type: typeof(Stream), description: "OK")]
        [SwaggerResponse(statusCode: 206, type: typeof(Stream), description: "Partial Content")]
        public async Task<IActionResult> GetVideoFile([FromRoute(Name = "id")][Required] Guid id, [FromHeader] string range)
        {
            var stream = await _videoService.GetVideo(id);
            return File(stream, "video/mp4", true);
        }

        /// <summary>
        /// Video metadata upload
        /// </summary>
        /// <param name="videoUploadDto"></param>
        /// <param name="id"></param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorised</response>
        [HttpPost("/video-metadata")]
        [Consumes("application/json")]
        [ValidateModelState]
        [SwaggerOperation("UpdateVideoMetadata")]
        [SwaggerResponse(statusCode: 200, type: typeof(VideoUploadResponseDto), description: "OK")]
        [SwaggerResponse(statusCode: 400, description: "Bad request")]
        [SwaggerResponse(statusCode: 401, description: "Unauthorized")]
        public virtual async Task<IActionResult> UploadVideoMetadata([FromBody] VideoUploadDto videoUploadDto)
        {
            VideoUploadResponseDto result;
            Guid CreatorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            result = await _videoService.UploadVideoMetadata(videoUploadDto, CreatorId);

            return Ok(result);

        }

        /// <summary>
        /// Video upload
        /// </summary>
        /// <param name="videoUploadDto"></param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorised</response>
        [HttpPost("{id}")]
        [Consumes("application/json")]
        [ValidateModelState]
        [SwaggerOperation("UploadVideo")]
        public virtual IActionResult UploadVideo([FromRoute(Name = "id")][Required] Guid id, [FromBody] VideoUploadDto videoUploadDto)
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
        [HttpGet("/video-metadata")]
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
        [HttpGet("/video-reaction")]
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
        /// <param name="id"></param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorised</response>
        /// <response code="404">Not found</response>
        [HttpPut("/video-metadata")]
        [Consumes("application/json")]
        [ValidateModelState]
        [SwaggerOperation("UpdateVideoMetadata")]
        public virtual async Task<IActionResult> UpdateVideoMetadata([FromBody] VideoUploadDto videoUploadDto, Guid id)
        {
            if (await _videoService.UpdateVideo(id, videoUploadDto))
            {
                return Ok();
            }
            else
            {
                return BadRequest("No such video");
            }
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
        [HttpPost("/video-reaction")]
        [Consumes("application/json")]
        [ValidateModelState]
        [SwaggerOperation("UpdateVideoReaction")]
        public virtual IActionResult UpdateVideoReaction([FromQuery(Name = "id")][Required()] Guid id, [FromBody] VideoReactionUpdateDto videoReactionUpdateDto)
        {
            throw new NotImplementedException();
        }

        
    }
}
