namespace Hangfire_SwaggerAuth.Middlewares
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.Request.Path.StartsWithSegments("/swagger") && !context.Request.Path.StartsWithSegments("/hangfire"))
            {
                if (!context.Request.Headers.TryGetValue("ApiKey", out var apiKey))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("API key not found");
                    return;
                }

                if (string.IsNullOrEmpty(apiKey.ToString()) || apiKey.ToString() != _configuration.GetSection("ApiKey").Value)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("API keys do not match");
                    return;
                }
            }

            await _next(context);
        }
    }
}
