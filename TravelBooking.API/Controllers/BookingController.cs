using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TravelBooking.Application.DTOs;
using TravelBooking.Application.Services.Interfaces;

namespace TravelBooking.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BookingController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    // POST: /api/booking
    [HttpPost]
    public async Task<IActionResult> BookRoom([FromBody] BookingCreateDto request)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        try
        {
            var response = await _bookingService.BookAsync(userId, request);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // GET: /api/booking/history
    [HttpGet("history")]
    public async Task<IActionResult> GetBookingHistory()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var response = await _bookingService.GetBookingHistoryAsync(userId);
        return Ok(response);
    }

    // GET: /api/booking/confirmation/{id}
    [HttpGet("confirmation/{id}")]
    public async Task<IActionResult> GetBookingConfirmation(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var response = await _bookingService.GetBookingConfirmationAsync(userId, id);

        return response != null
            ? Ok(response)
            : NotFound("Booking not found.");
    }

    // POST: /api/booking/cancel/{id}
    [HttpPost("cancel/{id}")]
    public async Task<IActionResult> CancelBooking(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var response = await _bookingService.CancelBookingAsync(userId, id);

        return response != null
            ? Ok(response)
            : BadRequest("Booking not found or already cancelled.");
    }
}