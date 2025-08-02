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
    public DbSet<RoomType> RoomTypes => Set<RoomType>();
    public DbSet<Discount> Discounts => Set<Discount>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Payment> Payments => Set<Payment>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Review>()
            .HasOne(r => r.DeletedByAdmin)
            .WithMany()
            .HasForeignKey(r => r.DeletedByAdminId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
