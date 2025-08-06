namespace TravelBooking.API.DTOs;
public class HotelDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int StarRate { get; set; }
    public string Location { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
}
