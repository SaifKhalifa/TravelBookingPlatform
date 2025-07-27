using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TravelBooking.Domain.Entities;
using TravelBooking.Infrastructure.Persistence;

namespace TravelBooking.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BookingController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public BookingController(ApplicationDbContext context)
    {
        _context = context;
    }

    // POST: /api/booking
    [HttpPost]
    public async Task<IActionResult> BookRoom([FromBody] Booking request)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == request.RoomId);
        if (room == null || !room.IsAvailable)
            return BadRequest("Room not available");

        // Calculate number of nights
        var nights = (request.CheckOutDate - request.CheckInDate).Days;
        if (nights <= 0)
            return BadRequest("Check-out must be after check-in");

        var totalPrice = nights * room.PricePerNight;

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

        return Ok(new
        {
            booking.Id,
            Room = room.RoomNumber,
            Total = booking.TotalPrice,
            booking.CheckInDate,
            booking.CheckOutDate,
            Status = booking.Status
        });
    }

    // GET: /api/booking/history
    [HttpGet("history")]
    public async Task<IActionResult> GetBookingHistory()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var bookings = await _context.Bookings
            .Where(b => b.UserId == userId)
            .Include(b => b.Room)
                .ThenInclude(r => r.Hotel)
            .OrderByDescending(b => b.CheckInDate)
            .ToListAsync();

        return Ok(bookings.Select(b => new
        {
            b.Id,
            Hotel = b.Room!.Hotel!.Name,
            Room = b.Room.RoomNumber,
            b.CheckInDate,
            b.CheckOutDate,
            b.TotalPrice,
            b.Status
        }));
    }

    // GET: /api/booking/confirmation/{id}
    [HttpGet("confirmation/{id}")]
    public async Task<IActionResult> GetBookingConfirmation(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var booking = await _context.Bookings
            .Include(b => b.Room)
                .ThenInclude(r => r.Hotel)
            .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

        if (booking == null)
            return NotFound("Booking not found");

        return Ok(new
        {
            booking.Id,
            Hotel = booking.Room!.Hotel!.Name,
            Address = booking.Room.Hotel.Location,
            Room = booking.Room.RoomNumber,
            booking.CheckInDate,
            booking.CheckOutDate,
            booking.TotalPrice,
            booking.Status
        });
    }

    // POST: /api/booking/cancel/{id}
    [HttpPost("cancel/{id}")]
    public async Task<IActionResult> CancelBooking(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var booking = await _context.Bookings
            .Include(b => b.Room)
            .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

        if (booking == null)
            return NotFound("Booking not found.");

        if (booking.Status == "Cancelled")
            return BadRequest("Booking is already cancelled.");

        booking.Status = "Cancelled";
        booking.Room!.IsAvailable = true;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            Message = "Booking cancelled successfully.",
            booking.Id,
            booking.Status
        });
    }

}
