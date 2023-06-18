using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyWideIO.API.Exceptions;
using MyWideIO.API.Models.Dto_Models;
using MyWideIO.API.Models.Enums;
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
    public class UserApiController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserApiController(IUserService userService)
        {
            _userService = userService;
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
        [Produces("application/json")]
        [SwaggerResponse(statusCode: 200, type: typeof(UserDto), description: "OK")]
        [SwaggerResponse(statusCode: 400, description: "Bad Request")]
        [SwaggerResponse(statusCode: 401, description: "Unauthorized")] // jeszcze 404 i 403 by sie przydaly
        public async Task<IActionResult> DeleteUserData([FromQuery(Name = "id")][Required()] Guid id)
        {
            if (id != Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier))) // jesli user probuje usunac nie swoje konto
                if (User.FindFirstValue(ClaimTypes.Role) != UserTypeEnum.Administrator.ToString()) // i nie jest adminem
                    return Forbid();
            await _userService.DeleteUserAsync(id);
            return Ok();
        }

        /// <summary>
        /// User data editing
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="updateUserDto"></param>
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
        [Authorize(Roles = "Creator,Simple")]
        public async Task<IActionResult> EditUserData([FromQuery(Name = "id")] Guid? id, [FromBody] UpdateUserDto updateUserDto)
        {
            if (id is null)
                id = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            else if (id != Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier))) // jesli user probuje edytowac nie swoje konto
                return Forbid();

            UserDto userDto = await _userService.EditUserDataAsync(updateUserDto, id.Value);
            return Ok(userDto);
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
        [Produces("application/json")]
        [SwaggerOperation("GetUserData")]
        [SwaggerResponse(statusCode: 200, description: "OK", type: typeof(UserDto))]
        [SwaggerResponse(statusCode: 400, description: "Bad request")]
        [SwaggerResponse(statusCode: 401, description: "Unauthorized")]
        [SwaggerResponse(statusCode: 404, description: "Not found")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUserData([FromQuery(Name = "id")] Guid? id)
        {
            if (id is null)
            {
                if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid nId))
                    return BadRequest("Not logged in users must provide an id");
                id = nId;
            }
            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out Guid askerId))
                askerId = Guid.Empty;
            UserDto userDto = await _userService.GetUserAsync(id.Value, askerId);
            return Ok(userDto);
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
        [Produces("application/json")]
        [ValidateModelState]
        [SwaggerOperation("LoginUser")]
        [AllowAnonymous] // nadpisuje [Authorize]
        [SwaggerResponse(statusCode: 200, type: typeof(LoginResponseDto), description: "OK")]
        [SwaggerResponse(statusCode: 400, description: "Bad Request")]
        [SwaggerResponse(statusCode: 401, description: "Incorrect password")]
        [SwaggerResponse(statusCode: 404, description: "Account does not exist")]

        public async Task<IActionResult> LoginUser([FromBody] LoginDto loginDto)
        {
            string token = await _userService.LoginUserAsync(loginDto);
            return Ok(new LoginResponseDto { Token = token });
        }

        /// <summary>
        /// User registration
        /// </summary>
        /// <param name="registerDto"></param>
        /// <response code="201">Created</response>
        /// <response code="400">Bad request</response>
        /// <response code="409">A user with this e-mail address already exists</response>
        [HttpPost("register")]
        //[Route("register")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ValidateModelState]
        [AllowAnonymous]
        [SwaggerOperation("RegisterUser")]
        [SwaggerResponse(statusCode: 201, description: "Created")]
        [SwaggerResponse(statusCode: 400, description: "Bad Request")]
        [SwaggerResponse(statusCode: 409, description: "A user with this e-mail address already exists")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterDto registerDto)
        {
            await _userService.RegisterUserAsync(registerDto);
            return StatusCode(201);
        }
    }
}
