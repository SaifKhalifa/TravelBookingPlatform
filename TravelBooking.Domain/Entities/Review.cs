using TravelBooking.Domain.Entities;

public class Review
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }

    public int HotelId { get; set; }
    public Hotel? Hotel { get; set; }

    public int Rating { get; set; } // 1–5
    public string Comment { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
