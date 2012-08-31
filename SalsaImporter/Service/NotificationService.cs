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

        //TODO: get these from config.
        public NotificationService(MailConfig mailConfig, ISmtpClient mailer)
        {
            _sender = mailConfig.Sender;
            _recipient = mailConfig.Recipient;
            _subject = mailConfig.Subject;
            _mailer = mailer;
        }

        public void SendNotification(string notification)
        {
            var msg = new MailMessage(_sender, _recipient, _subject, notification);
            
            _mailer.Send(msg);
        }
    }
}
