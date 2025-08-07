namespace TravelBooking.Application.DTOs;
public class RoomDto
{
    public int Id { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public int Adults { get; set; }
    public int Children { get; set; }
    public decimal PricePerNight { get; set; }
    public bool IsAvailable { get; set; }

    public string RoomType { get; set; } = string.Empty;
    public string? Discount { get; set; }
}
