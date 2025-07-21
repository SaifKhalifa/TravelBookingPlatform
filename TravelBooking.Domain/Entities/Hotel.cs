using TravelBooking.Domain.Entities;

namespace TravelBooking.Domain.Entities;
public class Hotel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int StarRate { get; set; }
    public string Location { get; set; } = string.Empty;
    public string Owner { get; set; } = string.Empty;

    public int CityId { get; set; }
    public City? City { get; set; }

    public ICollection<Room>? Rooms { get; set; }
}
