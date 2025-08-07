using AutoMapper;
using TravelBooking.Application.DTOs;
using TravelBooking.Application.Services.Interfaces;
using TravelBooking.Application.Services.Interfaces.IRepository;
using TravelBooking.Domain.Entities;

namespace TravelBooking.Application.Services;

public class ReviewService : IReviewService
{
    private readonly IReviewRepository _repo;
    private readonly IMapper _mapper;

    public ReviewService(IReviewRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<List<ReviewDto>> GetReviewsByHotelAsync(int hotelId)
    {
        var reviews = await _repo.GetReviewsByHotelAsync(hotelId);
        return _mapper.Map<List<ReviewDto>>(reviews);
    }

    public async Task<List<ReviewDto>> GetReviewsByUserAsync(int userId)
    {
        var reviews = await _repo.GetReviewsByUserAsync(userId);
        return _mapper.Map<List<ReviewDto>>(reviews);
    }

    public async Task LeaveReviewAsync(int userId, ReviewCreateDto dto)
    {
        if (!await _repo.HotelExistsAsync(dto.HotelId))
            throw new ArgumentException("Invalid Hotel ID");

        if (await _repo.HasReviewedAsync(userId, dto.HotelId))
            throw new InvalidOperationException("You have already reviewed this hotel.");

        var review = new Review
        {
            HotelId = dto.HotelId,
            UserId = userId,
            Rating = dto.Rating,
            Comment = dto.Comment,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        await _repo.AddReviewAsync(review);
        await _repo.SaveChangesAsync();
    }

    public async Task<bool> DeleteReviewAsync(int reviewId, int userId)
    {
        var review = await _repo.GetReviewByIdAsync(reviewId);
        if (review == null || review.UserId != userId || review.IsDeleted)
            return false;

        if ((DateTime.UtcNow - review.CreatedAt).TotalHours > 24)
            return false;

        review.IsDeleted = true;
        review.DeletedAt = DateTime.UtcNow;
        review.DeletedBy = $"User:{userId}";

        await _repo.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AdminDeleteReviewAsync(int reviewId, int adminId)
    {
        var review = await _repo.GetReviewByIdAsync(reviewId);
        if (review == null || review.IsDeleted)
            return false;

        review.IsDeleted = true;
        review.DeletedAt = DateTime.UtcNow;
        review.DeletedBy = $"Admin:{adminId}";
        review.DeletedByAdminId = adminId;

        await _repo.SaveChangesAsync();
        return true;
    }
}
