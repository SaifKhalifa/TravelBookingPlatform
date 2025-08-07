using AutoMapper;
using Moq;
using TravelBooking.Application.DTOs;
using TravelBooking.Application.Services;
using TravelBooking.Application.Services.Interfaces.IRepository;
using TravelBooking.Domain.Entities;
using Xunit;

namespace TravelBooking.Tests.Services;
public class ReviewServiceTests
{
    private readonly Mock<IReviewRepository> _mockRepo;
    private readonly Mock<IMapper> _mockMapper;
    private readonly ReviewService _service;

    public ReviewServiceTests()
    {
        _mockRepo = new Mock<IReviewRepository>();
        _mockMapper = new Mock<IMapper>();
        _service = new ReviewService(_mockRepo.Object, _mockMapper.Object);
    }

    #region LeaveReviewAsync Tests
    [Fact]
    public async Task LeaveReviewAsync_Should_AddReview_When_NotReviewedBefore()
    {
        // Arrange
        var userId = 1;
        var dto = new ReviewCreateDto { HotelId = 1, Rating = 5, Comment = "Great!" };

        _mockRepo.Setup(r => r.HasReviewedAsync(userId, dto.HotelId)).ReturnsAsync(false);

        // Act
        await _service.LeaveReviewAsync(userId, dto);

        // Assert
        _mockRepo.Verify(r => r.HasReviewedAsync(userId, dto.HotelId), Times.Once);

        _mockRepo.Verify(r => r.AddReviewAsync(It.Is<Review>(rev =>
            rev.HotelId == dto.HotelId &&
            rev.UserId == userId &&
            rev.Rating == dto.Rating &&
            rev.Comment == dto.Comment &&
            !rev.IsDeleted // Make sure the review is not deleted
        )), Times.Once);

        _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task LeaveReviewAsync_ShouldThrowInvalidOperationException_WhenAlreadyReviewed()
    {
        // Arrange
        var userId = 1;
        var dto = new ReviewCreateDto { HotelId = 1, Rating = 5, Comment = "Great!" };

        _mockRepo.Setup(r => r.HotelExistsAsync(dto.HotelId)).ReturnsAsync(true);
        _mockRepo.Setup(r => r.HasReviewedAsync(userId, dto.HotelId)).ReturnsAsync(true);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.LeaveReviewAsync(userId, dto)
        );
        Assert.Equal("You have already reviewed this hotel.", ex.Message);

        // AddReviewAsync and SaveChangesAsync should never get called
        _mockRepo.Verify(r => r.AddReviewAsync(It.IsAny<Review>()), Times.Never);
        _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task LeaveReviewAsync_ShouldThrowArgumentException_WhenHotelDoesNotExist()
    {
        // Arrange
        var userId = 1;
        var dto = new ReviewCreateDto { HotelId = 99, Rating = 5, Comment = "Not found!" };

        _mockRepo.Setup(r => r.HotelExistsAsync(dto.HotelId)).ReturnsAsync(false);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.LeaveReviewAsync(userId, dto)
        );
        Assert.Equal("Invalid Hotel ID", ex.Message);

        // should never get called
        _mockRepo.Verify(r => r.HasReviewedAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        _mockRepo.Verify(r => r.AddReviewAsync(It.IsAny<Review>()), Times.Never);
        _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task LeaveReviewAsync_ShouldAddReview_WhenHotelExistsAndNotReviewed()
    {
        // Arrange
        var userId = 1;
        var dto = new ReviewCreateDto { HotelId = 1, Rating = 5, Comment = "Nice!" };

        _mockRepo.Setup(r => r.HotelExistsAsync(dto.HotelId)).ReturnsAsync(true);
        _mockRepo.Setup(r => r.HasReviewedAsync(userId, dto.HotelId)).ReturnsAsync(false);

        // Act
        await _service.LeaveReviewAsync(userId, dto);

        // Assert
        _mockRepo.Verify(r => r.AddReviewAsync(It.Is<Review>(review =>
            review.HotelId == dto.HotelId &&
            review.UserId == userId &&
            review.Rating == dto.Rating &&
            review.Comment == dto.Comment &&
            !review.IsDeleted
        )), Times.Once);

        _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
    #endregion

    #region DeleteReviewAsync Tests
    [Fact]
    public async Task DeleteReviewAsync_ShouldSoftDelete_WhenUserOwnsReview()
    {
        // Arrange
        var review = new Review
        {
            Id = 1,
            UserId = 1,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
        _mockRepo.Setup(r => r.GetReviewByIdAsync(1)).ReturnsAsync(review);
        _mockRepo.Setup(r => r.SaveChangesAsync());

        // Act
        var result = await _service.DeleteReviewAsync(1, 1);

        // Assert
        Assert.True(result);
        Assert.True(review.IsDeleted);
        Assert.NotNull(review.DeletedAt);
        Assert.Equal("User:1", review.DeletedBy);
        _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteReviewAsync_ShouldReturnFalse_WhenReviewNotFound()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetReviewByIdAsync(1)).ReturnsAsync((Review?)null);

        // Act
        var result = await _service.DeleteReviewAsync(1, 1);

        // Assert
        Assert.False(result);
        _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteReviewAsync_ShouldReturnFalse_WhenUserDoesNotOwnReview()
    {
        // Arrange
        var review = new Review { Id = 1, UserId = 99, CreatedAt = DateTime.UtcNow, IsDeleted = false };
        _mockRepo.Setup(r => r.GetReviewByIdAsync(1)).ReturnsAsync(review);

        // Act
        var result = await _service.DeleteReviewAsync(1, 1);

        // Assert
        Assert.False(result);
        _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteReviewAsync_ShouldReturnFalse_WhenReviewIsAlreadyDeleted()
    {
        // arrange
        var review = new Review { Id = 1, UserId = 1, CreatedAt = DateTime.UtcNow, IsDeleted = true };
        _mockRepo.Setup(r => r.GetReviewByIdAsync(1)).ReturnsAsync(review);

        // act
        var result = await _service.DeleteReviewAsync(1, 1);

        // Assert
        Assert.False(result);
        _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteReviewAsync_ShouldReturnFalse_WhenReviewOlderThan24Hours()
    {
        // Arrange
        var review = new Review { Id = 1, UserId = 1, CreatedAt = DateTime.UtcNow.AddHours(-25), IsDeleted = false };
        _mockRepo.Setup(r => r.GetReviewByIdAsync(1)).ReturnsAsync(review);

        // Act
        var result = await _service.DeleteReviewAsync(1, 1);

        // Assert
        Assert.False(result);
        _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    #endregion

    #region AdminDeleteReviewAsync Tests
    [Fact]
    public async Task AdminDeleteReviewAsync_ShouldSoftDelete_WhenReviewExists()
    {
        // Arrange
        var review = new Review
        {
            Id = 1,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
        _mockRepo.Setup(r => r.GetReviewByIdAsync(1)).ReturnsAsync(review);
        _mockRepo.Setup(r => r.SaveChangesAsync());

        // Act
        var result = await _service.AdminDeleteReviewAsync(1, 7);

        // Assert
        Assert.True(result);
        Assert.True(review.IsDeleted);
        Assert.Equal("Admin:7", review.DeletedBy);
        Assert.Equal(7, review.DeletedByAdminId);
        Assert.NotNull(review.DeletedAt);
        _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task AdminDeleteReviewAsync_ShouldReturnFalse_WhenReviewNotFound()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetReviewByIdAsync(1)).ReturnsAsync((Review?)null);

        // Act
        var result = await _service.AdminDeleteReviewAsync(1, 7);

        // Assert
        Assert.False(result);
        _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task AdminDeleteReviewAsync_ShouldReturnFalse_WhenReviewAlreadyDeleted()
    {
        // Arrange
        var review = new Review
        {
            Id = 1,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = true // Already deleted!
        };
        _mockRepo.Setup(r => r.GetReviewByIdAsync(1)).ReturnsAsync(review);

        // Act
        var result = await _service.AdminDeleteReviewAsync(1, 7);

        // Assert
        Assert.False(result);
        _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Never);
    }
    #endregion

    #region GetReviews Tests
    [Fact]
    public async Task GetHotelReviewsAsync_ShouldReturnMappedDtos()
    {
        // Arrange
        var reviews = new List<Review> { new Review { Id = 1 } };
        var mappedDtos = new List<ReviewDto> { new ReviewDto { Id = 1 } };
        _mockRepo.Setup(r => r.GetReviewsByHotelAsync(1)).ReturnsAsync(reviews);
        _mockMapper.Setup(m => m.Map<List<ReviewDto>>(reviews)).Returns(mappedDtos);

        // Act
        var result = await _service.GetReviewsByHotelAsync(1);

        // Assert
        Assert.Single(result);
        Assert.Equal(mappedDtos, result);
    }

    [Fact]
    public async Task GetUserReviewsAsync_ShouldReturnMappedDtos()
    {
        // Arrange
        var reviews = new List<Review> { new Review { Id = 1 } };
        var mappedDtos = new List<ReviewDto> { new ReviewDto { Id = 1 } };
        _mockRepo.Setup(r => r.GetReviewsByUserAsync(1)).ReturnsAsync(reviews);
        _mockMapper.Setup(m => m.Map<List<ReviewDto>>(reviews)).Returns(mappedDtos);

        // Act
        var result = await _service.GetReviewsByUserAsync(1);

        // Assert
        Assert.Single(result);
        Assert.Equal(mappedDtos, result);
    }
    #endregion
}
