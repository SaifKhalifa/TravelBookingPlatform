using Microsoft.EntityFrameworkCore;
using TravelBooking.Domain.Entities;

namespace TravelBooking.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<City> Cities => Set<City>();
    public DbSet<Hotel> Hotels => Set<Hotel>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Booking> Bookings => Set<Booking>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // TO-DO: Fluent API configurations here
    }
}
