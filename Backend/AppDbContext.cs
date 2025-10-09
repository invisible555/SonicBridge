using Backend.Entity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Backend
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<User> User { get; set; } = null!;
        public DbSet<UserRefreshToken> UserRefreshToken { get; set; }
        public DbSet<UserAccessToken> UserAccessToken { get; set; }
        public DbSet<UserFile> UserFile { get; set; }
        public DbSet<TranscriptionTasks> TranscriptionTasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relacja jeden-do-wielu: User -> UserFile
            modelBuilder.Entity<UserFile>()
                .HasOne(f => f.User)
                .WithMany(u => u.Files)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Opcjonalnie: ustawienia dla innych encji
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Login)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}

