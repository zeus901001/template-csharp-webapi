using csharp_webapi.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;

namespace csharp_webapi.Middlewares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class VerifyAccessToken
    {
        private readonly RequestDelegate _next;

        public VerifyAccessToken(RequestDelegate next, IConfiguration config)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var accessToken = httpContext.Request.Headers["x-access-token"].FirstOrDefault();
            var user = VerifyAccessTokenExpired(accessToken);
            if (user != null)
                httpContext.Items["User"] = user;

            await _next(httpContext);
        }

        // Verify access token is expired.
        private User? VerifyAccessTokenExpired(string? token)
        {
            if (token == null)
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;            
                if (jwtToken!.ValidTo < DateTime.UtcNow)
                    return null;
                
                var payload = jwtToken!.Claims.First(x => x.Type == "payload").Value;
                var user = JsonSerializer.Deserialize<User>(payload, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

                return user;
            }
            catch
            {
                return null;
            }
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class VerifyAccessTokenExtensions
    {
        public static IApplicationBuilder UseVerifyAccessToken(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<VerifyAccessToken>();
        }
    }
}
