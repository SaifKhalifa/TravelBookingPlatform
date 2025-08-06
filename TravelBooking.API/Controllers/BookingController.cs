using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TravelBooking.API.DTOs;
using TravelBooking.Domain.Entities;
using TravelBooking.Infrastructure.Persistence;

namespace TravelBooking.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BookingController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public BookingController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    // POST: /api/booking
    [HttpPost]
    public async Task<IActionResult> BookRoom([FromBody] BookingCreateDto request)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var room = await _context.Rooms
            .Include(r => r.Discount)
            .Include(r => r.RoomType)
            .Include(r => r.Hotel)
            .FirstOrDefaultAsync(r => r.Id == request.RoomId);

        if (room == null || !room.IsAvailable)
            return BadRequest("Room not available");

        var nights = (request.CheckOutDate - request.CheckInDate).Days;
        if (nights <= 0)
            return BadRequest("Check-out must be after check-in");

        var discountPercent = room.Discount?.Percentage ?? 0;
        var basePrice = nights * room.PricePerNight;
        var totalPrice = basePrice * (1 - (discountPercent / 100));

        var booking = new Booking
        {
            UserId = userId,
            RoomId = request.RoomId,
            CheckInDate = request.CheckInDate,
            CheckOutDate = request.CheckOutDate,
            TotalPrice = totalPrice,
            Status = "Confirmed"
        };

        room.IsAvailable = false;

        await _context.Bookings.AddAsync(booking);
        await _context.SaveChangesAsync();

        var payment = new Payment
        {
            BookingId = booking.Id,
            Amount = totalPrice,
            Status = "Paid",
            Method = "Cash",
            TransactionId = $"TRX-{Guid.NewGuid().ToString().Substring(0, 8)}",
            CreatedAt = DateTime.UtcNow
        };

        await _context.Payments.AddAsync(payment);
        await _context.SaveChangesAsync();
        
        var response = _mapper.Map<BookingDto>(booking);

        return Ok(response);
    }

    // GET: /api/booking/history
    [HttpGet("history")]
    public async Task<IActionResult> GetBookingHistory()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var bookings = await _context.Bookings
            .Where(b => b.UserId == userId)
            .Include(b => b.Room)!
                .ThenInclude(r => r.Hotel)
            .Include(b => b.Room)!
                .ThenInclude(r => r.RoomType)
            .Include(b => b.Room)!
                .ThenInclude(r => r.Discount)
            .OrderByDescending(b => b.CheckInDate)
            .ToListAsync();

       var response = _mapper.Map<List<BookingDto>>(bookings);
        return Ok(response);
    }

    // GET: /api/booking/confirmation/{id}
    [HttpGet("confirmation/{id}")]
    public async Task<IActionResult> GetBookingConfirmation(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var booking = await _context.Bookings
            .Include(b => b.Room)!
                .ThenInclude(r => r.Hotel)
            .Include(b => b.Room)!
                .ThenInclude(r => r.RoomType)
            .Include(b => b.Room)!
                .ThenInclude(r => r.Discount)
            .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

        if (booking == null)
            return NotFound("Booking not found");
        
        var response = _mapper.Map<BookingDto>(booking);

        return Ok(response);
    }

    // POST: /api/booking/cancel/{id}
    [HttpPost("cancel/{id}")]
    public async Task<IActionResult> CancelBooking(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var booking = await _context.Bookings
            .Include(b => b.Room)!
                .ThenInclude(r => r.Hotel)
            .Include(b => b.Room)!
                .ThenInclude(r => r.RoomType)
            .Include(b => b.Room)!
                .ThenInclude(r => r.Discount)
            .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

        if (booking == null)
            return NotFound("Booking not found.");

        if (booking.Status == "Cancelled")
            return BadRequest("Booking is already cancelled.");

        booking.Status = "Cancelled";
        booking.Room!.IsAvailable = true;

        await _context.SaveChangesAsync();
        
        var response = _mapper.Map<BookingDto>(booking);

        return Ok(response);
    }
}