using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TravelBooking.Application.DTOs;
using TravelBooking.Application.Interfaces;
using TravelBooking.Application.Services.Interfaces;
using TravelBooking.Domain.Entities;

namespace TravelBooking.Application.Services;

public class BookingService : IBookingService
{
    private readonly IMapper _mapper;

    private readonly IBookingRepository _bookingRepo;

    public BookingService(IBookingRepository bookingRepo, IMapper mapper)
    {
        _bookingRepo = bookingRepo;
        _mapper = mapper;
    }

    public async Task<BookingDto> BookAsync(int userId, BookingCreateDto dto)
    {
        var room = await _bookingRepo.GetRoomWithDetailsAsync(dto.RoomId);

        if (room == null || !room.IsAvailable)
            throw new InvalidOperationException("Room not available");

        var nights = (dto.CheckOutDate - dto.CheckInDate).Days;
        if (nights <= 0)
            throw new ArgumentException("Check-out must be after check-in");

        var discountPercent = room.Discount?.Percentage ?? 0;
        var totalPrice = nights * room.PricePerNight * (1 - discountPercent / 100);

        var booking = new Booking
        {
            UserId = userId,
            RoomId = dto.RoomId,
            CheckInDate = dto.CheckInDate,
            CheckOutDate = dto.CheckOutDate,
            TotalPrice = totalPrice,
            Status = "Confirmed"
        };

        room.IsAvailable = false;

        await _bookingRepo.AddBookingAsync(booking);
        await _bookingRepo.SaveChangesAsync();

        var payment = new Payment
        {
            BookingId = booking.Id,
            Amount = totalPrice,
            Status = "Paid",
            Method = "Cash",
            TransactionId = $"TRX-{Guid.NewGuid().ToString().Substring(0, 8)}",
            CreatedAt = DateTime.UtcNow
        };

        await _bookingRepo.AddPaymentAsync(payment);
        await _bookingRepo.SaveChangesAsync();

        return _mapper.Map<BookingDto>(booking);
    }

    public async Task<List<BookingDto>> GetBookingHistoryAsync(int userId)
    {
        var bookings = await _bookingRepo.GetUserBookingsAsync(userId);

        return _mapper.Map<List<BookingDto>>(bookings);
    }

    public async Task<BookingDto?> GetBookingConfirmationAsync(int userId, int bookingId)
    {
        var booking = await _bookingRepo.GetBookingWithRoomAsync(userId, bookingId);

        return booking != null ? _mapper.Map<BookingDto>(booking) : null;
    }

    public async Task<BookingDto?> CancelBookingAsync(int userId, int bookingId)
    {
        var booking = await _bookingRepo.GetBookingAsync(userId, bookingId);

        if (booking == null || booking.Status == "Cancelled")
            return null;

        booking.Status = "Cancelled";
        booking.Room!.IsAvailable = true;

        await _bookingRepo.SaveChangesAsync();

        return _mapper.Map<BookingDto>(booking);
    }
}
