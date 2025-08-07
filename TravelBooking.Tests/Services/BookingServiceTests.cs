using AutoMapper;
using Moq;
using TravelBooking.Application.DTOs;
using TravelBooking.Application.Services;
using TravelBooking.Application.Services.Interfaces.IRepository;
using TravelBooking.Domain.Entities;

namespace TravelBooking.Tests.Services
{
    public class BookingServiceTests
    {
        private readonly Mock<IBookingRepository> _bookingRepoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly BookingService _service;

        public BookingServiceTests()
        {
            _bookingRepoMock = new Mock<IBookingRepository>();
            _mapperMock = new Mock<IMapper>();
            _service = new BookingService(_bookingRepoMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task BookAsync_ShouldBookRoom_WhenValidData()
        {
            // Arrange
            var room = new Room
            {
                Id = 1,
                PricePerNight = 100,
                IsAvailable = true,
                Discount = new Discount { Percentage = 10 }
            };

            var dto = new BookingCreateDto
            {
                RoomId = 1,
                CheckInDate = DateTime.Today,
                CheckOutDate = DateTime.Today.AddDays(2)
            };

            _bookingRepoMock.Setup(r => r.GetRoomWithDetailsAsync(dto.RoomId)).ReturnsAsync(room);
            _bookingRepoMock.Setup(r => r.AddBookingAsync(It.IsAny<Booking>())).Returns(Task.CompletedTask);
            _bookingRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            _bookingRepoMock.Setup(r => r.AddPaymentAsync(It.IsAny<Payment>())).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<BookingDto>(It.IsAny<Booking>())).Returns(new BookingDto { Id = 1 });

            // Act
            var result = await _service.BookAsync(1, dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task GetBookingHistoryAsync_ShouldReturnMappedDtos()
        {
            // Arrange
            var bookings = new List<Booking> { new Booking { Id = 1 } };
            _bookingRepoMock.Setup(r => r.GetUserBookingsAsync(1)).ReturnsAsync(bookings);
            _mapperMock.Setup(m => m.Map<List<BookingDto>>(bookings)).Returns(new List<BookingDto> { new BookingDto { Id = 1 } });

            // Act
            var result = await _service.GetBookingHistoryAsync(1);

            // Assert
            Assert.Single(result);
            Assert.Equal(1, result[0].Id);
        }

        [Fact]
        public async Task GetBookingConfirmationAsync_ShouldReturnNull_WhenNotFound()
        {
            _bookingRepoMock.Setup(r => r.GetBookingWithRoomAsync(1, 1)).ReturnsAsync((Booking?)null);

            var result = await _service.GetBookingConfirmationAsync(1, 1);

            Assert.Null(result);
        }

        [Fact]
        public async Task CancelBookingAsync_ShouldReturnNull_IfNotFoundOrAlreadyCancelled()
        {
            _bookingRepoMock.Setup(r => r.GetBookingAsync(1, 1)).ReturnsAsync((Booking?)null);

            var result = await _service.CancelBookingAsync(1, 1);

            Assert.Null(result);
        }

        [Fact]
        public async Task CancelBookingAsync_ShouldCancelBooking_WhenValid()
        {
            var booking = new Booking { Id = 1, Status = "Confirmed", Room = new Room() };
            _bookingRepoMock.Setup(r => r.GetBookingAsync(1, 1)).ReturnsAsync(booking);
            _bookingRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<BookingDto>(booking)).Returns(new BookingDto { Id = 1 });

            var result = await _service.CancelBookingAsync(1, 1);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }
    }
}