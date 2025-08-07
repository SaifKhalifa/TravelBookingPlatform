namespace TravelBooking.Domain.Entities;

public class Payment
{
    public int Id { get; set; }

    public int BookingId { get; set; }
    public Booking? Booking { get; set; }

    public decimal Amount { get; set; }
    public string Status { get; set; } = "Pending"; // Paid, Failed, Refunded
    public string Method { get; set; } = "Cash";    // Visa, PayPal, etc.
    public string? TransactionId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}