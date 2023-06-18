using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Validations;
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
    [Authorize]
    [Route("playlist")]
    public class PlaylistApiController : ControllerBase
    {
        private readonly IPlaylistService _playlistService;

        public PlaylistApiController(IPlaylistService playlistService)
        {
            _playlistService = playlistService;
        }

        /// <summary>
        /// Add video to playlist
        /// </summary>
        /// <param name="id">Playlist ID</param>
        /// <param name="videoId">Video ID to be added</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <response code="404">Not Found</response>
        [HttpPost("{id}/{videoId}")]
        [ValidateModelState]
        [SwaggerOperation("AddVideoToPlaylist")]
        public async Task<IActionResult> AddVideoToPlaylist([FromRoute(Name = "id")][Required] Guid id, [FromRoute(Name = "videoId")][Required] Guid videoId)
        {
            Guid viewerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            await _playlistService.AddVideoToPlaylistAsync(viewerId, id, videoId);
            return Ok();
        }

        /// <summary>
        /// Playlist creation
        /// </summary>
        /// <param name="createPlaylistRequestDto"></param>
        /// <response code="201">Created</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        [HttpPost("details")]
        [Consumes("application/json")]
        [ValidateModelState]
        [SwaggerOperation("CreatePlaylist")]
        [SwaggerResponse(statusCode: 201, type: typeof(CreatePlaylistResponseDto), description: "Created")]
        public async Task<IActionResult> CreatePlaylist([FromBody] CreatePlaylistRequestDto createPlaylistRequestDto)
        {
            Guid viewerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var createPlaylistResponseDto = await _playlistService.CreatePlaylistAsync(viewerId, createPlaylistRequestDto);
            return StatusCode(201, createPlaylistResponseDto);
        }

        /// <summary>
        /// Delete playlist
        /// </summary>
        /// <param name="id">Playlist ID</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        [HttpDelete("details")]
        [ValidateModelState]
        [SwaggerOperation("DeletePlaylist")]
        public async Task<IActionResult> DeletePlaylist([FromQuery(Name = "id")][Required()] Guid id)
        {
            Guid viewerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            await _playlistService.RemovePlaylistAsync(viewerId, id);
            return Ok();
        }

        /// <summary>
        /// Edit playlist properties
        /// </summary>
        /// <param name="id">Playlist ID</param>
        /// <param name="playlistEditDto"></param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <response code="404">Not Found</response>
        [HttpPut("details")]
        [Consumes("application/json")]
        [ValidateModelState]
        [SwaggerOperation("EditPlaylist")]
        [SwaggerResponse(statusCode: 200, type: typeof(PlaylistDto), description: "OK")]
        public async Task<IActionResult> EditPlaylist([FromQuery(Name = "id")][Required()] Guid id, [FromBody] PlaylistEditDto playlistEditDto)
        {
            Guid viewerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var playlistDto = await _playlistService.EditPlaylistAsync(viewerId, id, playlistEditDto);
            return Ok(playlistDto);
        }

        /// <summary>
        /// Get videos in playlist
        /// </summary>
        /// <param name="id">Playlist ID</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <response code="404">Not Found</response>
        [HttpGet("video")]
        [ValidateModelState]
        [SwaggerOperation("GetPlaylistContent")]
        [SwaggerResponse(statusCode: 200, type: typeof(PlaylistDto), description: "OK")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPlaylistContent([FromQuery(Name = "id")][Required()] Guid id)
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid viewerId))
                viewerId = Guid.Empty;
            var playlistDto = await _playlistService.GetPlaylistAsync(viewerId, id);
            return Ok(playlistDto);
        }

        /// <summary>
        /// Get all playlists for user
        /// </summary>
        /// <param name="id">User ID</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Not Found</response>
        [HttpGet("user")]
        [ValidateModelState]
        [SwaggerOperation("GetPlaylistsForUser")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<PlaylistBaseDto>), description: "OK")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPlaylistsForUser([FromQuery(Name = "id")][Required()] Guid id)
        {
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid viewerId))
                viewerId = Guid.Empty;
            var list = await _playlistService.GetUserPlaylistsAsync(viewerId, id);
            return Ok(list);

        }

        /// <summary>
        /// Get recommended videos
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet("recommended")]
        [ValidateModelState]
        [SwaggerOperation("GetRecommendedPlaylist")]
        [SwaggerResponse(statusCode: 200, type: typeof(PlaylistDto), description: "OK")]
        public virtual async Task<ActionResult<PlaylistDto>> GetRecommendedPlaylist()
        {
            Guid viewerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            return Ok(await _playlistService.GetReccomendedVideosPlaylist(viewerId));
        }

        /// <summary>
        /// Remove video from playlist
        /// </summary>
        /// <param name="id">Playlist ID</param>
        /// <param name="videoId">Video ID to be added</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <response code="404">Not Found</response>
        [HttpDelete("{id}/{videoId}")]
        [ValidateModelState]
        [SwaggerOperation("RemoveVideoFromPlaylist")]
        public async Task<IActionResult> RemoveVideoFromPlaylist([FromRoute(Name = "id")][Required] Guid id, [FromRoute(Name = "videoId")][Required] Guid videoId)
        {
            Guid viewerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            await _playlistService.RemoveVideoFromPlaylistAsync(viewerId, id, videoId);
            return Ok();
        }
    }
}
