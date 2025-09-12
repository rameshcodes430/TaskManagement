namespace TaskManagement.Api.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;
        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            // Log the incoming request
            _logger.LogInformation("Incoming Request: {Method} {Path} from {IpAddress}", context.Request.Method, context.Request.Path, context.Connection.RemoteIpAddress?.ToString());
            // Call the next middleware in the pipeline
            await _next(context);
            // Log the outgoing response
            _logger.LogInformation("Outgoing Response: Finished handling request. Status Code: {StatusCode}", context.Response.StatusCode);
        }
    }
}
