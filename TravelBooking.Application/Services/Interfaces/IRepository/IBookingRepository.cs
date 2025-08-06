using TravelBooking.Domain.Entities;

namespace TravelBooking.Application.Services.Interfaces.IRepository;

public interface IBookingRepository
{
    Task<Room?> GetRoomWithDetailsAsync(int roomId);
    Task AddBookingAsync(Booking booking);
    Task AddPaymentAsync(Payment payment);
    Task SaveChangesAsync();

    Task<List<Booking>> GetUserBookingsAsync(int userId);
    Task<Booking?> GetBookingAsync(int userId, int bookingId);
    Task<Booking?> GetBookingWithRoomAsync(int userId, int bookingId);
}