using MyWideIO.API.Exceptions;
using Newtonsoft.Json;
namespace MyWideIO.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            Console.WriteLine("middleware");
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var statusCode = exception switch
            {
                UserNotFoundException or VideoNotFoundException =>StatusCodes.Status404NotFound,
                DuplicateEmailException =>StatusCodes.Status409Conflict,
                IncorrectPasswordException =>StatusCodes.Status401Unauthorized,
                VideoIsPrivateException or ForbiddenException =>StatusCodes.Status403Forbidden,
                //UserNotFoundExceptionDelete => HttpStatusCode.BadRequest,
                CustomException =>StatusCodes.Status418ImATeapot,
                _ => StatusCodes.Status500InternalServerError
            };
            var response = new
            {
                exception.Message
            };
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
        }
    }
}
