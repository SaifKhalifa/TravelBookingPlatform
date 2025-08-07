namespace TravelBooking.Domain.Entities;

public class RoomType
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty; // Standard, Deluxe, Suite
    public string Description { get; set; } = string.Empty;
    public ICollection<Room>? Rooms { get; set; }
}
