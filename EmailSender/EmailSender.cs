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

    [Function(nameof(EmailSender))]
    public async Task Run(
        [ServiceBusTrigger("email-service", Connection = "SbConnection")]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        var body = message.Body.ToString();
        var bookingMessage = JsonSerializer.Deserialize<BookingMessage>(body);

        string connectionString = Environment.GetEnvironmentVariable("EmailClientConnection")!;
        var emailClient = new EmailClient(connectionString);

        var emailMessage = new EmailMessage(
           senderAddress: "DoNotReply@00b84a5f-4825-429d-8991-e3bdf57df9e1.azurecomm.net",
           content: new EmailContent("Test Email")
           {
               PlainText = $@"Thanks for your booking {bookingMessage!.FirstName}",
               Html = @$"
		        <html>
			        <body>
				        <h1>Thanks for your booking {bookingMessage.FirstName}</h1>
                        <p>Her is your booking details</p>
                        <p>Ticket quantity: {bookingMessage.TicketQuantity}</p>
                        <p>Total price: {bookingMessage.TotalPrice}</p>
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