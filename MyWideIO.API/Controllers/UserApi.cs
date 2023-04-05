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
using MyWideIO.API.Data.IRepositories;
using MyWideIO.API.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MyWideIO.API.Models.DB_Models;

namespace WideIO.API.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    public class UserApiController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserApiController(IUserService userService, UserManager<ViewerModel> userManager)
        {
            _userService = userService;
        }


        //// Generated code below:

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
        [Authorize(Roles = "Admin")]
        public virtual IActionResult BanUser([FromRoute(Name = "id")][Required] Guid id)
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
        public virtual async Task<IActionResult> DeleteUserData([FromQuery(Name = "id")][Required()] Guid id)
        {

            //TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(200, default(UserDto));
            //TODO: Uncomment the next line to return response 400 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(400);
            //TODO: Uncomment the next line to return response 401 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(401);
            
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (await _userService.DeleteUserAsync(id))
                return Ok();
            else
                return BadRequest();
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
        [Authorize]
        [SwaggerOperation("EditUserData")]
        [SwaggerResponse(statusCode: 200, type: typeof(UserDto), description: "OK")]
        public async virtual Task<IActionResult> EditUserData([FromQuery(Name = "id")] Guid? id, [FromBody] UpdateUserDto updateUserDto)
        {
            //TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(200, default(UserDto));
            //TODO: Uncomment the next line to return response 400 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(400);
            //TODO: Uncomment the next line to return response 401 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(401);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (await _userService.PutUserData(updateUserDto, (Guid)id))
                return Ok();
            else
                return BadRequest(ModelState);
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
        public virtual async Task<IActionResult> GetUserData([FromQuery(Name = "id")][Required()] Guid id)
        {

            //TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(200, default(UserDto));
            //TODO: Uncomment the next line to return response 400 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(400);
            //TODO: Uncomment the next line to return response 401 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(401);
            UserDto? userDto = null;
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if ((userDto = await _userService.GetUser(id)) != null)
                return Ok(userDto);
            else
                return BadRequest(ModelState);

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
        [SwaggerResponse(statusCode: 400, description: "Bad Request")]
        public virtual async Task<IActionResult> LoginUser([FromBody] LoginDto loginDto)
        {
            var token = await _userService.LoginUserAsync(loginDto);
            if (token.IsNullOrEmpty())
                return BadRequest(ModelState);
            else
                return Ok(new LoginResponseDto { Token = token });
            //TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(200, default(LoginResponseDto));
            //TODO: Uncomment the next line to return response 400 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(400);
            
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
        [SwaggerResponse(statusCode: 200, description: "OK")]
        [SwaggerResponse(statusCode: 400, description: "Bad Request")]
        public async virtual Task<IActionResult> RegisterUser([FromBody] RegisterDto registerDto)
        {
            if (await _userService.RegisterUserAsync(registerDto, ModelState))
                return Ok();
            else
                return BadRequest(ModelState);

            //TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(200);
            //TODO: Uncomment the next line to return response 400 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(400);

            throw new NotImplementedException();
        }
    }
}
