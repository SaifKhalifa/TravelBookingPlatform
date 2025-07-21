namespace TravelBooking.Domain.Entities;

public class City
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string PostOffice { get; set; } = string.Empty;
    public ICollection<Hotel>? Hotels { get; set; }
}
