using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UnictiveTest.Models;

namespace UnictiveTest.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Hobby> Hobbies => Set<Hobby>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasMany(u => u.Hobbies)
                .WithOne(h => h.User!)
                .HasForeignKey(h => h.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            var admin = new User
            {
                Id = 1,
                Name = "Admin",
                Email = "admin@local",
                PasswordHash = PasswordHasher.Hash("Admin123!")
            };
            modelBuilder.Entity<User>().HasData(admin);
        }
    }
}
