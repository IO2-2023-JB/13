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
    public class PlaylistApiController : ControllerBase
    {
        /// <summary>
        /// Add video to playlist
        /// </summary>
        /// <param name="id">Playlist ID</param>
        /// <param name="videoId">Video ID to be added</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        [HttpPost]
        [ValidateModelState]
        [SwaggerOperation("AddVideoToPlaylist")]
        public virtual IActionResult AddVideoToPlaylist([FromRoute(Name = "id")][Required] string id, [FromRoute(Name = "videoId")][Required] string videoId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Playlist creation
        /// </summary>
        /// <param name="createPlaylistRequestDto"></param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        [HttpPost]
        [Consumes("application/json")]
        [ValidateModelState]
        [SwaggerOperation("CreatePlaylist")]
        [SwaggerResponse(statusCode: 200, type: typeof(CreatePlaylistResponseDto), description: "OK")]
        public virtual IActionResult CreatePlaylist([FromBody] CreatePlaylistRequestDto createPlaylistRequestDto)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Delete playlist
        /// </summary>
        /// <param name="id">Playlist ID</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        [HttpDelete]
        [ValidateModelState]
        [SwaggerOperation("DeletePlaylist")]
        public virtual IActionResult DeletePlaylist([FromQuery(Name = "id")][Required()] Guid id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Edit playlist properties
        /// </summary>
        /// <param name="id">Playlist ID</param>
        /// <param name="playlistEditDto"></param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        [HttpPut]
        [Consumes("application/json")]
        [ValidateModelState]
        [SwaggerOperation("EditPlaylist")]
        [SwaggerResponse(statusCode: 200, type: typeof(UserDto), description: "OK")]
        public virtual IActionResult EditPlaylist([FromQuery(Name = "id")][Required()] Guid id, [FromBody] PlaylistEditDto playlistEditDto)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get videos in playlist
        /// </summary>
        /// <param name="id">Playlist ID</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [ValidateModelState]
        [SwaggerOperation("GetPlaylistContent")]
        [SwaggerResponse(statusCode: 200, type: typeof(PlaylistDto), description: "OK")]
        public virtual IActionResult GetPlaylistContent([FromQuery(Name = "id")][Required()] Guid id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get all playlists for user
        /// </summary>
        /// <param name="id">User ID</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [ValidateModelState]
        [SwaggerOperation("GetPlaylistsForUser")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<PlaylistBaseDto>), description: "OK")]
        public virtual IActionResult GetPlaylistsForUser([FromQuery(Name = "id")][Required()] Guid id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get recommended videos
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [ValidateModelState]
        [SwaggerOperation("GetRecommendedPlaylist")]
        [SwaggerResponse(statusCode: 200, type: typeof(PlaylistDto), description: "OK")]
        public virtual IActionResult GetRecommendedPlaylist()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Remove video from playlist
        /// </summary>
        /// <param name="id">Playlist ID</param>
        /// <param name="videoId">Video ID to be added</param>
        /// <response code="200">OK</response>
        /// <response code="204">Video already removed</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        [HttpDelete]
        [ValidateModelState]
        [SwaggerOperation("RemoveVideoFromPlaylist")]
        public virtual IActionResult RemoveVideoFromPlaylist([FromRoute(Name = "id")][Required] string id, [FromRoute(Name = "videoId")][Required] string videoId)
        {
            throw new NotImplementedException();
        }
    }
}
