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
        private readonly string _sender;
        private readonly string _recipient;
        private readonly string _subject;
        private readonly ISmtpClient _mailer;

        public NotificationService(ISmtpClient mailer)
        {
            _sender = Config.SmtpFromAddress;
            _recipient = Config.SmtpNotificationRecipient.Replace(';',',');
            _subject = string.Format("Salsa Sync Notification ({0})", Config.Environment);
            _mailer = mailer;
        }

        public void SendNotification(string notification)
        {
            var html = notification.Replace(Environment.NewLine, "<br/>");
            var msg = new MailMessage(_sender, _recipient, _subject, html) 
            {IsBodyHtml = true, BodyEncoding = Encoding.UTF8};
            
            _mailer.Send(msg);
        }
    }
}
