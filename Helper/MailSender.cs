using System.Net;
using System.Net.Mail;
namespace SpeakerManagement.Helper
{
    public class MailSender
    {
        #region Private
        protected readonly static string smtp = "smtp.gmail.com";
        protected readonly static int smtpPort = 587;
        protected readonly static string mail = "";
        protected readonly static string appCode = "";
        #endregion

        #region Public
        public static bool SendEmailConfirmationMail(string recipientEmail, string subject, string htmlBody, string attachmentFilePath)
        {
            try
            {
                using (var client = new SmtpClient(smtp, smtpPort))
                {
                    client.EnableSsl = true;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(mail, appCode);

                    // Create and send email message
                    var message = new MailMessage(mail, recipientEmail, subject, htmlBody);
                    message.IsBodyHtml = true; // Set body as HTML

                    // Attach a file to the email message
                    if (!string.IsNullOrEmpty(attachmentFilePath) && File.Exists(attachmentFilePath))
                    {
                        var attachment = new Attachment(attachmentFilePath);
                        message.Attachments.Add(attachment);
                    }

                    client.Send(message);

                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion
    }
}
