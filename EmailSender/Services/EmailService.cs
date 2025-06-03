using Azure;
using Azure.Communication.Email;
using Azure.Messaging.ServiceBus;
using EmailSender.Models;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace EmailSender.Services;

public class EmailService(EmailClient emailClient)
{
    private readonly EmailClient _emailClient = emailClient;

    public async Task SendVerificationEmailAsync(VerifyEmailMessage message)
    {
        var emailMessage = new EmailMessage(
          senderAddress: "DoNotReply@00b84a5f-4825-429d-8991-e3bdf57df9e1.azurecomm.net",
          content: new EmailContent("Verify email")
          {
              PlainText = $@"Thanks for your booking",
              Html = @$"
                <html>
                  <body style='margin:0; padding:40px 10px; background-color:#f4f4f4; font-family: Arial, sans-serif;'>
                    <table width='100%' height='100%'>
                    <tr>
                        <td align='center'>
                        <table width='100%' style='max-width: 600px; background-color: #ffffff; border-radius: 8px; padding: 30px; box-shadow: 0 0 10px rgba(0,0,0,0.1);'>
                            <tr>
                            <td align='center'>
                                <h2 style='color: #37437D;'>Confirm Your Email</h2>
                                <p style='font-size: 16px; color: #1C2346;'>Thank you for registering! Please verify your email by clicking the button below.</p>
                                <p style='margin: 30px 0;'>
                                <a href='http://localhost:5173/confirmed?email={message.Email}&token={message.Token}'
                                    style='padding: 12px 24px; background-color: #F26CF9; color: #FFFFFF; text-decoration: none; border-radius: 30px; display: inline-block; font-size: 16px;'>
                                    Verify Email
                                </a>
                                </p>                     
                            </td>
                            </tr>
                        </table>
                        </td>
                    </tr>
                    </table>
                  </body>
                </html>"

          },
          recipients: new EmailRecipients(new List<EmailAddress> { new EmailAddress($"{message.Email}") }));

        var emailSendOperation = await _emailClient.SendAsync(
                WaitUntil.Completed,
                emailMessage);
    }

    public async Task SendConfirmationEmailAsync(BookingMessage bookingMessage)
    {

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
                      <p style='font-size: 16px; margin: 8px 0;'><strong>Location:</strong>{bookingMessage.Location}</p>
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

        var emailSendOperation = await _emailClient.SendAsync(
                WaitUntil.Completed,
                emailMessage);
        
    }
}
