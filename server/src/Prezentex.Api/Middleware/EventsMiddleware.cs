using Prezentex.Application.Common.Interfaces.Events;

namespace Prezentex.Api.Middleware
{
    public class EventsMiddleware
    {
        private readonly RequestDelegate _next;

        public EventsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        // IMessageWriter is injected into InvokeAsync
        public async Task InvokeAsync(HttpContext httpContext, IEventService eventService)
        {
            eventService.SubscribeEvents();
            await _next(httpContext);
        }
    }

    public static class EventsMiddlewareExtensions
    {
        public static IApplicationBuilder UseEvents(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<EventsMiddleware>();
        }
    }
}
