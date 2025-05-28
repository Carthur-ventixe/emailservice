namespace EmailSender.Models;

public class BookingMessage
{
    public string EventName { get; set; } = null!;
    public string PackageName { get; set; } = null!;
    public DateTime EventDate { get; set; }
    public string Location { get; set; } = null!;
    public int TicketQuantity { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime? BookingDate { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Street { get; set; } = null!;
    public string City { get; set; } = null!;
    public string PostalCode { get; set; } = null!;
}
