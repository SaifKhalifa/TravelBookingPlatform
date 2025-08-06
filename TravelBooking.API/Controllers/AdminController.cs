using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelBooking.Application.DTOs;
using TravelBooking.Application.Services.Interfaces;
using TravelBooking.Domain.Entities;

namespace TravelBooking.API.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    #region Cities CRUD

    [HttpGet("cities")]
    public async Task<IActionResult> GetCities() =>
        Ok(await _adminService.GetCitiesAsync());

    [HttpPost("cities")]
    public async Task<IActionResult> CreateCity([FromBody] City city)
    {
        await _adminService.CreateCityAsync(city);
        return Ok(city);
    }

    [HttpPut("cities/{id}")]
    public async Task<IActionResult> UpdateCity(int id, [FromBody] City city)
    {
        var result = await _adminService.UpdateCityAsync(id, city);
        return result ? Ok(city) : NotFound("City not found");
    }

    [HttpDelete("cities/{id}")]
    public async Task<IActionResult> DeleteCity(int id)
    {
        var result = await _adminService.DeleteCityAsync(id);
        return result ? Ok("City deleted") : NotFound("City not found");
    }

    #endregion

    #region Hotels CRUD

    [HttpGet("hotels")]
    public async Task<IActionResult> GetHotels() =>
        Ok(await _adminService.GetHotelsAsync());

    [HttpPost("hotels")]
    public async Task<IActionResult> CreateHotel([FromBody] CreateHotelDto dto)
    {
        var result = await _adminService.CreateHotelAsync(dto);
        return Ok(new { result.Id, result.Name });
    }

    [HttpPut("hotels/{id}")]
    public async Task<IActionResult> UpdateHotel(int id, [FromBody] Hotel updated)
    {
        var success = await _adminService.UpdateHotelAsync(id, updated);
        return success ? Ok(updated) : NotFound("Hotel not found");
    }

    [HttpDelete("hotels/{id}")]
    public async Task<IActionResult> DeleteHotel(int id)
    {
        var success = await _adminService.DeleteHotelAsync(id);
        return success ? Ok("Hotel deleted") : NotFound("Hotel not found");
    }

    #endregion

    #region Rooms CRUD

    [HttpGet("rooms")]
    public async Task<IActionResult> GetRooms() =>
        Ok(await _adminService.GetRoomsAsync());

    [HttpPost("rooms")]
    public async Task<IActionResult> CreateRoom([FromBody] CreateRoomDto dto)
    {
        var result = await _adminService.CreateRoomAsync(dto);
        return Ok(new { result.Id, result.RoomNumber });
    }

    [HttpPut("rooms/{id}")]
    public async Task<IActionResult> UpdateRoom(int id, [FromBody] Room room)
    {
        var success = await _adminService.UpdateRoomAsync(id, room);
        return success ? Ok(room) : NotFound("Room not found");
    }

    [HttpDelete("rooms/{id}")]
    public async Task<IActionResult> DeleteRoom(int id)
    {
        var success = await _adminService.DeleteRoomAsync(id);
        return success ? Ok("Room deleted") : NotFound("Room not found");
    }

    #endregion

    #region Room Types

    [HttpGet("roomtypes")]
    public async Task<IActionResult> GetRoomTypes() =>
        Ok(await _adminService.GetRoomTypesAsync());

    [HttpPost("roomtypes")]
    public async Task<IActionResult> CreateRoomType([FromBody] CreateRoomTypeDto dto)
    {
        var result = await _adminService.CreateRoomTypeAsync(dto);
        return Ok(new { result.Id, result.Name });
    }

    [HttpDelete("roomtypes/{id}")]
    public async Task<IActionResult> DeleteRoomType(int id)
    {
        var success = await _adminService.DeleteRoomTypeAsync(id);
        return success ? Ok("Room type deleted") : NotFound("Room type not found");
    }

    #endregion

    #region Discounts

    [HttpGet("discounts")]
    public async Task<IActionResult> GetDiscounts() =>
        Ok(await _adminService.GetDiscountsAsync());

    [HttpPost("discounts")]
    public async Task<IActionResult> CreateDiscount([FromBody] CreateDiscountDto dto)
    {
        var result = await _adminService.CreateDiscountAsync(dto);
        return Ok(new { result.Id, result.Name });
    }

    [HttpDelete("discounts/{id}")]
    public async Task<IActionResult> DeleteDiscount(int id)
    {
        var success = await _adminService.DeleteDiscountAsync(id);
        return success ? Ok("Discount deleted") : NotFound("Discount not found");
    }

    #endregion
}