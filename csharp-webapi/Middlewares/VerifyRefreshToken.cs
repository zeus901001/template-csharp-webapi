namespace csharp_webapi.Middlewares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class VerifyRefreshToken
    {
        private readonly RequestDelegate _next;

        public VerifyRefreshToken(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var refreshToken = httpContext.Request.Headers["x-refresh-token"].FirstOrDefault();
            // var expired = VerifyRefreshTokenExpired(refreshToken);
            // if (expired)

            await _next(httpContext);
        }

        // Verify refresh token is expired.
        private bool VerifyRefreshTokenExpired(string? token)
        {
            return false;
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class VerifyRefreshTokenExtensions
    {
        public static IApplicationBuilder UseVerifyRefreshToken(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<VerifyRefreshToken>();
        }
    }
}
