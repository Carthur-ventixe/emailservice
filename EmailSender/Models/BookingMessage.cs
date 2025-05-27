namespace EmailSender.Models;

public class BookingMessage
{
    public int TicketQuantity { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime? BookingDate { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
}
