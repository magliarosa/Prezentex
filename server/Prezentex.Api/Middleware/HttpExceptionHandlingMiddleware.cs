using Prezentex.Api.Exceptions;

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
                httpContext = SetStatusCode(httpContext, ex);
            }
        }

        private static HttpContext SetStatusCode(HttpContext httpContext, Exception ex)
        {
            switch (ex)
            {
                case ArgumentException _:
                    httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                    httpContext.Response.WriteAsJsonAsync(new {errors = ex.Message});
                    break;
                case ResourceNotFoundException _:
                    httpContext.Response.StatusCode = StatusCodes.Status404NotFound; 
                    break;
                default:
                    httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    break;
            }

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

