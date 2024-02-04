using Microsoft.EntityFrameworkCore;
using TaskManager.Data.Entities;

namespace TaskManager.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Job> Jobs { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
