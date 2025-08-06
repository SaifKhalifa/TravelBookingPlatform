using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TravelBooking.Application.DTOs;
using TravelBooking.Application.Services.Interfaces;

namespace TravelBooking.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ReviewController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    // POST: /api/review
    [HttpPost]
    public async Task<IActionResult> LeaveReview([FromBody] ReviewCreateDto dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        try
        {
            await _reviewService.LeaveReviewAsync(userId, dto);
            return Ok("Review submitted successfully");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }

    // GET: /api/review/hotel/5
    [AllowAnonymous]
    [HttpGet("hotel/{hotelId}")]
    public async Task<IActionResult> GetHotelReviews(int hotelId)
    {
        var response = await _reviewService.GetReviewsByHotelAsync(hotelId);
        return Ok(response);
    }

    // DELETE: /api/review/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReview(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var success = await _reviewService.DeleteReviewAsync(id, userId);
        return success
            ? Ok("Review soft-deleted and logged.")
            : BadRequest("Review not found, already deleted, or outside deletion time window.");
    }

    // Admin DELETE: /api/review/admin/{id}
    [Authorize(Roles = "Admin")]
    [HttpDelete("admin/{id}")]
    public async Task<IActionResult> AdminDeleteReview(int id)
    {
        var adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var success = await _reviewService.AdminDeleteReviewAsync(id, adminId);
        return success
            ? Ok($"Review {id} deleted by admin {adminId}.")
            : NotFound("Review not found or already deleted.");
    }

    // Admin GET: /api/review/user/{userId}
    [Authorize(Roles = "Admin")]
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserReviews(int userId)
    {
        var response = await _reviewService.GetReviewsByUserAsync(userId);
        return Ok(response);
    }
}
