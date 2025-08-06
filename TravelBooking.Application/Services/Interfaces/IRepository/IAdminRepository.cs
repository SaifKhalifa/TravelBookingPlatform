using TravelBooking.Domain.Entities;

namespace TravelBooking.Application.Interfaces;

public interface IAdminRepository
{
    Task<bool> CityExistsAsync(int cityId);
    Task<bool> HotelExistsAsync(int hotelId);

    Task AddHotelAsync(Hotel hotel);
    Task AddRoomAsync(Room room);
    Task AddRoomTypeAsync(RoomType type);
    Task AddDiscountAsync(Discount discount);

    Task SaveChangesAsync();
}
