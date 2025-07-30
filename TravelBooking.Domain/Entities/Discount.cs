using TravelBooking.Domain.Entities;

public class Discount
{
    public int Id { get; set; }

    public string Code { get; set; } = string.Empty; // Optional: promo code that user can enter
    public string Name { get; set; } = string.Empty;

    public decimal Percentage { get; set; } // 15 means 15% off
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public ICollection<Room>? Rooms { get; set; }
}