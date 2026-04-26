using Microsoft.EntityFrameworkCore;
using AmazonBestSellersExplorer.WebAPI.Models;

namespace AmazonBestSellersExplorer.WebAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<FavoriteProduct> FavoriteProducts { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(eb =>
            {
                eb.HasKey(u => u.UserId);
                eb.Property(u => u.Login).IsRequired().HasMaxLength(50);
                eb.Property(u => u.PasswordHash).IsRequired();
                eb.HasMany(u => u.FavoriteProducts)
                  .WithOne(fp => fp.User)
                  .HasForeignKey(fp => fp.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<FavoriteProduct>(eb =>
            {
                eb.HasKey(fp => fp.FavoriteProductId);
                eb.Property(fp => fp.Title).IsRequired();
                eb.Property(fp => fp.Price);
                eb.Property(fp => fp.Asin).IsRequired().HasMaxLength(20);
            });
        }
    }
}


