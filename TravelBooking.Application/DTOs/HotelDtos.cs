namespace TravelBooking.Application.DTOs;
public class HotelDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int StarRate { get; set; }
    public string Location { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
}

public class HotelWithRoomsDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int StarRate { get; set; }
    public string Location { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public List<RoomDto> Rooms { get; set; } = new();
}

public class HotelQueryDto
{
    public string? City { get; set; }
    public int? Stars { get; set; }
}