using System.Net.Mail;

namespace SalsaImporter.Service
{
    public interface ISmtpClient
    {
        void Send(MailMessage message);
        void EnableSsl(bool setting);
    }
}
