﻿using Microsoft.AspNetCore.Identity.UI.Services;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Configuration;

namespace Zetta.Utilidades
{
    public class EmailSender : IEmailSender, IEmailSenderWithAttachment
    {
        public string SendGridSecret { get; set; }

        public EmailSender(IConfiguration _config)
        {
            SendGridSecret = _config.GetValue<string>("SendGrid:SecretKey");
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SendGridClient(SendGridSecret);
            var from = new EmailAddress("trevi83.ft@gmail.com");
            var to = new EmailAddress(email);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, "", htmlMessage);

            return client.SendEmailAsync(msg);
        }

        public async Task SendEmailWithAttachmentAsync(string email, string subject, string htmlMessage, byte[] attachmentData, string attachmentFileName)
        {
            var client = new SendGridClient(SendGridSecret);
            var from = new EmailAddress("trevi83.ft@gmail.com");
            var to = new EmailAddress(email);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, "", htmlMessage);

            var attachment = new Attachment
            {
                Content = Convert.ToBase64String(attachmentData),
                Filename = attachmentFileName,
                Type = "application/pdf", // Establece el tipo MIME del archivo adjunto
                Disposition = "attachment" // Indica que el archivo adjunto debe ser descargado
            };

            msg.AddAttachment(attachment);

            await client.SendEmailAsync(msg);
        }
    }

}
