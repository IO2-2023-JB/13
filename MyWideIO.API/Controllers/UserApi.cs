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
using MyWideIO.API.Exceptions;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Authorization;

namespace WideIO.API.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Authorize]
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
        [HttpPost("ban/{id}")]
        //[Route("ban/{id}")]
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
        [HttpDelete("user")]
        //[Route("user")]
        [ValidateModelState]
        [SwaggerOperation("DeleteUserData")]
        [SwaggerResponse(statusCode: 200, type: typeof(UserDto), description: "OK")]
        public virtual IActionResult DeleteUserData([FromQuery(Name = "id")][Required()] Guid id)
        {

            //TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(200, default(UserDto));
            //TODO: Uncomment the next line to return response 400 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(400);
            //TODO: Uncomment the next line to return response 401 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(401);
            string exampleJson = null!;
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
        [HttpPut("user")]
        //[Route("user")]
        [Consumes("application/json")]
        [ValidateModelState]
        [SwaggerOperation("EditUserData")]
        [SwaggerResponse(statusCode: 200, type: typeof(UserDto), description: "OK")]
        [SwaggerResponse(statusCode: 400, description: "Bad Request")]
        [SwaggerResponse(statusCode: 401, description: "Unauthorized")]
        public async virtual Task<IActionResult> EditUserData([FromBody] UserDto userDto)
        {
            if (User.FindFirstValue(ClaimTypes.NameIdentifier) != userDto.Id.ToString())
                return Unauthorized();
            try
            {
                await _userService.EditUserDataAsync(userDto);
                return Ok(userDto);// po co sie zwraca to 
            }
            catch (UserException e) // blad
            {
                return BadRequest(e.Message);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// User data retrieval. No id parameter results in sending the data of the currently logged in user.
        /// </summary>
        /// <param name="id">User ID</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Not found</response>
        [HttpGet("user")]
        //[Route("user")]
        [ValidateModelState]
        [SwaggerOperation("GetUserData")]
        [SwaggerResponse(statusCode: 200, description: "OK", type: typeof(UserDto))]
        [SwaggerResponse(statusCode: 400, description: "Bad request")]
        [SwaggerResponse(statusCode: 401, description: "Unauthorized")]
        [SwaggerResponse(statusCode: 404, description: "Not found")]

        public virtual async Task<IActionResult> GetUserData([FromQuery(Name = "id")] Guid? id)
        {
            if (id == null)
                id = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            try
            {
                var userDto = await _userService.GetUserAsync(id.Value);
                return Ok(userDto);
            }
            catch(UserNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (UserException e) // blad
            {
                return BadRequest(e.Message);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// User login
        /// </summary>
        /// <param name="loginDto"></param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Incorrect password</response>
        /// <response code="404">Account does not exist</response>
        [HttpPost("login")]
        //[Route("login")]
        [Consumes("application/json")]
        [ValidateModelState]
        [SwaggerOperation("LoginUser")]
        [AllowAnonymous]
        [SwaggerResponse(statusCode: 200, type: typeof(LoginResponseDto), description: "OK")]
        [SwaggerResponse(statusCode: 400, description: "Bad Request")]
        [SwaggerResponse(statusCode: 401, description: "Incorrect password")]
        [SwaggerResponse(statusCode: 404, description: "Account does not exist")]

        public virtual async Task<IActionResult> LoginUser([FromBody] LoginDto loginDto)
        {
            try
            {
                var token = await _userService.LoginUserAsync(loginDto);
                return Ok(new LoginResponseDto { Token = token });
            }
            catch (IncorrectPasswordException e)
            {
                return Unauthorized(e.Message);
            }
            catch (UserNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (UserException e) // blad
            {
                return BadRequest(e.Message);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// User registration
        /// </summary>
        /// <param name="registerDto"></param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="409">A user with this e-mail address already exists</response>
        [HttpPost("register")]
        //[Route("register")]
        [Consumes("application/json")]
        [ValidateModelState]
        [AllowAnonymous]
        [SwaggerOperation("RegisterUser")]
        [SwaggerResponse(statusCode: 200, description: "OK")]
        [SwaggerResponse(statusCode: 400, description: "Bad Request")]
        [SwaggerResponse(statusCode: 409, description: "A user with this e-mail address already exists")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterDto registerDto)
        {
            try
            {
                await _userService.RegisterUserAsync(registerDto);
                return Ok();
            }
            catch (DuplicateEmailException e) // email juz istnieje
            {
                return Conflict(e.Message);
            }
            catch (UserException e) // inny blad
            {
                return BadRequest(e.Message);
            }
            catch
            {
                throw;
            }
        }
    }
}
