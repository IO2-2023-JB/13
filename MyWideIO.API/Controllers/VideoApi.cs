using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyWideIO.API.Models.DB_Models;
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
        public virtual async Task<IActionResult> DeleteVideo([FromQuery(Name = "id")][Required()] Guid id)
        {
            if (await _videoService.RemoveVideoIfExist(id))
                return Ok();
            else
                return BadRequest("Video o podanym ID nie istnieje");
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
        /// <param name="accessToken">Access token</param>
        /// <param name="range">VideoFileRange</param>
        /// <response code="200">OK</response>
        /// <response code="206">Partial Content</response>
        /// <response code="401">Unauthorised</response>
        /// <response code="416">Range Not Satisfiable</response>
        [AllowAnonymous]
        [HttpGet("{id}")]
        [ValidateModelState]
        [SwaggerOperation("GetVideoFile")]
        [SwaggerResponse(statusCode: 200, type: typeof(Stream), description: "OK")]
        [SwaggerResponse(statusCode: 206, type: typeof(Stream), description: "Partial Content")]
        public async Task<IActionResult> GetVideoFile([FromRoute(Name = "id")][Required] Guid id, [FromQuery(Name = "access_token")][Required()] string accessToken, [FromHeader] string range)
        {
            
            // cos trzeba z tym tokenem zrobic jak inne grupy nie daja w headerze
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
        /// Video file upload
        /// </summary>
        /// <param name="id">Video ID</param>
        /// <param name="videoFile"></param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorised</response>
        [HttpPost("{id}")]
        [Consumes("multipart/form-data")]
        [ValidateModelState]
        [SwaggerOperation("PostVideoFile")]
        public virtual async Task<IActionResult> PostVideoFile([FromRoute(Name = "id")][Required] Guid id, [FromForm] IFormFile videoFile) // cos w tym stylu
        {
            
            Stream vidStream = videoFile.OpenReadStream();
            try
            {
                 await _videoService.UploadVideoAsync(id, vidStream);
            }
            catch (ApplicationException ex) // already uploading
            {
                return BadRequest(ex.Message); //BadRequest wtedy?
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok();
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
        public virtual async Task<IActionResult> GetVideoMetadata([FromQuery(Name = "id")][Required()] Guid id)
        {
            try
            {
                VideoMetadataDto model = await _videoService.GetVideoMetadata(id);
                return Ok(model);
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
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
        public virtual async Task<IActionResult> GetVideoReactions([FromQuery(Name = "id")][Required()] Guid id)
        {

            Guid viewerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            try
            {
               var a = await _videoService.GetVideoReaction(id, viewerId);
                return Ok(a);
            }
            catch (Exception ex)
            {

            }

            return BadRequest();
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
        /// <response code="500">Internal server error</response>
        [HttpPut("/video-metadata")]
        [Consumes("application/json")]
        [ValidateModelState]
        [SwaggerOperation("UpdateVideoMetadata")]
        public virtual async Task<IActionResult> UpdateVideoMetadata([FromQuery(Name = "id")][Required()] Guid id, [FromBody] VideoUploadDto videoUploadDto)
        {
            if (await _videoService.UpdateVideo(id, videoUploadDto))
            {
                return Ok();
            }
            else
            {
                return BadRequest("No such video");
            }
            // await _videoService.UpdateVideo(id, videoUploadDto);
            // return Ok();
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
        public virtual async Task<IActionResult> UpdateVideoReaction([FromQuery(Name = "id")][Required()] Guid id, [FromBody] VideoReactionUpdateDto videoReactionUpdateDto)
        {
            Guid viewerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            try
            {
                await _videoService.UpdateVideoReaction(id, viewerId, videoReactionUpdateDto);
            }
            catch(Exception ex) 
            {
                return BadRequest(ex.Message);
            }


            return Ok();
        }

        
    }
}
