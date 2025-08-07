using Microsoft.EntityFrameworkCore;
using TravelBooking.Application.Services.Interfaces.IRepository;
using TravelBooking.Domain.Entities;
using TravelBooking.Infrastructure.Persistence;

namespace TravelBooking.Infrastructure.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly ApplicationDbContext _context;

    public BookingRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Room?> GetRoomWithDetailsAsync(int roomId) =>
        await _context.Rooms
            .Include(r => r.Hotel)
            .Include(r => r.Discount)
            .Include(r => r.RoomType)
            .FirstOrDefaultAsync(r => r.Id == roomId);

    public async Task AddBookingAsync(Booking booking) =>
        await _context.Bookings.AddAsync(booking);

    public async Task AddPaymentAsync(Payment payment) =>
        await _context.Payments.AddAsync(payment);

    public async Task SaveChangesAsync() =>
        await _context.SaveChangesAsync();

    public async Task<List<Booking>> GetUserBookingsAsync(int userId) =>
        await _context.Bookings
            .Where(b => b.UserId == userId)
            .Include(b => b.Room)!.ThenInclude(r => r.Hotel)
            .Include(b => b.Room)!.ThenInclude(r => r.RoomType)
            .Include(b => b.Room)!.ThenInclude(r => r.Discount)
            .OrderByDescending(b => b.CheckInDate)
            .ToListAsync();

    public async Task<Booking?> GetBookingAsync(int userId, int bookingId) =>
        await _context.Bookings
            .FirstOrDefaultAsync(b => b.Id == bookingId && b.UserId == userId);

    public async Task<Booking?> GetBookingWithRoomAsync(int userId, int bookingId) =>
        await _context.Bookings
            .Include(b => b.Room)!
                .ThenInclude(r => r.Hotel)
            .Include(b => b.Room)!
                .ThenInclude(r => r.RoomType)
            .Include(b => b.Room)!
                .ThenInclude(r => r.Discount)
            .FirstOrDefaultAsync(b => b.Id == bookingId && b.UserId == userId);
}
