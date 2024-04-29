namespace Zetta.Utilidades
{
    public interface IEmailSenderWithAttachment
    {
        Task SendEmailWithAttachmentAsync(string email, string subject, string htmlMessage, byte[] attachmentData, string attachmentFileName);

    }
}
