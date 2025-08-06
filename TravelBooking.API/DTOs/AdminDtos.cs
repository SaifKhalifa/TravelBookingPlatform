namespace TravelBooking.API.DTOs;
public class CreateRoomTypeDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class CreateDiscountDto
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public decimal Percentage { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

public class CreateHotelDto
{
    public string Name { get; set; } = string.Empty;
    public int StarRate { get; set; }
    public string Location { get; set; } = string.Empty;
    public string Owner { get; set; } = string.Empty;
    public int CityId { get; set; }
}

public class CreateRoomDto
{
    public string RoomNumber { get; set; } = string.Empty;
    public int Adults { get; set; }
    public int Children { get; set; }
    public decimal PricePerNight { get; set; }
    public bool IsAvailable { get; set; }
    public int HotelId { get; set; }
    public int RoomTypeId { get; set; }
    public int? DiscountId { get; set; }
}
