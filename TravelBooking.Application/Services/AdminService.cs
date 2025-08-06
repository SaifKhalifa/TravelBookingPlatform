using AutoMapper;
using TravelBooking.Application.DTOs;
using TravelBooking.Application.Interfaces;
using TravelBooking.Application.Services.Interfaces;
using TravelBooking.Domain.Entities;

namespace TravelBooking.Application.Services;

public class AdminService : IAdminService
{
    private readonly IAdminRepository _adminRepo;
    private readonly IMapper _mapper;

    public AdminService(IAdminRepository adminRepo, IMapper mapper)
    {
        _adminRepo = adminRepo;
        _mapper = mapper;
    }

    public async Task<Hotel> CreateHotelAsync(CreateHotelDto dto)
    {
        if (!await _adminRepo.CityExistsAsync(dto.CityId))
            throw new ArgumentException("Invalid CityId");

        var hotel = _mapper.Map<Hotel>(dto);
        await _adminRepo.AddHotelAsync(hotel);
        await _adminRepo.SaveChangesAsync();

        return hotel;
    }

    public async Task<Room> CreateRoomAsync(CreateRoomDto dto)
    {
        if (!await _adminRepo.HotelExistsAsync(dto.HotelId))
            throw new ArgumentException("Invalid HotelId");

        var room = _mapper.Map<Room>(dto);
        await _adminRepo.AddRoomAsync(room);
        await _adminRepo.SaveChangesAsync();

        return room;
    }

    public async Task<RoomType> CreateRoomTypeAsync(CreateRoomTypeDto dto)
    {
        var type = _mapper.Map<RoomType>(dto);
        await _adminRepo.AddRoomTypeAsync(type);
        await _adminRepo.SaveChangesAsync();
        return type;
    }

    public async Task<Discount> CreateDiscountAsync(CreateDiscountDto dto)
    {
        var discount = _mapper.Map<Discount>(dto);
        await _adminRepo.AddDiscountAsync(discount);
        await _adminRepo.SaveChangesAsync();
        return discount;
    }
}
