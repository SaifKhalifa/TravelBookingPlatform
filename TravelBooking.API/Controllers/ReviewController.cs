using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TravelBooking.API.DTOs;
using TravelBooking.Domain.Entities;
using TravelBooking.Infrastructure.Persistence;

namespace TravelBooking.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ReviewController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public ReviewController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    // POST: /api/review
    [HttpPost]
    public async Task<IActionResult> LeaveReview([FromBody] Review review)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var hotel = await _context.Hotels.FindAsync(review.HotelId);
        if (hotel == null) return NotFound("Hotel not found");

        var newReview = new Review
        {
            UserId = userId,
            HotelId = review.HotelId,
            Rating = review.Rating,
            Comment = review.Comment,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Reviews.AddAsync(newReview);
        await _context.SaveChangesAsync();

        return Ok("Review submitted successfully");
    }

    // GET: /api/review/hotel/5
    [AllowAnonymous]
    [HttpGet("hotel/{hotelId}")]
    public async Task<IActionResult> GetHotelReviews(int hotelId)
    {
        var reviews = await _context.Reviews
            .Where(r => r.HotelId == hotelId && !r.IsDeleted)
            .Include(r => r.User)
            .Include(r => r.Hotel)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        var response = _mapper.Map<List<ReviewDto>>(reviews);
        return Ok(response);

    }

    // DELETE: /api/review/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReview(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var review = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);
        if (review == null || review.IsDeleted)
            return NotFound("Review not found or already deleted.");

        var timeSincePosted = DateTime.UtcNow - review.CreatedAt;
        if (timeSincePosted.TotalHours > 24)
            return BadRequest("You can only delete your review within 24 hours of posting.");

        review.IsDeleted = true;
        review.DeletedAt = DateTime.UtcNow;
        review.DeletedBy = $"User:{userId}";
        review.DeletedByAdminId = null;

        await _context.SaveChangesAsync();
        return Ok("Review soft-deleted and logged.");
    }

    // Admin deletion of reviews
    [Authorize(Roles = "Admin")]
    [HttpDelete("admin/{id}")]
    public async Task<IActionResult> AdminDeleteReview(int id)
    {
        var adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var review = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == id);
        if (review == null || review.IsDeleted)
            return NotFound("Review not found or already deleted.");

        review.IsDeleted = true;
        review.DeletedAt = DateTime.UtcNow;
        review.DeletedBy = $"Admin:{adminId}";
        review.DeletedByAdminId = adminId;

        await _context.SaveChangesAsync();
        return Ok($"Review {id} deleted by admin {adminId}.");
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserReviews(int userId)
    {
        var reviews = await _context.Reviews
            .Where(r => r.UserId == userId)
            .Include(r => r.Hotel)
            .Include(r => r.User)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        var response = _mapper.Map<List<ReviewDto>>(reviews);
        return Ok(response);

    }


}
