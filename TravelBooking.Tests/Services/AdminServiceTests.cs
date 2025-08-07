using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Xunit;
using AutoMapper;
using TravelBooking.Application.Services;
using TravelBooking.Application.DTOs;
using TravelBooking.Domain.Entities;
using TravelBooking.Application.Services.Interfaces.IRepository;

namespace TravelBooking.Tests.Services;

public class AdminServiceTests
{
    private readonly Mock<IAdminRepository> _mockRepo;
    private readonly Mock<IMapper> _mockMapper;
    private readonly AdminService _service;

    public AdminServiceTests()
    {
        _mockRepo = new Mock<IAdminRepository>();
        _mockMapper = new Mock<IMapper>();
        _service = new AdminService(_mockRepo.Object, _mockMapper.Object);
    }

    #region City Management

    [Fact]
    public async Task GetCitiesAsync_ShouldReturnCities()
    {
        // Arrange
        var cities = new List<City> { new City { Id = 1, Name = "Nablus" } };
        _mockRepo.Setup(r => r.GetAllCitiesAsync()).ReturnsAsync(cities);

        // Act
        var result = await _service.GetCitiesAsync();

        // Assert
        Assert.Single(result);
        Assert.Equal("Nablus", result[0].Name);
    }

    [Fact]
    public async Task CreateCityAsync_ShouldCallRepo()
    {
        // Arrange
        var city = new City { Name = "Jenin" };
        _mockRepo.Setup(r => r.AddCityAsync(city)).Returns(Task.CompletedTask);
        _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        await _service.CreateCityAsync(city);

        // Assert
        _mockRepo.Verify(r => r.AddCityAsync(city), Times.Once);
        _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateCityAsync_ShouldReturnFalse_WhenCityNotFound()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetCityByIdAsync(99)).ReturnsAsync((City?)null);

        // Act
        var result = await _service.UpdateCityAsync(99, new City());

        // Assert
        Assert.False(result);
        _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateCityAsync_ShouldUpdateAndSave_WhenCityFound()
    {
        // Arrange
        var city = new City { Id = 1, Name = "Old", Country = "A", PostOffice = "X" };
        var updated = new City { Name = "New", Country = "B", PostOffice = "Y" };
        _mockRepo.Setup(r => r.GetCityByIdAsync(1)).ReturnsAsync(city);
        _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _service.UpdateCityAsync(1, updated);

        // Assert
        Assert.True(result);
        Assert.Equal("New", city.Name);
        Assert.Equal("B", city.Country);
        Assert.Equal("Y", city.PostOffice);
        _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteCityAsync_ShouldReturnFalse_WhenCityNotFound()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetCityByIdAsync(99)).ReturnsAsync((City?)null);

        // Act
        var result = await _service.DeleteCityAsync(99);

        // Assert
        Assert.False(result);
        _mockRepo.Verify(r => r.RemoveCity(It.IsAny<City>()), Times.Never);
    }

    [Fact]
    public async Task DeleteCityAsync_ShouldRemoveAndSave_WhenCityFound()
    {
        // Arrange
        var city = new City { Id = 2, Name = "ToDelete" };
        _mockRepo.Setup(r => r.GetCityByIdAsync(2)).ReturnsAsync(city);
        _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _service.DeleteCityAsync(2);

        // Assert
        Assert.True(result);
        _mockRepo.Verify(r => r.RemoveCity(city), Times.Once);
        _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    #endregion

    #region Hotel Management

    [Fact]
    public async Task CreateHotelAsync_ShouldThrow_WhenCityNotExists()
    {
        // Arrange
        var dto = new CreateHotelDto { CityId = 5 };
        _mockRepo.Setup(r => r.CityExistsAsync(dto.CityId)).ReturnsAsync(false);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateHotelAsync(dto));
        Assert.Equal("Invalid CityId", ex.Message);
    }

    [Fact]
    public async Task CreateHotelAsync_ShouldMapAddAndReturnHotel_WhenCityExists()
    {
        // Arrange
        var dto = new CreateHotelDto { CityId = 5 };
        var hotel = new Hotel { Id = 10, CityId = 5 };
        _mockRepo.Setup(r => r.CityExistsAsync(dto.CityId)).ReturnsAsync(true);
        _mockMapper.Setup(m => m.Map<Hotel>(dto)).Returns(hotel);
        _mockRepo.Setup(r => r.AddHotelAsync(hotel)).Returns(Task.CompletedTask);
        _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _service.CreateHotelAsync(dto);

        // Assert
        Assert.Equal(hotel, result);
        _mockRepo.Verify(r => r.AddHotelAsync(hotel), Times.Once);
        _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateHotelAsync_ShouldReturnFalse_WhenHotelNotFound()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetHotelByIdAsync(100)).ReturnsAsync((Hotel?)null);

        // Act
        var result = await _service.UpdateHotelAsync(100, new Hotel());

        // Assert
        Assert.False(result);
        _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateHotelAsync_ShouldUpdateAndSave_WhenHotelFound()
    {
        // Arrange
        var hotel = new Hotel { Id = 1, Name = "Old", Location = "Loc", Owner = "A", StarRate = 3, CityId = 1 };
        var updated = new Hotel { Name = "New", Location = "NewLoc", Owner = "B", StarRate = 4, CityId = 2 };
        _mockRepo.Setup(r => r.GetHotelByIdAsync(1)).ReturnsAsync(hotel);
        _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _service.UpdateHotelAsync(1, updated);

        // Assert
        Assert.True(result);
        Assert.Equal("New", hotel.Name);
        Assert.Equal("NewLoc", hotel.Location);
        Assert.Equal("B", hotel.Owner);
        Assert.Equal(4, hotel.StarRate);
        Assert.Equal(2, hotel.CityId);
        _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteHotelAsync_ShouldReturnFalse_WhenHotelNotFound()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetHotelByIdAsync(404)).ReturnsAsync((Hotel?)null);

        // Act
        var result = await _service.DeleteHotelAsync(404);

        // Assert
        Assert.False(result);
        _mockRepo.Verify(r => r.RemoveHotel(It.IsAny<Hotel>()), Times.Never);
    }

    [Fact]
    public async Task DeleteHotelAsync_ShouldRemoveAndSave_WhenHotelFound()
    {
        // Arrange
        var hotel = new Hotel { Id = 8, Name = "TestDelete" };
        _mockRepo.Setup(r => r.GetHotelByIdAsync(8)).ReturnsAsync(hotel);
        _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _service.DeleteHotelAsync(8);

        // Assert
        Assert.True(result);
        _mockRepo.Verify(r => r.RemoveHotel(hotel), Times.Once);
        _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetHotelsAsync_ShouldReturnHotels()
    {
        // Arrange
        var hotels = new List<Hotel> { new Hotel { Id = 1, Name = "Royal" } };
        _mockRepo.Setup(r => r.GetAllHotelsAsync()).ReturnsAsync(hotels);

        // Act
        var result = await _service.GetHotelsAsync();

        // Assert
        Assert.Single(result);
        Assert.Equal("Royal", result[0].Name);
    }

    #endregion

    #region Room Management

    [Fact]
    public async Task CreateRoomAsync_ShouldThrow_WhenHotelNotExists()
    {
        // Arrange
        var dto = new CreateRoomDto { HotelId = 9 };
        _mockRepo.Setup(r => r.HotelExistsAsync(dto.HotelId)).ReturnsAsync(false);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateRoomAsync(dto));
        Assert.Equal("Invalid HotelId", ex.Message);
    }

    [Fact]
    public async Task CreateRoomAsync_ShouldMapAddAndReturnRoom_WhenHotelExists()
    {
        // Arrange
        var dto = new CreateRoomDto { HotelId = 7 };
        var room = new Room { Id = 5, HotelId = 7 };
        _mockRepo.Setup(r => r.HotelExistsAsync(dto.HotelId)).ReturnsAsync(true);
        _mockMapper.Setup(m => m.Map<Room>(dto)).Returns(room);
        _mockRepo.Setup(r => r.AddRoomAsync(room)).Returns(Task.CompletedTask);
        _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _service.CreateRoomAsync(dto);

        // Assert
        Assert.Equal(room, result);
        _mockRepo.Verify(r => r.AddRoomAsync(room), Times.Once);
        _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateRoomAsync_ShouldReturnFalse_WhenRoomNotFound()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetRoomByIdAsync(99)).ReturnsAsync((Room?)null);

        // Act
        var result = await _service.UpdateRoomAsync(99, new Room());

        // Assert
        Assert.False(result);
        _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateRoomAsync_ShouldUpdateAndSave_WhenRoomFound()
    {
        // Arrange
        var room = new Room { Id = 1, RoomNumber = "101", Adults = 2, Children = 0, PricePerNight = 50, IsAvailable = true, HotelId = 1, RoomTypeId = 1, DiscountId = null };
        var updated = new Room { RoomNumber = "201", Adults = 3, Children = 2, PricePerNight = 80, IsAvailable = false, HotelId = 2, RoomTypeId = 2, DiscountId = 1 };
        _mockRepo.Setup(r => r.GetRoomByIdAsync(1)).ReturnsAsync(room);
        _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _service.UpdateRoomAsync(1, updated);

        // Assert
        Assert.True(result);
        Assert.Equal("201", room.RoomNumber);
        Assert.Equal(3, room.Adults);
        Assert.Equal(2, room.Children);
        Assert.Equal(80, room.PricePerNight);
        Assert.False(room.IsAvailable);
        Assert.Equal(2, room.HotelId);
        Assert.Equal(2, room.RoomTypeId);
        Assert.Equal(1, room.DiscountId);
        _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteRoomAsync_ShouldReturnFalse_WhenRoomNotFound()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetRoomByIdAsync(100)).ReturnsAsync((Room?)null);

        // Act
        var result = await _service.DeleteRoomAsync(100);

        // Assert
        Assert.False(result);
        _mockRepo.Verify(r => r.RemoveRoom(It.IsAny<Room>()), Times.Never);
    }

    [Fact]
    public async Task DeleteRoomAsync_ShouldRemoveAndSave_WhenRoomFound()
    {
        // Arrange
        var room = new Room { Id = 5, RoomNumber = "105" };
        _mockRepo.Setup(r => r.GetRoomByIdAsync(5)).ReturnsAsync(room);
        _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _service.DeleteRoomAsync(5);

        // Assert
        Assert.True(result);
        _mockRepo.Verify(r => r.RemoveRoom(room), Times.Once);
        _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetRoomsAsync_ShouldReturnRooms()
    {
        // Arrange
        var rooms = new List<Room> { new Room { Id = 1, RoomNumber = "101" } };
        _mockRepo.Setup(r => r.GetAllRoomsAsync()).ReturnsAsync(rooms);

        // Act
        var result = await _service.GetRoomsAsync();

        // Assert
        Assert.Single(result);
        Assert.Equal("101", result[0].RoomNumber);
    }

    #endregion

    #region RoomType Management

    [Fact]
    public async Task CreateRoomTypeAsync_ShouldMapAddAndReturnType()
    {
        // Arrange
        var dto = new CreateRoomTypeDto { Name = "Suite" };
        var type = new RoomType { Id = 2, Name = "Suite" };
        _mockMapper.Setup(m => m.Map<RoomType>(dto)).Returns(type);
        _mockRepo.Setup(r => r.AddRoomTypeAsync(type)).Returns(Task.CompletedTask);
        _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _service.CreateRoomTypeAsync(dto);

        // Assert
        Assert.Equal(type, result);
        _mockRepo.Verify(r => r.AddRoomTypeAsync(type), Times.Once);
        _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetRoomTypesAsync_ShouldReturnRoomTypes()
    {
        // Arrange
        var types = new List<RoomType> { new RoomType { Id = 1, Name = "Single" } };
        _mockRepo.Setup(r => r.GetAllRoomTypesAsync()).ReturnsAsync(types);

        // Act
        var result = await _service.GetRoomTypesAsync();

        // Assert
        Assert.Single(result);
        Assert.Equal("Single", result[0].Name);
    }

    [Fact]
    public async Task DeleteRoomTypeAsync_ShouldReturnFalse_WhenTypeNotFound()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetRoomTypeByIdAsync(99)).ReturnsAsync((RoomType?)null);

        // Act
        var result = await _service.DeleteRoomTypeAsync(99);

        // Assert
        Assert.False(result);
        _mockRepo.Verify(r => r.RemoveRoomType(It.IsAny<RoomType>()), Times.Never);
    }

    [Fact]
    public async Task DeleteRoomTypeAsync_ShouldRemoveAndSave_WhenTypeFound()
    {
        // Arrange
        var type = new RoomType { Id = 2, Name = "ToDelete" };
        _mockRepo.Setup(r => r.GetRoomTypeByIdAsync(2)).ReturnsAsync(type);
        _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _service.DeleteRoomTypeAsync(2);

        // Assert
        Assert.True(result);
        _mockRepo.Verify(r => r.RemoveRoomType(type), Times.Once);
        _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    #endregion

    #region Discount Management

    [Fact]
    public async Task CreateDiscountAsync_ShouldMapAddAndReturnDiscount()
    {
        // Arrange
        var dto = new CreateDiscountDto { Percentage = 10 };
        var discount = new Discount { Id = 3, Percentage = 10 };
        _mockMapper.Setup(m => m.Map<Discount>(dto)).Returns(discount);
        _mockRepo.Setup(r => r.AddDiscountAsync(discount)).Returns(Task.CompletedTask);
        _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _service.CreateDiscountAsync(dto);

        // Assert
        Assert.Equal(discount, result);
        _mockRepo.Verify(r => r.AddDiscountAsync(discount), Times.Once);
        _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetDiscountsAsync_ShouldReturnDiscounts()
    {
        // Arrange
        var discounts = new List<Discount> { new Discount { Id = 1, Percentage = 15 } };
        _mockRepo.Setup(r => r.GetAllDiscountsAsync()).ReturnsAsync(discounts);

        // Act
        var result = await _service.GetDiscountsAsync();

        // Assert
        Assert.Single(result);
        Assert.Equal(15, result[0].Percentage);
    }

    [Fact]
    public async Task DeleteDiscountAsync_ShouldReturnFalse_WhenDiscountNotFound()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetDiscountByIdAsync(99)).ReturnsAsync((Discount?)null);

        // Act
        var result = await _service.DeleteDiscountAsync(99);

        // Assert
        Assert.False(result);
        _mockRepo.Verify(r => r.RemoveDiscount(It.IsAny<Discount>()), Times.Never);
    }

    [Fact]
    public async Task DeleteDiscountAsync_ShouldRemoveAndSave_WhenDiscountFound()
    {
        // Arrange
        var discount = new Discount { Id = 5, Percentage = 25 };
        _mockRepo.Setup(r => r.GetDiscountByIdAsync(5)).ReturnsAsync(discount);
        _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _service.DeleteDiscountAsync(5);

        // Assert
        Assert.True(result);
        _mockRepo.Verify(r => r.RemoveDiscount(discount), Times.Once);
        _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    #endregion
}