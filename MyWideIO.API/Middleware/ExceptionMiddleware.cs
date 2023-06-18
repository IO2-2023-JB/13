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
            if (exception is OperationCanceledException)
                Console.WriteLine(exception.Message);
            var statusCode = exception switch
            {
                OperationCanceledException or
                BadRequestException => StatusCodes.Status400BadRequest,
                UserNotFoundException or
                VideoNotFoundException or
                PlaylistNotFoundException or
                CommentNotFoundException or
                NotFoundException or
                TicketNotFoundException => StatusCodes.Status404NotFound,
                DuplicateEmailException => StatusCodes.Status409Conflict,
                IncorrectPasswordException => StatusCodes.Status401Unauthorized,
                VideoIsPrivateException or
                ForbiddenException => StatusCodes.Status403Forbidden,
                //UserNotFoundExceptionDelete => HttpStatusCode.BadRequest,
                CustomException => StatusCodes.Status418ImATeapot,
                _ => StatusCodes.Status500InternalServerError
            };
            var Messages = new List<string> { exception.Message };
            Exception e = exception;
            while (e.InnerException != null)
            {
                e = e.InnerException;
                if (!string.IsNullOrEmpty(e.Message))
                    Messages.Add(e.Message);
            }
            var response = new
            {
                Messeges = Messages
            };
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
        }
    }
}
