using AutoMapper;
using TravelBooking.Application.DTOs;
using TravelBooking.Application.Services.Interfaces.IRepository;

namespace TravelBooking.Application.Services;

public class HotelService : IHotelService
{
    private readonly IHotelRepository _repo;
    private readonly IMapper _mapper;

    public HotelService(IHotelRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<List<HotelDto>> GetAllHotelsAsync(HotelQueryDto query)
    {
        var hotels = await _repo.GetAllHotelsAsync(query.City, query.Stars);

        return hotels.Select(h => new HotelDto
        {
            Id = h.Id,
            Name = h.Name,
            StarRate = h.StarRate,
            Location = h.Location,
            City = h.City!.Name
        }).ToList();
    }

    public async Task<HotelWithRoomsDto?> GetHotelByIdAsync(int id)
    {
        var hotel = await _repo.GetHotelWithRoomsAsync(id);
        if (hotel == null) return null;

        var dto = _mapper.Map<HotelWithRoomsDto>(hotel);
        dto.Rooms = hotel.Rooms.Select(r => _mapper.Map<RoomDto>(r)).ToList();
        return dto;
    }
}
