using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Threading.Tasks;

namespace SalsaImporter.Service
{
    public class NotificationService
    {
        private string _sender;
        private string _recipient;
        private string _subject;
        private ISmtpClient _mailer;

        public NotificationService(ISmtpClient mailer)
        {
            _sender = Config.SmtpFromAddress;
            _recipient = Config.SmtpNotificationRecipient.Replace(';',',');
            _subject = string.Format("Salsa Sync Notification ({0})", Config.Environment);
            _mailer = mailer;
        }

        public void SendNotification(string notification)
        {
            var msg = new MailMessage(_sender, _recipient, _subject, notification);
            _mailer.Send(msg);
        }
    }
}
