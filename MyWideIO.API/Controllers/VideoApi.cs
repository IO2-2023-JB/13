using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyWideIO.API.Exceptions;
using MyWideIO.API.Models.DB_Models;
using MyWideIO.API.Models.Dto_Models;
using MyWideIO.API.Services.Interfaces;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using WideIO.API.Attributes;

namespace MyWideIO.API.Controllers
{


    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Route("video")]
    [Authorize]
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
        /// <param name="videoId">Video ID</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorised</response>
        /// <response code="403">Forbidden</response>
        /// <response code="404">Not found</response>
        [HttpDelete]
        [ValidateModelState]
        [SwaggerOperation("DeleteVideo")]
        [Authorize(Roles = "Creator,Administrator")]
        public virtual async Task<IActionResult> DeleteVideo([FromQuery(Name = "id")][Required()] Guid videoId)
        {
            Guid creatorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            await _videoService.RemoveVideoAsync(videoId, creatorId);
            return Ok();
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
        [SwaggerResponse(statusCode: 400, description: "Bad Request")]
        public virtual async Task<IActionResult> GetSubscribedVideos()
        {
            Guid subscriber = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            VideoListDto subscribedVideos = await _videoService.GetVideosSubscribedByUser(subscriber);
            return Ok(subscribedVideos);
        }

        /// <summary>
        /// Get user&#39;s video
        /// </summary>
        /// <param name="ceatorId">User ID</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet("/user/videos")]
        [ValidateModelState]
        [SwaggerOperation("GetUserVideos")]
        [SwaggerResponse(statusCode: 200, type: typeof(VideoListDto), description: "OK")]
        [AllowAnonymous]
        public virtual async Task<IActionResult> GetUserVideos([FromQuery(Name = "id")][Required()] Guid ceatorId)
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid viewerId))
                viewerId = Guid.Empty;
            var videoListDto = await _videoService.GetUserVideosAsync(ceatorId, viewerId);
            return Ok(videoListDto);
        }

        /// <summary>
        /// Video file retreival
        /// </summary>
        /// <param name="videoId">Video ID</param>
        /// <param name="accessToken">Access token</param>
        /// <param name="range">VideoFileRange</param>
        /// <response code="200">OK</response>
        /// <response code="206">Partial Content</response>
        /// <response code="401">Unauthorised</response>
        /// <response code="403">Forbidden</response>
        /// <response code="404">Not Found</response>
        /// <response code="416">Range Not Satisfiable</response>
        [HttpGet("{id}")]
        [ValidateModelState]
        [SwaggerOperation("GetVideoFile")]
        [SwaggerResponse(statusCode: 200, type: typeof(FileStreamResult), description: "OK")]
        [SwaggerResponse(statusCode: 206, type: typeof(FileStreamResult), description: "Partial Content")]
        [Produces("video/mp4")]
        [AllowAnonymous]
        public async Task<IActionResult> GetVideoFile([FromRoute(Name = "id")][Required] Guid videoId, [FromQuery(Name = "access_token")][Required()] string accessToken, [FromHeader] string range, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid viewerId))
                viewerId = Guid.Empty;
            var stream = await _videoService.GetVideoAsync(videoId, viewerId, cancellationToken);
            return File(stream, "video/mp4", true);
        }

        /// <summary>
        /// Video metadata upload
        /// </summary>
        /// <param name="videoUploadDto"></param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorised</response>
        [HttpPost("/video-metadata")]
        [Consumes("application/json")]
        [ValidateModelState]
        [SwaggerOperation("UpdateVideoMetadata")]
        [SwaggerResponse(statusCode: 200, type: typeof(VideoUploadResponseDto), description: "OK")]
        [Authorize(Roles = "Creator")]
        public virtual async Task<IActionResult> UploadVideoMetadata([FromBody] VideoUploadDto videoUploadDto)
        {
            Guid creatorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _videoService.UploadVideoMetadataAsync(videoUploadDto, creatorId);

            return Ok(result);

        }
        /// <summary>
        /// Video file upload
        /// </summary>
        /// <param name="videoId">Video ID</param>
        /// <param name="videoFile"></param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorised</response>
        /// <response code="500">Internal server error</response>
        [HttpPost("{id}")]
        [Consumes("multipart/form-data")]
        [ValidateModelState]
        [SwaggerOperation("PostVideoFile")]
        [Authorize(Roles = "Creator")]
        [RequestSizeLimit(1_000_000_000)]
        [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = long.MaxValue)]

        public virtual async Task<IActionResult> PostVideoFile([FromRoute(Name = "id")][Required] Guid videoId, [FromForm] IFormFile videoFile, CancellationToken cancellationToken)
        {
            Guid creatorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            string ext = videoFile.ContentType.ToLower();

            if (!allowedExtensions.Contains(ext))
                throw new VideoException("File extension error");
            using (Stream vidStream = videoFile.OpenReadStream())
            {
                await _videoService.UploadVideoAsync(videoId, creatorId, vidStream, ext, cancellationToken);
            }

            return Ok();

        }
        private /*static*/ readonly HashSet<string> allowedExtensions = new() { "video/mp4", "video/webm", "video/avi", "video/x-matroska" };
        /// <summary>
        /// Video metadata retreival
        /// </summary>
        /// <param name="videoId">Video ID</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorised</response>
        /// <response code="403">Forbidden</response>
        /// <response code="404">Not found</response>y
        [HttpGet("/video-metadata")]
        [ValidateModelState]
        [SwaggerOperation("GetVideoMetadata")]
        [SwaggerResponse(statusCode: 200, type: typeof(VideoMetadataDto), description: "OK")]
        [AllowAnonymous]
        public virtual async Task<IActionResult> GetVideoMetadata([FromQuery(Name = "id")][Required()] Guid videoId)
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid viewerId))
                viewerId = Guid.Empty;

            VideoMetadataDto model = await _videoService.GetVideoMetadataAsync(videoId, viewerId);
            return Ok(model);
        }


        /// <summary>
        /// Video reaction retreival
        /// </summary>
        /// <param name="videoId">Video ID</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="404">Not found</response>
        [HttpGet("/video-reaction")]
        [ValidateModelState]
        [SwaggerOperation("GetVideoReactions")]
        [SwaggerResponse(statusCode: 200, type: typeof(VideoReactionDto), description: "OK")]
        [AllowAnonymous]
        public virtual async Task<IActionResult> GetVideoReactions([FromQuery(Name = "id")][Required()] Guid videoId)
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid viewerId))
                viewerId = Guid.Empty;
            VideoReactionDto videoReactionDto = await _videoService.GetVideoReactionAsync(videoId, viewerId);
            return Ok(videoReactionDto);
        }

        /// <summary>
        /// Video metadata update
        /// </summary>
        /// <param name="videoUploadDto"></param>
        /// <param name="videoId"></param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorised</response>
        /// <response code="403">Forbidden</response>
        /// <response code="404">Not found</response>
        [HttpPut("/video-metadata")]
        [Consumes("application/json")]
        [ValidateModelState]
        [SwaggerOperation("UpdateVideoMetadata")]
        [Authorize(Roles = "Creator")]
        public virtual async Task<IActionResult> UpdateVideoMetadata([FromQuery(Name = "id")][Required()] Guid videoId, [FromBody] VideoUploadDto videoUploadDto)
        {
            Guid creatorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            await _videoService.UpdateVideoAsync(videoId, creatorId, videoUploadDto);
            return Ok();
        }

        /// <summary>
        /// Video reaction update
        /// </summary>
        /// <param name="videoId">Video ID</param>
        /// <param name="videoReactionUpdateDto"></param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorised</response>
        /// <response code="403">Forbidden</response>
        /// <response code="404">Not found</response>
        [HttpPost("/video-reaction")]
        [Consumes("application/json")]
        [ValidateModelState]
        [SwaggerOperation("UpdateVideoReaction")]
        public virtual async Task<IActionResult> UpdateVideoReaction([FromQuery(Name = "id")][Required()] Guid videoId, [FromBody] VideoReactionUpdateDto videoReactionUpdateDto)
        {
            Guid viewerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            await _videoService.UpdateVideoReactionAsync(videoId, viewerId, videoReactionUpdateDto);
            return Ok();
        }


    }
}
