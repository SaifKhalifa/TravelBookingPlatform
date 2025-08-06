using TravelBooking.Application.DTOs;

namespace TravelBooking.Application.Services.Interfaces;

public interface IBookingService
{
    Task<BookingDto> BookAsync(int userId, BookingCreateDto dto);
    Task<List<BookingDto>> GetBookingHistoryAsync(int userId);
    Task<BookingDto?> GetBookingConfirmationAsync(int userId, int bookingId);
    Task<BookingDto?> CancelBookingAsync(int userId, int bookingId);
}