namespace TravelBooking.Domain.Entities;

public class Review
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }

    public int HotelId { get; set; }
    public Hotel? Hotel { get; set; }

    public int Rating { get; set; } // 1–5
    public string Comment { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; } // Can be "User:1" or "Admin:7"
    public int? DeletedByAdminId { get; set; } // If deleted by admin
    public User? DeletedByAdmin { get; set; } // FK to User for admin deletion

}
