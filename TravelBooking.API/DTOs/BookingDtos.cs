namespace TravelBooking.API.DTOs;
public class BookingCreateDto
{
    public int RoomId { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
}

public class BookingDto
{
    public int Id { get; set; }
    public string Hotel { get; set; } = string.Empty;
    public string Room { get; set; } = string.Empty;
    public string RoomType { get; set; } = string.Empty;
    public string? Discount { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; } = "Confirmed";
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
}
