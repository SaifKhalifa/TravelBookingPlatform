using TravelBooking.Domain.Entities;

namespace TravelBooking.Domain.Entities;
public class Booking
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User? User { get; set; }

    public int RoomId { get; set; }
    public Room? Room { get; set; }

    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; } = "Pending"; // or Confirmed, Cancelled
}
