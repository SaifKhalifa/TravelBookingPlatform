using TravelBooking.Domain.Entities;

namespace TravelBooking.Domain.Entities;
public class Room
{
    public int Id { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public int Adults { get; set; }
    public int Children { get; set; }
    public decimal PricePerNight { get; set; }
    public bool IsAvailable { get; set; } = true;

    public int HotelId { get; set; }
    public Hotel? Hotel { get; set; }

    public int RoomTypeId { get; set; }
    public RoomType? RoomType { get; set; }

    public int? DiscountId { get; set; }
    public Discount? Discount { get; set; }

    public ICollection<Booking>? Bookings { get; set; }
}
