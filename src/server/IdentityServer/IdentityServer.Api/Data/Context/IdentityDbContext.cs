using IdentityServer.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Api.Data.Context
{
    public class IdentityDbContext(DbContextOptions<IdentityDbContext> options) : DbContext(options)
    {
        public DbSet<AppUser> Users { get; set; }
    }
}
