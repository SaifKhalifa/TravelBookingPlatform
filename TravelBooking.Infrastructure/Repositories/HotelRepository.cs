using Microsoft.EntityFrameworkCore;
using TravelBooking.Application.Services.Interfaces.IRepository;
using TravelBooking.Domain.Entities;
using TravelBooking.Infrastructure.Persistence;

namespace TravelBooking.Infrastructure.Repositories;

public class HotelRepository : IHotelRepository
{
    private readonly ApplicationDbContext _context;

    public HotelRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Hotel>> GetAllHotelsAsync(string? city = null, int? stars = null)
    {
        var query = _context.Hotels
            .Include(h => h.City)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(city))
            query = query.Where(h => h.City!.Name.ToLower().Contains(city.ToLower()));

        if (stars.HasValue)
            query = query.Where(h => h.StarRate == stars.Value);

        return await query.ToListAsync();
    }

    public async Task<Hotel?> GetHotelWithRoomsAsync(int id)
    {
        return await _context.Hotels
            .Include(h => h.City)
            .Include(h => h.Rooms)!.ThenInclude(r => r.RoomType)
            .Include(h => h.Rooms)!.ThenInclude(r => r.Discount)
            .FirstOrDefaultAsync(h => h.Id == id);
    }
}
