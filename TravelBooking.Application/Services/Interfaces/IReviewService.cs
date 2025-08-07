using TravelBooking.Application.DTOs;

namespace TravelBooking.Application.Services.Interfaces;

public interface IReviewService
{
    Task<List<ReviewDto>> GetReviewsByHotelAsync(int hotelId);
    Task<List<ReviewDto>> GetReviewsByUserAsync(int userId);
    Task LeaveReviewAsync(int userId, ReviewCreateDto dto);
    Task<bool> DeleteReviewAsync(int reviewId, int userId);
    Task<bool> AdminDeleteReviewAsync(int reviewId, int adminId);
}