namespace TravelBooking.Application.DTOs;
public class ReviewDto
{
    public int Id { get; set; }
    public string Hotel { get; set; } = string.Empty;
    public string User { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}
