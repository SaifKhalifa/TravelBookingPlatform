using TravelBooking.Application.DTOs;
using TravelBooking.Domain.Entities;

namespace TravelBooking.Application.Services.Interfaces;

public interface IAdminService
{
    Task<Hotel> CreateHotelAsync(CreateHotelDto dto);
    Task<Room> CreateRoomAsync(CreateRoomDto dto);
    Task<RoomType> CreateRoomTypeAsync(CreateRoomTypeDto dto);
    Task<Discount> CreateDiscountAsync(CreateDiscountDto dto);
}
