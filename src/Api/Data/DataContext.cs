using Microsoft.EntityFrameworkCore;
using UserService.Api.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace UserService.Api.Data
{
    public class DataContext : DbContext
    {
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<Subject> Subjects { get; set; } = null!;
        public DbSet<SubjectRelationship> SubjectsRelationships { get; set; } = null!;
        public DbSet<UserProgress> UsersProgress { get; set; } = null!;
        public DbSet<Career> Careers { get; set; } = null!;

        public DataContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
                v => v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
            modelBuilder.Entity<User>()
                .Property(u => u.UpdatedAt)
                .HasConversion(dateTimeConverter);
            modelBuilder.Entity<User>()
                .Property(u => u.DeletedAt)
                .HasConversion(dateTimeConverter);
        }
    }
}
