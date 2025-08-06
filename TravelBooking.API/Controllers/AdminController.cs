using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelBooking.Application.DTOs;
using TravelBooking.Domain.Entities;
using TravelBooking.Infrastructure.Persistence;

namespace TravelBooking.API.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public AdminController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    // ========== CITIES CRUD ==========
    #region Cities CRUD
    // GET: /api/admin/cities
    [HttpGet("cities")]
    public async Task<IActionResult> GetCities()
    {
        var cities = await _context.Cities.ToListAsync();
        return Ok(cities);
    }

    // POST: /api/admin/cities
    [HttpPost("cities")]
    public async Task<IActionResult> CreateCity([FromBody] City city)
    {
        await _context.Cities.AddAsync(city);
        await _context.SaveChangesAsync();
        return Ok(city);
    }

    // PUT: /api/admin/cities/{id}
    [HttpPut("cities/{id}")]
    public async Task<IActionResult> UpdateCity(int id, [FromBody] City updatedCity)
    {
        var city = await _context.Cities.FindAsync(id);
        if (city == null) return NotFound("City not found");

        city.Name = updatedCity.Name;
        city.Country = updatedCity.Country;
        city.PostOffice = updatedCity.PostOffice;

        await _context.SaveChangesAsync();
        return Ok(city);
    }

    // DELETE: /api/admin/cities/{id}
    [HttpDelete("cities/{id}")]
    public async Task<IActionResult> DeleteCity(int id)
    {
        var city = await _context.Cities.FindAsync(id);
        if (city == null) return NotFound("City not found");

        _context.Cities.Remove(city);
        await _context.SaveChangesAsync();
        return Ok("City deleted");
    }
    #endregion

    // ========== HOTELS CRUD ==========
    #region Hotels CRUD

    // GET: /api/admin/hotels
    [HttpGet("hotels")]
    public async Task<IActionResult> GetHotels()
    {
        var hotels = await _context.Hotels.Include(h => h.City).ToListAsync();
        return Ok(hotels);
    }

    // POST: /api/admin/hotels
    [HttpPost("hotels")]
    public async Task<IActionResult> CreateHotel([FromBody] CreateHotelDto dto)
    {
        var city = await _context.Cities.FindAsync(dto.CityId);
        if (city == null) return BadRequest("Invalid CityId");        

        var hotel = _mapper.Map<Hotel>(dto);

        await _context.Hotels.AddAsync(hotel);
        await _context.SaveChangesAsync();

        return Ok(new { hotel.Id, hotel.Name });
    }


    // PUT: /api/admin/hotels/{id}
    [HttpPut("hotels/{id}")]
    public async Task<IActionResult> UpdateHotel(int id, [FromBody] Hotel updatedHotel)
    {
        var hotel = await _context.Hotels.FindAsync(id);
        if (hotel == null) return NotFound("Hotel not found");

        hotel.Name = updatedHotel.Name;
        hotel.Owner = updatedHotel.Owner;
        hotel.StarRate = updatedHotel.StarRate;
        hotel.Location = updatedHotel.Location;
        hotel.CityId = updatedHotel.CityId;

        await _context.SaveChangesAsync();
        return Ok(hotel);
    }

    // DELETE: /api/admin/hotels/{id}
    [HttpDelete("hotels/{id}")]
    public async Task<IActionResult> DeleteHotel(int id)
    {
        var hotel = await _context.Hotels.FindAsync(id);
        if (hotel == null) return NotFound("Hotel not found");

        _context.Hotels.Remove(hotel);
        await _context.SaveChangesAsync();
        return Ok("Hotel deleted");
    }
    #endregion

    // ========== ROOMS CRUD ==========
    #region Rooms CRUD

    // GET: /api/admin/rooms
    [HttpGet("rooms")]
    public async Task<IActionResult> GetRooms()
    {
        var rooms = await _context.Rooms.Include(r => r.Hotel).ToListAsync();
        return Ok(rooms);
    }

    // POST: /api/admin/rooms
    [HttpPost("rooms")]
    public async Task<IActionResult> CreateRoom([FromBody] CreateRoomDto dto)
    {
        var hotel = await _context.Hotels.FindAsync(dto.HotelId);
        if (hotel == null) return BadRequest("Invalid HotelId");

        var room = _mapper.Map<Room>(dto);

        await _context.Rooms.AddAsync(room);
        await _context.SaveChangesAsync();

        return Ok(new { room.Id, room.RoomNumber });
    }


    // PUT: /api/admin/rooms/{id}
    [HttpPut("rooms/{id}")]
    public async Task<IActionResult> UpdateRoom(int id, [FromBody] Room updatedRoom)
    {
        var room = await _context.Rooms.FindAsync(id);
        if (room == null) return NotFound("Room not found");

        room.RoomNumber = updatedRoom.RoomNumber;
        room.Adults = updatedRoom.Adults;
        room.Children = updatedRoom.Children;
        room.PricePerNight = updatedRoom.PricePerNight;
        room.IsAvailable = updatedRoom.IsAvailable;
        room.HotelId = updatedRoom.HotelId;

        await _context.SaveChangesAsync();
        return Ok(room);
    }

    // DELETE: /api/admin/rooms/{id}
    [HttpDelete("rooms/{id}")]
    public async Task<IActionResult> DeleteRoom(int id)
    {
        var room = await _context.Rooms.FindAsync(id);
        if (room == null) return NotFound("Room not found");

        _context.Rooms.Remove(room);
        await _context.SaveChangesAsync();
        return Ok("Room deleted");
    }
    #endregion

    // ========== ROOM TYPES CRUD ==========
    #region Room Types CRUD
    // GET: /api/admin/roomtypes
    [HttpGet("roomtypes")]
    public async Task<IActionResult> GetRoomTypes()
    {
        var types = await _context.RoomTypes.ToListAsync();
        return Ok(types);
    }

    // POST: /api/admin/roomtypes
    [HttpPost("roomtypes")]
    public async Task<IActionResult> CreateRoomType([FromBody] CreateRoomTypeDto dto)
    {
        var type = _mapper.Map<RoomType>(dto);

        await _context.RoomTypes.AddAsync(type);
        await _context.SaveChangesAsync();

        return Ok(new { type.Id, type.Name });
    }

    // DELETE: /api/admin/roomtypes/{id}
    [HttpDelete("roomtypes/{id}")]
    public async Task<IActionResult> DeleteRoomType(int id)
    {
        var type = await _context.RoomTypes.FindAsync(id);
        if (type == null) return NotFound("Room type not found");
        _context.RoomTypes.Remove(type);
        await _context.SaveChangesAsync();
        return Ok("Room type deleted");
    }
    #endregion
  
    // ========== DISCOUNTS CRUD ==========
    #region Discounts CRUD
    // GET: /api/admin/discounts
    [HttpGet("discounts")]
    public async Task<IActionResult> GetDiscounts()
    {
        var discounts = await _context.Discounts.ToListAsync();
        return Ok(discounts);
    }

    // POST: /api/admin/discounts
    [HttpPost("discounts")]
    public async Task<IActionResult> CreateDiscount([FromBody] CreateDiscountDto dto)
    {
        var discount = _mapper.Map<Discount>(dto);

        await _context.Discounts.AddAsync(discount);
        await _context.SaveChangesAsync();

        return Ok(new { discount.Id, discount.Name });
    }

    // DELETE: /api/admin/discounts/{id}
    [HttpDelete("discounts/{id}")]
    public async Task<IActionResult> DeleteDiscount(int id)
    {
        var discount = await _context.Discounts.FindAsync(id);
        if (discount == null) return NotFound("Discount not found");
        _context.Discounts.Remove(discount);
        await _context.SaveChangesAsync();
        return Ok("Discount deleted");
    }
    #endregion
}
