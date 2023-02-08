using csharp_webapi.Config;
using csharp_webapi.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace csharp_webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : BaseController
    {
        private readonly DataContext _context;

        public AccountController(DataContext context)
        {
            _context = context;
        }

        [HttpGet("users")]
        public async Task<ActionResult<User>> Users()
        {
            var users = await _context.Users.AsNoTracking().ToListAsync();
            return Ok(users);
        }
    }
}
