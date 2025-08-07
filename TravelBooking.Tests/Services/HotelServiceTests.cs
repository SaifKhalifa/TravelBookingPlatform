using AutoMapper;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TravelBooking.Application.DTOs;
using TravelBooking.Application.Services;
using TravelBooking.Application.Services.Interfaces.IRepository;
using TravelBooking.Domain.Entities;
using Xunit;

namespace TravelBooking.Tests.Services;

public class HotelServiceTests
{
    private readonly Mock<IHotelRepository> _mockRepo;
    private readonly Mock<IMapper> _mockMapper;
    private readonly HotelService _service;

    public HotelServiceTests()
    {
        _mockRepo = new Mock<IHotelRepository>();
        _mockMapper = new Mock<IMapper>();
        _service = new HotelService(_mockRepo.Object, _mockMapper.Object);
    }

    #region GetAllHotelsAsync

    [Fact]
    public async Task GetAllHotelsAsync_ShouldReturnHotelDtos_WhenHotelsExist()
    {
        // Arrange
        var hotels = new List<Hotel>
        {
            new Hotel { Id = 1, Name = "Hotel One", StarRate = 5, Location = "Downtown", City = new City { Name = "Amman" } },
            new Hotel { Id = 2, Name = "Hotel Two", StarRate = 3, Location = "Center", City = new City { Name = "Irbid" } }
        };
        var query = new HotelQueryDto { City = null, Stars = null };

        _mockRepo.Setup(r => r.GetAllHotelsAsync(null, null)).ReturnsAsync(hotels);

        // Act
        var result = await _service.GetAllHotelsAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("Hotel One", result[0].Name);
        Assert.Equal("Amman", result[0].City);
    }

    [Fact]
    public async Task GetAllHotelsAsync_ShouldApplyCityFilter()
    {
        // Arrange
        var hotels = new List<Hotel>
        {
            new Hotel { Id = 1, Name = "Hotel One", StarRate = 5, Location = "Downtown", City = new City { Name = "Amman" } }
        };
        var query = new HotelQueryDto { City = "amman", Stars = null };

        _mockRepo.Setup(r => r.GetAllHotelsAsync("amman", null)).ReturnsAsync(hotels);

        // Act
        var result = await _service.GetAllHotelsAsync(query);

        // Assert
        Assert.Single(result);
        Assert.Equal("Amman", result[0].City);
    }

    [Fact]
    public async Task GetAllHotelsAsync_ShouldApplyStarsFilter()
    {
        // Arrange
        var hotels = new List<Hotel>
        {
            new Hotel { Id = 1, Name = "Hotel One", StarRate = 4, Location = "Beach", City = new City { Name = "Aqaba" } }
        };
        var query = new HotelQueryDto { City = null, Stars = 4 };

        _mockRepo.Setup(r => r.GetAllHotelsAsync(null, 4)).ReturnsAsync(hotels);

        // Act
        var result = await _service.GetAllHotelsAsync(query);

        // Assert
        Assert.Single(result);
        Assert.Equal(4, result[0].StarRate);
    }

    [Fact]
    public async Task GetAllHotelsAsync_ShouldReturnEmptyList_WhenNoHotelsFound()
    {
        // Arrange
        var query = new HotelQueryDto { City = "nowhere", Stars = 2 };
        _mockRepo.Setup(r => r.GetAllHotelsAsync("nowhere", 2)).ReturnsAsync(new List<Hotel>());

        // Act
        var result = await _service.GetAllHotelsAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    #endregion

    #region GetHotelByIdAsync

    [Fact]
    public async Task GetHotelByIdAsync_ShouldReturnDto_WhenHotelExists()
    {
        // Arrange
        var hotel = new Hotel
        {
            Id = 1,
            Name = "Royal",
            StarRate = 5,
            Location = "Main St.",
            City = new City { Name = "Amman" },
            Rooms = new List<Room>
            {
                new Room { Id = 11, RoomType = new RoomType { Name = "Suite" }, Discount = null }
            }
        };
        var mappedHotelDto = new HotelWithRoomsDto { Id = 1, Name = "Royal" };
        var mappedRoomDto = new RoomDto { Id = 11, RoomType = "Suite" };

        _mockRepo.Setup(r => r.GetHotelWithRoomsAsync(1)).ReturnsAsync(hotel);
        _mockMapper.Setup(m => m.Map<HotelWithRoomsDto>(hotel)).Returns(mappedHotelDto);
        _mockMapper.Setup(m => m.Map<RoomDto>(It.IsAny<Room>())).Returns(mappedRoomDto);

        // Act
        var result = await _service.GetHotelByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.NotNull(result.Rooms);
        Assert.Single(result.Rooms);
        Assert.Equal(mappedRoomDto, result.Rooms.First());
    }

    [Fact]
    public async Task GetHotelByIdAsync_ShouldReturnNull_WhenHotelNotFound()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetHotelWithRoomsAsync(99)).ReturnsAsync((Hotel?)null);

        // Act
        var result = await _service.GetHotelByIdAsync(99);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetHotelByIdAsync_ShouldReturnRoomsList_WhenHotelHasMultipleRooms()
    {
        // Arrange
        var hotel = new Hotel
        {
            Id = 2,
            Name = "Multiple Rooms Hotel",
            StarRate = 4,
            City = new City { Name = "Jericho" },
            Rooms = new List<Room>
            {
                new Room { Id = 1, RoomType = new RoomType { Name = "Single" } },
                new Room { Id = 2, RoomType = new RoomType { Name = "Double" } }
            }
        };

        var mappedHotelDto = new HotelWithRoomsDto { Id = 2, Name = "Multiple Rooms Hotel" };
        var mappedRoomDtos = new List<RoomDto>
        {
            new RoomDto { Id = 1, RoomType = "Single" },
            new RoomDto { Id = 2, RoomType = "Double" }
        };

        _mockRepo.Setup(r => r.GetHotelWithRoomsAsync(2)).ReturnsAsync(hotel);
        _mockMapper.Setup(m => m.Map<HotelWithRoomsDto>(hotel)).Returns(mappedHotelDto);
        _mockMapper.Setup(m => m.Map<RoomDto>(It.IsAny<Room>()))
            .Returns((Room r) => mappedRoomDtos.First(d => d.Id == r.Id));

        // Act
        var result = await _service.GetHotelByIdAsync(2);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Rooms.Count);
        Assert.Equal(mappedRoomDtos[0], result.Rooms[0]);
        Assert.Equal(mappedRoomDtos[1], result.Rooms[1]);
    }

    #endregion
}
