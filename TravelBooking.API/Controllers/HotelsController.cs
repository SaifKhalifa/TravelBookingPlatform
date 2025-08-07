using Microsoft.AspNetCore.Mvc;
using TravelBooking.Application.DTOs;
using TravelBooking.Application.Services.Interfaces;

namespace TravelBooking.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HotelsController : ControllerBase
{
    private readonly IHotelService _hotelService;

    public HotelsController(IHotelService hotelService)
    {
        _hotelService = hotelService;
    }

    // GET: /api/hotels?city=paris&stars=5
    [HttpGet]
    public async Task<IActionResult> GetHotels([FromQuery] string? city, [FromQuery] int? stars)
    {
        var filter = new HotelQueryDto { City = city, Stars = stars };
        var hotels = await _hotelService.GetAllHotelsAsync(filter);
        return Ok(hotels);
    }

    // GET: /api/hotels/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetHotel(int id)
    {
        var hotel = await _hotelService.GetHotelByIdAsync(id);
        return hotel != null
            ? Ok(hotel)
            : NotFound("Hotel not found.");
    }

    // GET: /api/hotels/{id}/rooms
    [HttpGet("{id}/rooms")]
    public async Task<IActionResult> GetHotelRooms(int id)
    {
        var hotel = await _hotelService.GetHotelByIdAsync(id);
        if (hotel == null)
            return NotFound("Hotel not found.");

        var availableRooms = hotel.Rooms.Where(r => r.IsAvailable).ToList();
        return Ok(availableRooms);
    }
}