namespace EmailSender.Models;

public class VerifyEmailMessage
{
    public string MessageType { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Token { get; set; } = null!;
}
