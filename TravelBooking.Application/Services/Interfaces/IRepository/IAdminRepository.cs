using TravelBooking.Domain.Entities;

namespace TravelBooking.Application.Services.Interfaces.IRepository;

public interface IAdminRepository
{
    // -------- Cities --------
    Task<List<City>> GetAllCitiesAsync();
    Task<City?> GetCityByIdAsync(int id);
    Task AddCityAsync(City city);
    void RemoveCity(City city);

    // -------- Hotels --------
    Task<List<Hotel>> GetAllHotelsAsync();
    Task<Hotel?> GetHotelByIdAsync(int id);
    Task<bool> CityExistsAsync(int cityId);
    Task AddHotelAsync(Hotel hotel);
    void RemoveHotel(Hotel hotel);

    // -------- Rooms --------
    Task<List<Room>> GetAllRoomsAsync();
    Task<Room?> GetRoomByIdAsync(int id);
    Task<bool> HotelExistsAsync(int hotelId);
    Task AddRoomAsync(Room room);
    void RemoveRoom(Room room);

    // -------- RoomTypes --------
    Task<List<RoomType>> GetAllRoomTypesAsync();
    Task<RoomType?> GetRoomTypeByIdAsync(int id);
    Task AddRoomTypeAsync(RoomType type);
    void RemoveRoomType(RoomType type);

    // -------- Discounts --------
    Task<List<Discount>> GetAllDiscountsAsync();
    Task<Discount?> GetDiscountByIdAsync(int id);
    Task AddDiscountAsync(Discount discount);
    void RemoveDiscount(Discount discount);

    // -------- Save --------
    Task SaveChangesAsync();
}
