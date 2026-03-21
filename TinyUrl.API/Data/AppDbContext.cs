namespace TinyUrl.API.Data
{
    using Microsoft.EntityFrameworkCore;
    using TinyUrl.API.Models;
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<TinyUrl> TinyUrls => Set<TinyUrl>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TinyUrl>()
                .HasIndex(x => x.ShortCode)
                .IsUnique();

            modelBuilder.Entity<TinyUrl>()
                .Property(x => x.OriginalUrl)
                .IsRequired()
                .HasMaxLength(2048);

            modelBuilder.Entity<TinyUrl>()
                .Property(x => x.ShortCode)
                .IsRequired()
                .HasMaxLength(6);
        }
    }
}
