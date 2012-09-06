using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace SalsaImporter.Service
{
    public class EmailService : ISmtpClient
    {
        private readonly SmtpClient _mailer ; 

        public EmailService()
        {
            _mailer = new SmtpClient(Config.SmtpHost, Config.SmtpPort);
            if (Config.SmtpRequireLogin)
                _mailer.Credentials = new NetworkCredential(Config.SmtpUserName, Config.SmtpFromPassword);
            _mailer.EnableSsl = Config.SmtpRequireSsl;
        }

        public void Send(MailMessage message)
        {
            _mailer.Send(message);
        }

      
    }
}
