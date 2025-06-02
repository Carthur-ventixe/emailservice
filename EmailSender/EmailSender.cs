using Azure;
using Azure.Communication.Email;
using Azure.Messaging.ServiceBus;
using EmailSender.Models;
using EmailSender.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Mail;
using System.Text.Json;
using System.Threading.Tasks;

namespace EmailSender;

public class EmailSender(ILogger<EmailSender> logger, EmailService emailService)
{
    private readonly ILogger<EmailSender> _logger = logger;
    private readonly EmailService _emailService = emailService;

    // Tagit hjälp av chatgpt
    [Function(nameof(EmailSender))]
    public async Task Run(
        [ServiceBusTrigger("email-service", Connection = "SbConnection")]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        var body = message.Body.ToString();

        var messageType = JsonSerializer.Deserialize<BaseMessage>(body);
        if (messageType == null)
        {
            _logger.LogError("Message is empty");
            return;
        }

        switch (messageType.MessageType)
        {
            case "booking":
                var bookingMessage = JsonSerializer.Deserialize<BookingMessage>(body);
                if (bookingMessage == null)
                {
                    _logger.LogError("BookingMessage is empty");
                    return;
                }

                await _emailService.SendConfirmationEmailAsync(bookingMessage);
                break;

            case "verify-email":
                var verifyEmilMessage = JsonSerializer.Deserialize<VerifyEmailMessage>(body);
                if (verifyEmilMessage == null)
                {
                    _logger.LogError("VerifyMessage is empty");
                    return;
                }

                await _emailService.SendVerificationEmailAsync(verifyEmilMessage);
                break;
        }

        _logger.LogInformation("Message ID: {id}", message.MessageId);
        _logger.LogInformation("Message Body: {body}", message.Body);
        _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);

        await messageActions.CompleteMessageAsync(message);
    }
}