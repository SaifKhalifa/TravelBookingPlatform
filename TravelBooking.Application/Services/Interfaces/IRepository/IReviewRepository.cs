using TravelBooking.Domain.Entities;

namespace TravelBooking.Application.Services.Interfaces.IRepository;

public interface IReviewRepository
{
    Task<bool> HasReviewedAsync(int userId, int hotelId);
    Task<bool> HotelExistsAsync(int hotelId);
    
    Task<Hotel?> GetHotelByIdAsync(int hotelId);
    Task<User?> GetUserByIdAsync(int userId);

    Task AddReviewAsync(Review review);

    Task<List<Review>> GetReviewsByHotelAsync(int hotelId);
    Task<List<Review>> GetReviewsByUserAsync(int userId);
    Task<Review?> GetReviewByIdAsync(int reviewId);

    Task SaveChangesAsync();
}