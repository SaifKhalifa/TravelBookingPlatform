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

    public async Task<bool> CityExistsAsync(int cityId) =>
        await _context.Cities.AnyAsync(c => c.Id == cityId);

    public async Task<bool> HotelExistsAsync(int hotelId) =>
        await _context.Hotels.AnyAsync(h => h.Id == hotelId);

    public async Task AddHotelAsync(Hotel hotel) =>
        await _context.Hotels.AddAsync(hotel);

    public async Task AddRoomAsync(Room room) =>
        await _context.Rooms.AddAsync(room);

    public async Task AddRoomTypeAsync(RoomType type) =>
        await _context.RoomTypes.AddAsync(type);

    public async Task AddDiscountAsync(Discount discount) =>
        await _context.Discounts.AddAsync(discount);

    public async Task SaveChangesAsync() =>
        await _context.SaveChangesAsync();
}
