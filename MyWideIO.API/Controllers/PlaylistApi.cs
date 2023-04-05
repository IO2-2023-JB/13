/*
 * VideIO API
 *
 * VideIO project API specification.
 *
 * The version of the OpenAPI document: 1.0.6
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
        [Route("/zagorskim/VideIO/1.0.0/playlist/{id}/{videoId}")]
        [ValidateModelState]
        [SwaggerOperation("AddVideoToPlaylist")]
        public virtual IActionResult AddVideoToPlaylist([FromRoute (Name = "id")][Required]string id, [FromRoute (Name = "videoId")][Required]string videoId)
        {

            //TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(200);
            //TODO: Uncomment the next line to return response 400 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(400);
            //TODO: Uncomment the next line to return response 401 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(401);

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
        [Route("/zagorskim/VideIO/1.0.0/playlist/details")]
        [Consumes("application/json")]
        [ValidateModelState]
        [SwaggerOperation("CreatePlaylist")]
        [SwaggerResponse(statusCode: 200, type: typeof(CreatePlaylistResponseDto), description: "OK")]
        public virtual IActionResult CreatePlaylist([FromBody]CreatePlaylistRequestDto createPlaylistRequestDto)
        {

            //TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(200, default(CreatePlaylistResponseDto));
            //TODO: Uncomment the next line to return response 400 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(400);
            //TODO: Uncomment the next line to return response 401 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(401);
            string exampleJson = null;
            exampleJson = "{\r\n  \"id\" : \"123e4567-e89b-12d3-a456-426614174000\"\r\n}";
            
            var example = exampleJson != null
            ? JsonConvert.DeserializeObject<CreatePlaylistResponseDto>(exampleJson)
            : default(CreatePlaylistResponseDto);
            //TODO: Change the data returned
            return new ObjectResult(example);
        }

        /// <summary>
        /// Delete playlist
        /// </summary>
        /// <param name="id">Playlist ID</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        [HttpDelete]
        [Route("/zagorskim/VideIO/1.0.0/playlist/details")]
        [ValidateModelState]
        [SwaggerOperation("DeletePlaylist")]
        public virtual IActionResult DeletePlaylist([FromQuery (Name = "id")][Required()]Guid id)
        {

            //TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(200);
            //TODO: Uncomment the next line to return response 400 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(400);
            //TODO: Uncomment the next line to return response 401 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(401);

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
        [Route("/zagorskim/VideIO/1.0.0/playlist/details")]
        [Consumes("application/json")]
        [ValidateModelState]
        [SwaggerOperation("EditPlaylist")]
        [SwaggerResponse(statusCode: 200, type: typeof(UserDto), description: "OK")]
        public virtual IActionResult EditPlaylist([FromQuery (Name = "id")][Required()]Guid id, [FromBody]PlaylistEditDto playlistEditDto)
        {

            //TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(200, default(UserDto));
            //TODO: Uncomment the next line to return response 400 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(400);
            //TODO: Uncomment the next line to return response 401 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(401);
            string exampleJson = null;
            exampleJson = "{\r\n  \"surname\" : \"Doe\",\r\n  \"avatarImage\" : \"https://example.com/avatar/user-id\",\r\n  \"nickname\" : \"johnny123\",\r\n  \"name\" : \"John\",\r\n  \"id\" : \"123e4567-e89b-12d3-a456-426614174000\",\r\n  \"accountBalance\" : 0.8008281904610115,\r\n  \"subscriptionsCount\" : 6,\r\n  \"email\" : \"john.doe@mail.com\"\r\n}";
            
            var example = exampleJson != null
            ? JsonConvert.DeserializeObject<UserDto>(exampleJson)
            : default(UserDto);
            //TODO: Change the data returned
            return new ObjectResult(example);
        }

        /// <summary>
        /// Get videos in playlist
        /// </summary>
        /// <param name="id">Playlist ID</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/zagorskim/VideIO/1.0.0/playlist/video")]
        [ValidateModelState]
        [SwaggerOperation("GetPlaylistContent")]
        [SwaggerResponse(statusCode: 200, type: typeof(PlaylistDto), description: "OK")]
        public virtual IActionResult GetPlaylistContent([FromQuery (Name = "id")][Required()]Guid id)
        {

            //TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(200, default(PlaylistDto));
            //TODO: Uncomment the next line to return response 400 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(400);
            //TODO: Uncomment the next line to return response 401 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(401);
            string exampleJson = null;
            exampleJson = "{\r\n  \"name\" : \"Favorites\",\r\n  \"videos\" : [ {\r\n    \"duration\" : \"743\",\r\n    \"thumbnail\" : \"https://example.com/thumbnail/video-id\",\r\n    \"uploadDate\" : \"2000-01-23T04:56:07.000+00:00\",\r\n    \"description\" : \"All right, so here we are in front of the elephants, the cool thing about these guys is that they have really, really, really long trunks, and that's, that's cool. And that's pretty much all there is to say.\",\r\n    \"id\" : \"123e4567-e89b-12d3-a456-426614174000\",\r\n    \"viewCount\" : 1234503,\r\n    \"title\" : \"Me at the zoo\"\r\n  }, {\r\n    \"duration\" : \"743\",\r\n    \"thumbnail\" : \"https://example.com/thumbnail/video-id\",\r\n    \"uploadDate\" : \"2000-01-23T04:56:07.000+00:00\",\r\n    \"description\" : \"All right, so here we are in front of the elephants, the cool thing about these guys is that they have really, really, really long trunks, and that's, that's cool. And that's pretty much all there is to say.\",\r\n    \"id\" : \"123e4567-e89b-12d3-a456-426614174000\",\r\n    \"viewCount\" : 1234503,\r\n    \"title\" : \"Me at the zoo\"\r\n  } ]\r\n}";
            
            var example = exampleJson != null
            ? JsonConvert.DeserializeObject<PlaylistDto>(exampleJson)
            : default(PlaylistDto);
            //TODO: Change the data returned
            return new ObjectResult(example);
        }

        /// <summary>
        /// Get all playlists for user
        /// </summary>
        /// <param name="id">User ID</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/zagorskim/VideIO/1.0.0/playlist/user")]
        [ValidateModelState]
        [SwaggerOperation("GetPlaylistsForUser")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<PlaylistBaseDto>), description: "OK")]
        public virtual IActionResult GetPlaylistsForUser([FromQuery (Name = "id")][Required()]Guid id)
        {

            //TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(200, default(List<PlaylistBaseDto>));
            //TODO: Uncomment the next line to return response 400 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(400);
            //TODO: Uncomment the next line to return response 401 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(401);
            string exampleJson = null;
            exampleJson = "[ {\r\n  \"name\" : \"Favorites\",\r\n  \"count\" : 25,\r\n  \"id\" : \"123e4567-e89b-12d3-a456-426614174000\"\r\n}, {\r\n  \"name\" : \"Favorites\",\r\n  \"count\" : 25,\r\n  \"id\" : \"123e4567-e89b-12d3-a456-426614174000\"\r\n} ]";
            
            var example = exampleJson != null
            ? JsonConvert.DeserializeObject<List<PlaylistBaseDto>>(exampleJson)
            : default(List<PlaylistBaseDto>);
            //TODO: Change the data returned
            return new ObjectResult(example);
        }

        /// <summary>
        /// Get recommended videos
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/zagorskim/VideIO/1.0.0/playlist/recommended")]
        [ValidateModelState]
        [SwaggerOperation("GetRecommendedPlaylist")]
        [SwaggerResponse(statusCode: 200, type: typeof(PlaylistDto), description: "OK")]
        public virtual IActionResult GetRecommendedPlaylist()
        {

            //TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(200, default(PlaylistDto));
            //TODO: Uncomment the next line to return response 400 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(400);
            //TODO: Uncomment the next line to return response 401 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(401);
            string exampleJson = null;
            exampleJson = "{\r\n  \"name\" : \"Favorites\",\r\n  \"videos\" : [ {\r\n    \"duration\" : \"743\",\r\n    \"thumbnail\" : \"https://example.com/thumbnail/video-id\",\r\n    \"uploadDate\" : \"2000-01-23T04:56:07.000+00:00\",\r\n    \"description\" : \"All right, so here we are in front of the elephants, the cool thing about these guys is that they have really, really, really long trunks, and that's, that's cool. And that's pretty much all there is to say.\",\r\n    \"id\" : \"123e4567-e89b-12d3-a456-426614174000\",\r\n    \"viewCount\" : 1234503,\r\n    \"title\" : \"Me at the zoo\"\r\n  }, {\r\n    \"duration\" : \"743\",\r\n    \"thumbnail\" : \"https://example.com/thumbnail/video-id\",\r\n    \"uploadDate\" : \"2000-01-23T04:56:07.000+00:00\",\r\n    \"description\" : \"All right, so here we are in front of the elephants, the cool thing about these guys is that they have really, really, really long trunks, and that's, that's cool. And that's pretty much all there is to say.\",\r\n    \"id\" : \"123e4567-e89b-12d3-a456-426614174000\",\r\n    \"viewCount\" : 1234503,\r\n    \"title\" : \"Me at the zoo\"\r\n  } ]\r\n}";
            
            var example = exampleJson != null
            ? JsonConvert.DeserializeObject<PlaylistDto>(exampleJson)
            : default(PlaylistDto);
            //TODO: Change the data returned
            return new ObjectResult(example);
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
        [Route("/zagorskim/VideIO/1.0.0/playlist/{id}/{videoId}")]
        [ValidateModelState]
        [SwaggerOperation("RemoveVideoFromPlaylist")]
        public virtual IActionResult RemoveVideoFromPlaylist([FromRoute (Name = "id")][Required]string id, [FromRoute (Name = "videoId")][Required]string videoId)
        {

            //TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(200);
            //TODO: Uncomment the next line to return response 204 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(204);
            //TODO: Uncomment the next line to return response 400 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(400);
            //TODO: Uncomment the next line to return response 401 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(401);

            throw new NotImplementedException();
        }
    }
}
