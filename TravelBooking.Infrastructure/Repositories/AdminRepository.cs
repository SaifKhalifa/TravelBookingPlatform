using Microsoft.EntityFrameworkCore;
using TravelBooking.Application.Services.Interfaces.IRepository;
using TravelBooking.Domain.Entities;
using TravelBooking.Infrastructure.Persistence;

namespace TravelBooking.Infrastructure.Repositories;

public class AdminRepository : IAdminRepository
{
    private readonly ApplicationDbContext _context;

    public AdminRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    // -------- Cities --------
    public async Task<List<City>> GetAllCitiesAsync() =>
        await _context.Cities.ToListAsync();

    public async Task<City?> GetCityByIdAsync(int id) =>
        await _context.Cities.FindAsync(id);

    public async Task AddCityAsync(City city) =>
        await _context.Cities.AddAsync(city);

    public void RemoveCity(City city) =>
        _context.Cities.Remove(city);

    // -------- Hotels --------
    public async Task<List<Hotel>> GetAllHotelsAsync() =>
        await _context.Hotels.ToListAsync();

    public async Task<Hotel?> GetHotelByIdAsync(int id) =>
        await _context.Hotels.FindAsync(id);

    public async Task<bool> CityExistsAsync(int cityId) =>
        await _context.Cities.AnyAsync(c => c.Id == cityId);

    public async Task AddHotelAsync(Hotel hotel) =>
        await _context.Hotels.AddAsync(hotel);

    public void RemoveHotel(Hotel hotel) =>
        _context.Hotels.Remove(hotel);

    // -------- Rooms --------
    public async Task<List<Room>> GetAllRoomsAsync() =>
        await _context.Rooms.ToListAsync();

    public async Task<Room?> GetRoomByIdAsync(int id) =>
        await _context.Rooms.FindAsync(id);

    public async Task<bool> HotelExistsAsync(int hotelId) =>
        await _context.Hotels.AnyAsync(h => h.Id == hotelId);

    public async Task AddRoomAsync(Room room) =>
        await _context.Rooms.AddAsync(room);

    public void RemoveRoom(Room room) =>
        _context.Rooms.Remove(room);

    // -------- Room Types --------
    public async Task<List<RoomType>> GetAllRoomTypesAsync() =>
        await _context.RoomTypes.ToListAsync();

    public async Task<RoomType?> GetRoomTypeByIdAsync(int id) =>
        await _context.RoomTypes.FindAsync(id);

    public async Task AddRoomTypeAsync(RoomType type) =>
        await _context.RoomTypes.AddAsync(type);

    public void RemoveRoomType(RoomType type) =>
        _context.RoomTypes.Remove(type);

    // -------- Discounts --------
    public async Task<List<Discount>> GetAllDiscountsAsync() =>
        await _context.Discounts.ToListAsync();

    public async Task<Discount?> GetDiscountByIdAsync(int id) =>
        await _context.Discounts.FindAsync(id);

    public async Task AddDiscountAsync(Discount discount) =>
        await _context.Discounts.AddAsync(discount);

    public void RemoveDiscount(Discount discount) =>
        _context.Discounts.Remove(discount);

    // -------- Save --------
    public async Task SaveChangesAsync() =>
        await _context.SaveChangesAsync();
}