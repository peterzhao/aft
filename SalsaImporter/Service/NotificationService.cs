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
        private bool _secureFlag;

        public NotificationService(ISmtpClient mailer)
        {
            _sender = Config.SmtpFromAddress;
            _recipient = Config.SmtpNotificationRecipient;
            _secureFlag = Config.SmtpRequireSsl;
            _subject = "Salsa Sync Notification";
            _mailer = mailer;
        }

        public void SendNotification(string notification)
        {
            var msg = new MailMessage(_sender, _recipient, _subject, notification);
            _mailer.EnableSsl(_secureFlag);
            _mailer.Send(msg);
        }
    }
}
