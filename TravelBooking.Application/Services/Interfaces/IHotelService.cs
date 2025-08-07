using TravelBooking.Application.DTOs;

public interface IHotelService
{
    Task<List<HotelDto>> GetAllHotelsAsync(HotelQueryDto query);
    Task<HotelWithRoomsDto?> GetHotelByIdAsync(int id);
}