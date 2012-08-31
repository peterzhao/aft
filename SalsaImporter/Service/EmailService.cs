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
            if (Config.SmtpRequireLogin)
            {
                _mailer = new SmtpClient(Config.SmtpHost, Config.SmtpPort)
                              {Credentials = new NetworkCredential(Config.SmtpFromAddress, Config.SmtpFromPassword)};
            }
        }

        public void Send(MailMessage message)
        {
            _mailer.Send(message);
        }

        public void EnableSsl(bool setting)
        {
            _mailer.EnableSsl = setting;
        }
    }
}
