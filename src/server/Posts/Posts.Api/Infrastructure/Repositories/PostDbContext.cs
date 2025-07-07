using BuildingBlocks.Models;
using Microsoft.EntityFrameworkCore;
using Posts.Api.Core.Domain.Entities;

namespace Posts.Api.Infrastructure.Repositories
{
    public class PostDbContext(DbContextOptions<PostDbContext> options) : DbContext(options)
    {

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ChangeTracker.Entries()
                .Where(e => (e.State == EntityState.Added || e.State == EntityState.Modified) && e.Entity is BaseEntity baseEntity)
                .ToList()
                .ForEach(e =>
                {
                    if (e.Entity is BaseEntity baseEntity)
                    {
                        if (e.State == EntityState.Added)
                            baseEntity.CreateDate = DateTime.UtcNow;
                        else if (e.State == EntityState.Modified)
                            baseEntity.ModifyDate = DateTime.UtcNow;
                    }
                });
            return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            ChangeTracker.Entries()
              .Where(e => (e.State == EntityState.Added || e.State == EntityState.Modified) && e.Entity is BaseEntity baseEntity)
              .ToList()
              .ForEach(e =>
              {
                  if (e.Entity is BaseEntity baseEntity)
                  {
                      if (e.State == EntityState.Added)
                          baseEntity.CreateDate = DateTime.UtcNow;
                      else if (e.State == EntityState.Modified)
                          baseEntity.ModifyDate = DateTime.UtcNow;
                  }
              });
            return base.SaveChanges();
        }

        public DbSet<Post> Posts { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Follower> Followers { get; set; }
    }
}
