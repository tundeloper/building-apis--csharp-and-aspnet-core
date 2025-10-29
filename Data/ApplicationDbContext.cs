using Microsoft.EntityFrameworkCore;
using MyApiApp.Models.Entities;

namespace MyApiApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }

        // Example DbSet — add your entities here
        // public DbSet<Employee> Employees { get; set; }
    }
}
