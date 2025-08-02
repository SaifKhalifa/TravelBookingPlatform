using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelBooking.API.DTOs;
using TravelBooking.Infrastructure.Persistence;

namespace TravelBooking.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HotelsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public HotelsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: /api/hotels
    [HttpGet]
    public async Task<IActionResult> GetHotels()
    {
        var hotels = await _context.Hotels
            .Include(h => h.City)
            .Select(h => new HotelDto
            {
                Id = h.Id,
                Name = h.Name,
                StarRate = h.StarRate,
                Location = h.Location,
                City = h.City!.Name
            })
            .ToListAsync();

        return Ok(hotels);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetHotel(int id)
    {
        var hotel = await _context.Hotels
            .Include(h => h.City)
            .Include(h => h.Rooms)
                .ThenInclude(r => r.RoomType)
            .Include(h => h.Rooms)
                .ThenInclude(r => r.Discount)
            .FirstOrDefaultAsync(h => h.Id == id);

        if (hotel == null)
            return NotFound("Hotel not found.");

        var response = new
        {
            hotel.Id,
            hotel.Name,
            hotel.StarRate,
            hotel.Location,
            City = hotel.City!.Name,
            Rooms = hotel.Rooms.Select(r => new RoomDto
            {
                Id = r.Id,
                RoomNumber = r.RoomNumber,
                Adults = r.Adults,
                Children = r.Children,
                PricePerNight = r.PricePerNight,
                IsAvailable = r.IsAvailable,
                RoomType = r.RoomType!.Name,
                Discount = r.Discount?.Name
            })
        };

        return Ok(response);
    }

    // GET: /api/hotels/{id}/rooms
    [HttpGet("{id}/rooms")]
    public async Task<IActionResult> GetRoomsForHotel(int id)
    {
        var rooms = await _context.Rooms
            .Where(r => r.HotelId == id && r.IsAvailable)
            .ToListAsync();

        return Ok(rooms);
    }
}
