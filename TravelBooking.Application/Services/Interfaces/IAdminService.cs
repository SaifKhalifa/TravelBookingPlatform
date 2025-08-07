using TravelBooking.Application.DTOs;
using TravelBooking.Domain.Entities;

namespace TravelBooking.Application.Services.Interfaces;

public interface IAdminService
{
    // CITIES
    Task<List<City>> GetCitiesAsync();
    Task CreateCityAsync(City city);
    Task<bool> UpdateCityAsync(int id, City city);
    Task<bool> DeleteCityAsync(int id);

    // HOTELS
    Task<Hotel> CreateHotelAsync(CreateHotelDto dto);
    Task<List<Hotel>> GetHotelsAsync();
    Task<bool> UpdateHotelAsync(int id, Hotel updated);
    Task<bool> DeleteHotelAsync(int id);

    // ROOMS
    Task<Room> CreateRoomAsync(CreateRoomDto dto);
    Task<List<Room>> GetRoomsAsync();
    Task<bool> UpdateRoomAsync(int id, Room updated);
    Task<bool> DeleteRoomAsync(int id);

    // ROOM TYPES
    Task<RoomType> CreateRoomTypeAsync(CreateRoomTypeDto dto);
    Task<List<RoomType>> GetRoomTypesAsync();
    Task<bool> DeleteRoomTypeAsync(int id);

    // DISCOUNTS
    Task<Discount> CreateDiscountAsync(CreateDiscountDto dto);
    Task<List<Discount>> GetDiscountsAsync();
    Task<bool> DeleteDiscountAsync(int id);

}