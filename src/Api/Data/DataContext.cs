using Microsoft.EntityFrameworkCore;
using UserService.Api.Models;

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
    }
}