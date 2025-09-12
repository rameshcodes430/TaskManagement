namespace TaskManagement.Api.Middleware
{
    public class RootRedirectMiddleware
    {
        private readonly RequestDelegate _next;
        public RootRedirectMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path == "/" && context.Request.Method == "GET")
            {
                context.Response.Redirect("/swagger");
                return;
            }
            await _next(context);
        }
    }
}
