using System.Net;
using System.Text.Json;

namespace TenantFlowERP.Middleware
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
            try
            {
                // Continue request
                await _next(context);
            }
            catch (Exception ex)
            {
                // Handle exception
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(
            HttpContext context,
            Exception exception)
        {
            context.Response.ContentType =
                "application/json";

            context.Response.StatusCode =
                (int)HttpStatusCode.InternalServerError;

            var response = new
            {
                message = "Something went wrong",
                error = exception.Message
            };

            var json =
                JsonSerializer.Serialize(response);

            return context.Response.WriteAsync(json);
        }
    }
}