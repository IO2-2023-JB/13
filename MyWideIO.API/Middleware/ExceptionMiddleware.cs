using Azure;
using Microsoft.AspNetCore.Http;
using MyWideIO.API.Exceptions;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Net;

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
            var statusCode = HttpStatusCode.InternalServerError;

            if (exception is UserNotFoundException)
                statusCode = HttpStatusCode.NotFound;

            else if (exception is UserNotFoundExceptionDelete)
                statusCode = HttpStatusCode.BadRequest;

            else if (exception is DuplicateEmailException)
                statusCode = HttpStatusCode.Conflict;

            else if (exception is IncorrectPasswordException)
                statusCode = HttpStatusCode.Unauthorized;

            else if (exception is UserException)
                statusCode = HttpStatusCode.BadRequest;

            var response = new
            {
                exception.Message
            };
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
        }
    }



}