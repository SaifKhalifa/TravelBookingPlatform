using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            .ToListAsync();

        return Ok(hotels);
    }

    // GET: /api/hotels/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetHotel(int id)
    {
        var hotel = await _context.Hotels
            .Include(h => h.City)
            .Include(h => h.Rooms)
            .FirstOrDefaultAsync(h => h.Id == id);

        if (hotel == null)
            return NotFound("Hotel not found.");

        return Ok(hotel);
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
