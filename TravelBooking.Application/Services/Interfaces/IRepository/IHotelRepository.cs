using TravelBooking.Domain.Entities;

namespace TravelBooking.Application.Services.Interfaces.IRepository;

public interface IHotelRepository
{
    Task<List<Hotel>> GetAllHotelsAsync(string? city = null, int? stars = null);
    Task<Hotel?> GetHotelWithRoomsAsync(int id);
}
