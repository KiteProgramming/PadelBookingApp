using Microsoft.EntityFrameworkCore;
using PadelBookingApp.Domain.Entities;  // Ensure this points to your Entities folder

namespace PadelBookingApp.Infrastructure
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }

        public DbSet<User> Users { get; set; }
    }
}