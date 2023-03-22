/*
 * VideIO API
 *
 * VideIO project API specification.
 *
 * The version of the OpenAPI document: 1.0.0
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
    public class UserApiController : ControllerBase
    { 
        /// <summary>
        /// Ban user
        /// </summary>
        /// <param name="id">User ID</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        [HttpPost]
        [Route("/zagorskim/VideIO/1.0.0/ban/{id}")]
        [ValidateModelState]
        [SwaggerOperation("BanUser")]
        public virtual IActionResult BanUser([FromRoute (Name = "id")][Required]Guid id)
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
        /// User account deletion
        /// </summary>
        /// <param name="id">User ID</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        [HttpDelete]
        [Route("/zagorskim/VideIO/1.0.0/user")]
        [ValidateModelState]
        [SwaggerOperation("DeleteUserData")]
        [SwaggerResponse(statusCode: 200, type: typeof(UserDto), description: "OK")]
        public virtual IActionResult DeleteUserData([FromQuery (Name = "id")][Required()]Guid id)
        {

            //TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(200, default(UserDto));
            //TODO: Uncomment the next line to return response 400 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(400);
            //TODO: Uncomment the next line to return response 401 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(401);
            string exampleJson = null;
            exampleJson = "{\r\n  \"surname\" : \"Doe\",\r\n  \"nickname\" : \"johnny123\",\r\n  \"name\" : \"John\",\r\n  \"id\" : \"123e4567-e89b-12d3-a456-426614174000\",\r\n  \"accountBalance\" : 0.8008281904610115,\r\n  \"email\" : \"john.doe@mail.com\"\r\n}";
            
            var example = exampleJson != null
            ? JsonConvert.DeserializeObject<UserDto>(exampleJson)
            : default(UserDto);
            //TODO: Change the data returned
            return new ObjectResult(example);
        }

        /// <summary>
        /// User data editing
        /// </summary>
        /// <param name="userDto"></param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        [HttpPut]
        [Route("/zagorskim/VideIO/1.0.0/user")]
        [Consumes("application/json")]
        [ValidateModelState]
        [SwaggerOperation("EditUserData")]
        [SwaggerResponse(statusCode: 200, type: typeof(UserDto), description: "OK")]
        public virtual IActionResult EditUserData([FromBody]UserDto userDto)
        {

            //TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(200, default(UserDto));
            //TODO: Uncomment the next line to return response 400 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(400);
            //TODO: Uncomment the next line to return response 401 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(401);
            string exampleJson = null;
            exampleJson = "{\r\n  \"surname\" : \"Doe\",\r\n  \"nickname\" : \"johnny123\",\r\n  \"name\" : \"John\",\r\n  \"id\" : \"123e4567-e89b-12d3-a456-426614174000\",\r\n  \"accountBalance\" : 0.8008281904610115,\r\n  \"email\" : \"john.doe@mail.com\"\r\n}";
            
            var example = exampleJson != null
            ? JsonConvert.DeserializeObject<UserDto>(exampleJson)
            : default(UserDto);
            //TODO: Change the data returned
            return new ObjectResult(example);
        }

        /// <summary>
        /// User data retrieval
        /// </summary>
        /// <param name="id">User ID</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/zagorskim/VideIO/1.0.0/user")]
        [ValidateModelState]
        [SwaggerOperation("GetUserData")]
        [SwaggerResponse(statusCode: 200, type: typeof(UserDto), description: "OK")]
        public virtual IActionResult GetUserData([FromQuery (Name = "id")][Required()]Guid id)
        {

            //TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(200, default(UserDto));
            //TODO: Uncomment the next line to return response 400 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(400);
            //TODO: Uncomment the next line to return response 401 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(401);
            string exampleJson = null;
            exampleJson = "{\r\n  \"surname\" : \"Doe\",\r\n  \"nickname\" : \"johnny123\",\r\n  \"name\" : \"John\",\r\n  \"id\" : \"123e4567-e89b-12d3-a456-426614174000\",\r\n  \"accountBalance\" : 0.8008281904610115,\r\n  \"email\" : \"john.doe@mail.com\"\r\n}";
            
            var example = exampleJson != null
            ? JsonConvert.DeserializeObject<UserDto>(exampleJson)
            : default(UserDto);
            //TODO: Change the data returned
            return new ObjectResult(example);
        }

        /// <summary>
        /// User login
        /// </summary>
        /// <param name="loginDto"></param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        [HttpPost]
        [Route("/zagorskim/VideIO/1.0.0/login")]
        [Consumes("application/json")]
        [ValidateModelState]
        [SwaggerOperation("LoginUser")]
        [SwaggerResponse(statusCode: 200, type: typeof(LoginResponseDto), description: "OK")]
        public virtual IActionResult LoginUser([FromBody]LoginDto loginDto)
        {

            //TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(200, default(LoginResponseDto));
            //TODO: Uncomment the next line to return response 400 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(400);
            string exampleJson = null;
            exampleJson = "Custom MIME type example not yet supported: application:json";
            
            var example = exampleJson != null
            ? JsonConvert.DeserializeObject<LoginResponseDto>(exampleJson)
            : default(LoginResponseDto);
            //TODO: Change the data returned
            return new ObjectResult(example);
        }

        /// <summary>
        /// User registration
        /// </summary>
        /// <param name="registerDto"></param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        [HttpPost]
        [Route("/zagorskim/VideIO/1.0.0/register")]
        [Consumes("application/json")]
        [ValidateModelState]
        [SwaggerOperation("RegisterUser")]
        public virtual IActionResult RegisterUser([FromBody]RegisterDto registerDto)
        {

            //TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(200);
            //TODO: Uncomment the next line to return response 400 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(400);

            throw new NotImplementedException();
        }
    }
}