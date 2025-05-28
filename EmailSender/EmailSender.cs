using Azure;
using Azure.Communication.Email;
using Azure.Messaging.ServiceBus;
using EmailSender.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace EmailSender;

public class EmailSender
{
    private readonly ILogger<EmailSender> _logger;

    public EmailSender(ILogger<EmailSender> logger)
    {
        _logger = logger;
    }

    // Tagit hjälp av chatgpt
    [Function(nameof(EmailSender))]
    public async Task Run(
        [ServiceBusTrigger("email-service", Connection = "SbConnection")]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        var body = message.Body.ToString();

        var bookingMessage = JsonSerializer.Deserialize<BookingMessage>(body);

        if (bookingMessage == null)
        {
            _logger.LogError("BookingMessage is empty");
            return;
        }

        string connectionString = Environment.GetEnvironmentVariable("EmailClientConnection")!;
        var emailClient = new EmailClient(connectionString);

        var emailMessage = new EmailMessage(
           senderAddress: "DoNotReply@00b84a5f-4825-429d-8991-e3bdf57df9e1.azurecomm.net",
           content: new EmailContent("Booking Confirmation")
           {              
               PlainText = $@"Thanks for your booking",
               Html = @$"
                <html>
                  <body>
                    <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; background-color: #ffffff; border: 1px solid #e0e0e0; border-radius: 8px;'>
                      <h1 style='color: #2c3e50; text-align: center;'>Thanks for your booking!</h1>
                      <h3 style='color: #34495e;'>Here are your booking details</h3>

                      <h4 style='color: #2c3e50; margin-top: 24px;'>Ticket Information</h4>
                      <p style='font-size: 16px; margin: 8px 0;'><strong>Ticket quantity:</strong> {bookingMessage.TicketQuantity}</p>
                      <p style='font-size: 16px; margin: 8px 0;'><strong>Package:</strong> {bookingMessage.PackageName}</p>
                      <p style='font-size: 16px; margin: 8px 0;'><strong>Total price:</strong> ${bookingMessage.TotalPrice}</p>

                      <h4 style='color: #2c3e50; margin-top: 24px;'>Event Details</h4>
                      <p style='font-size: 16px; margin: 8px 0;'><strong>Event:</strong> {bookingMessage.EventName}</p>                      
                      <p style='font-size: 16px; margin: 8px 0;'><strong>Location:</strong> Luleå, Sweden</p>
                      <p style='font-size: 16px; margin: 8px 0;'><strong>Date:</strong> {bookingMessage.EventDate}</p>

                      <h4 style='color: #2c3e50; margin-top: 24px;'>Customer Details</h4>
                      <p style='font-size: 16px; margin: 8px 0;'><strong>Name:</strong> {bookingMessage.FirstName} {bookingMessage.LastName}</p>
                      <p style='font-size: 16px; margin: 8px 0;'><strong>Email:</strong> {bookingMessage.Email}</p>
                      <p style='font-size: 16px; margin: 8px 0;'><strong>Address:</strong></p>
                      <p style='font-size: 16px; margin: 2px 0 8px 16px;'>
                        Street: {bookingMessage.Street}<br>
                        City: {bookingMessage.City}<br>
                        Postal Code: {bookingMessage.PostalCode}
                      </p>

                      <hr style='margin: 24px 0; border: none; border-top: 1px solid #ccc;'>

                      <p style='font-size: 14px; color: #888;'>This is an automated confirmation email. Please do not reply.</p>
                    </div>
                  </body>
                </html>"

           },
           recipients: new EmailRecipients(new List<EmailAddress> { new EmailAddress($"{bookingMessage.Email}") }));

        EmailSendOperation emailSendOperation = emailClient.Send(
                WaitUntil.Completed,
                emailMessage);

        _logger.LogInformation("Message ID: {id}", message.MessageId);
        _logger.LogInformation("Message Body: {body}", message.Body);
        _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);

        // Complete the message
        await messageActions.CompleteMessageAsync(message);
    }
}