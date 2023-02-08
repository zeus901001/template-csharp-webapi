using Microsoft.EntityFrameworkCore;
using csharp_webapi.Entities;

namespace csharp_webapi.Config
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}