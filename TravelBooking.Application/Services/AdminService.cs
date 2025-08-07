using AutoMapper;
using TravelBooking.Application.DTOs;
using TravelBooking.Application.Services.Interfaces;
using TravelBooking.Application.Services.Interfaces.IRepository;
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

    // ---------------------- HOTEL CREATION ----------------------
    #region Hotel Management
    public async Task<Hotel> CreateHotelAsync(CreateHotelDto dto)
    {
        if (!await _adminRepo.CityExistsAsync(dto.CityId))
            throw new ArgumentException("Invalid CityId");

        var hotel = _mapper.Map<Hotel>(dto);
        await _adminRepo.AddHotelAsync(hotel);
        await _adminRepo.SaveChangesAsync();
        return hotel;
    }

    public async Task<bool> UpdateHotelAsync(int id, Hotel updated)
    {
        var hotel = await _adminRepo.GetHotelByIdAsync(id);
        if (hotel == null) return false;

        hotel.Name = updated.Name;
        hotel.Location = updated.Location;
        hotel.Owner = updated.Owner;
        hotel.StarRate = updated.StarRate;
        hotel.CityId = updated.CityId;

        await _adminRepo.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteHotelAsync(int id)
    {
        var hotel = await _adminRepo.GetHotelByIdAsync(id);
        if (hotel == null) return false;

        _adminRepo.RemoveHotel(hotel);
        await _adminRepo.SaveChangesAsync();
        return true;
    }

    public async Task<List<Hotel>> GetHotelsAsync() =>
        await _adminRepo.GetAllHotelsAsync();
    #endregion

    // ---------------------- ROOM MANAGEMENT ----------------------
    #region Room Management
    public async Task<Room> CreateRoomAsync(CreateRoomDto dto)
    {
        if (!await _adminRepo.HotelExistsAsync(dto.HotelId))
            throw new ArgumentException("Invalid HotelId");

        var room = _mapper.Map<Room>(dto);
        await _adminRepo.AddRoomAsync(room);
        await _adminRepo.SaveChangesAsync();
        return room;
    }

    public async Task<bool> UpdateRoomAsync(int id, Room updated)
    {
        var room = await _adminRepo.GetRoomByIdAsync(id);
        if (room == null) return false;

        room.RoomNumber = updated.RoomNumber;
        room.Adults = updated.Adults;
        room.Children = updated.Children;
        room.PricePerNight = updated.PricePerNight;
        room.IsAvailable = updated.IsAvailable;
        room.HotelId = updated.HotelId;
        room.RoomTypeId = updated.RoomTypeId;
        room.DiscountId = updated.DiscountId;

        await _adminRepo.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteRoomAsync(int id)
    {
        var room = await _adminRepo.GetRoomByIdAsync(id);
        if (room == null) return false;

        _adminRepo.RemoveRoom(room);
        await _adminRepo.SaveChangesAsync();
        return true;
    }

    public async Task<List<Room>> GetRoomsAsync() =>
        await _adminRepo.GetAllRoomsAsync();
    #endregion

    // ---------------------- ROOM TYPE MANAGEMENT ----------------------
    #region Room Type Management
    public async Task<RoomType> CreateRoomTypeAsync(CreateRoomTypeDto dto)
    {
        var type = _mapper.Map<RoomType>(dto);
        await _adminRepo.AddRoomTypeAsync(type);
        await _adminRepo.SaveChangesAsync();
        return type;
    }

    public async Task<List<RoomType>> GetRoomTypesAsync() =>
        await _adminRepo.GetAllRoomTypesAsync();

    public async Task<bool> DeleteRoomTypeAsync(int id)
    {
        var type = await _adminRepo.GetRoomTypeByIdAsync(id);
        if (type == null) return false;

        _adminRepo.RemoveRoomType(type);
        await _adminRepo.SaveChangesAsync();
        return true;
    }
    #endregion

    // ---------------------- DISCOUNTS ----------------------
    #region Discount Management
    public async Task<Discount> CreateDiscountAsync(CreateDiscountDto dto)
    {
        var discount = _mapper.Map<Discount>(dto);
        await _adminRepo.AddDiscountAsync(discount);
        await _adminRepo.SaveChangesAsync();
        return discount;
    }

    public async Task<List<Discount>> GetDiscountsAsync() =>
        await _adminRepo.GetAllDiscountsAsync();

    public async Task<bool> DeleteDiscountAsync(int id)
    {
        var discount = await _adminRepo.GetDiscountByIdAsync(id);
        if (discount == null) return false;

        _adminRepo.RemoveDiscount(discount);
        await _adminRepo.SaveChangesAsync();
        return true;
    }
    #endregion

    // ---------------------- CITY MANAGEMENT ----------------------
    #region City Management
    public async Task<List<City>> GetCitiesAsync() =>
        await _adminRepo.GetAllCitiesAsync();

    public async Task CreateCityAsync(City city)
    {
        await _adminRepo.AddCityAsync(city);
        await _adminRepo.SaveChangesAsync();
    }

    public async Task<bool> UpdateCityAsync(int id, City city)
    {
        var existing = await _adminRepo.GetCityByIdAsync(id);
        if (existing == null) return false;

        existing.Name = city.Name;
        existing.Country = city.Country;
        existing.PostOffice = city.PostOffice;

        await _adminRepo.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteCityAsync(int id)
    {
        var city = await _adminRepo.GetCityByIdAsync(id);
        if (city == null) return false;

        _adminRepo.RemoveCity(city);
        await _adminRepo.SaveChangesAsync();
        return true;
    }
    #endregion
}