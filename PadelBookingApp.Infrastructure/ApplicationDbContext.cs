using Microsoft.EntityFrameworkCore;
using PadelBookingApp.Domain.Entities;

namespace PadelBookingApp.Infrastructure
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }

        public DbSet<User> Users { get; set; }
        public DbSet<Booking> Bookings { get; set; }
    }
}