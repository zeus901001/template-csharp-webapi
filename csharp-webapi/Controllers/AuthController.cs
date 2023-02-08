using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json.Nodes;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using System.Text;
using System.Security.Cryptography;
using csharp_webapi.Config;
using csharp_webapi.Entities;
using csharp_webapi.Helpers;
using csharp_webapi.Models.Auth;

namespace csharp_webapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : BaseController
    {
        private readonly IConfiguration _config;
        private readonly DataContext _context;

        public AuthController(IConfiguration config, DataContext context)
        {
            _config = config;
            _context = context;
        }

        // Login action.
        [HttpPost("login")]
        [AllowAnonymous]
        public ActionResult Login(LoginModel model)
        {
            var user = _context.Users.AsNoTracking().SingleOrDefault(u => u.Email == model.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(model?.Password, user.Password))
                return Ok(new { success = false, message = "Your credentials are incorrect." });

            var accessToken = GenerateAccessToken(user);
            var refreshToken = GenerateRefreshToken();

            var record = _context.RefreshTokens.AsNoTracking().SingleOrDefault(x => x.Email == user.Email);
            if (record == null)
            {
                _context.RefreshTokens.Add(new RefreshToken { Email = user.Email, Token = refreshToken, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow });
                _context.SaveChanges();
            }
            
            return Ok(new { success = true, accessToken, refreshToken });
        }

        // Register action.
        [HttpPost("register")]
        [AllowAnonymous]
        public ActionResult Register(RegisterModel model)
        {
            var user = _context.Users.AsNoTracking().SingleOrDefault(u => u.Email == model.Email);
            if (user != null)
                return Ok(new { success = false, message = "Your email is already in use." });

            user = new User
            {
                Email = model?.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(model?.Password),
                FirstName = model?.FirstName,
                LastName = model?.LastName,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            var accessToken = GenerateAccessToken(user);
            var refreshToken = GenerateRefreshToken();

            var record = _context.RefreshTokens.AsNoTracking().SingleOrDefault(x => x.Email == user.Email);
            if (record == null)
            {
                _context.RefreshTokens.Add(new RefreshToken { Email = user.Email, Token = refreshToken, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow });
                _context.SaveChanges();
            }

            return Ok(new { success = true, accessToken, refreshToken });
        }

        // Update access token for user from refresh token.
        [HttpPost("updateAccessToken")]
        [AllowAnonymous]
        public ActionResult UpdateAccessToken(RefreshTokenModel model)
        {
            if (model.RefreshToken.IsNullOrEmpty())
                return new JsonResult(new { message = "AccessForBidden" }) { StatusCode = StatusCodes.Status403Forbidden };

            var record = _context.RefreshTokens.AsNoTracking().SingleOrDefault(x => x.Token == model.RefreshToken);
            if (record == null)
                return new JsonResult(new { message = "AccessForBidden" }) { StatusCode = StatusCodes.Status403Forbidden };

            var user = _context.Users.AsNoTracking().SingleOrDefault(u => u.Email == record.Email);
            var accessToken = GenerateAccessToken(user!);

            return Ok(new { accessToken });
        }

        // Remove refresh token in token list.
        [HttpPost("removeRefreshToken")]
        [AllowAnonymous]
        public ActionResult RemoveRefreshToken(RefreshTokenModel model)
        {
            if (model.RefreshToken.IsNullOrEmpty())
                return new JsonResult(new { message = "AccessForBidden" }) { StatusCode = StatusCodes.Status403Forbidden };

            var record = _context.RefreshTokens.AsNoTracking().SingleOrDefault(x => x.Token == model.RefreshToken);
            if (record == null)
                return new JsonResult(new { message = "AccessForBidden" }) { StatusCode = StatusCodes.Status403Forbidden };

            _context.RefreshTokens.Remove(record);
            _context.SaveChanges();

            
            return Ok("Your refresh token is invalid.");
        }

        // Generate access token for user.
        private string GenerateAccessToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["Jwt:AccessTokenKey"]!);
            var options = new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("payload", JsonSerializer.Serialize(user, options), JsonClaimValueTypes.Json) }),
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(_config["Jwt:AccessTokenExpiredInMinutes"]!)),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        // Generate refresh token for user.
        private string GenerateRefreshToken()
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["Jwt:RefreshTokenKey"]!);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(Convert.ToBase64String(RandomNumberGenerator.GetBytes(64))),
                Expires = DateTime.UtcNow.AddDays(double.Parse(_config["Jwt:RefreshTokenExpiredInDays"]!)),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
