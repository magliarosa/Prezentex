using Prezentex.Application.Exceptions;

namespace Prezentex.Api.Middleware
{
    public class HttpExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public HttpExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        // IMessageWriter is injected into InvokeAsync
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                httpContext = await SetResponse(httpContext, ex);
            }
        }

        private static async Task<HttpContext> SetResponse(HttpContext httpContext, Exception ex)
        {
            int statusCode;
            string message = String.Empty;
            switch (ex)
            {
                case ArgumentException _:
                    statusCode = StatusCodes.Status400BadRequest;
                    message = ex.Message;
                    break;
                case ResourceNotFoundException _:
                    statusCode = StatusCodes.Status404NotFound;
                    message = ex.Message;
                    break;
                case UnauthorizedAccessException _:
                    statusCode = StatusCodes.Status403Forbidden;
                    message = ex.Message;
                    break;
                default:
                    statusCode = StatusCodes.Status500InternalServerError;
                    break;
            }

            httpContext.Response.StatusCode = statusCode;
            if (!String.IsNullOrWhiteSpace(message))
                await httpContext.Response.WriteAsJsonAsync(new { errors = ex.Message });

            return httpContext;
        }
    }

    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseHttpErrorHandling(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<HttpExceptionHandlingMiddleware>();
        }
    }
}

