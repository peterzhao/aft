using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using SalsaImporter.Service;

namespace SalsaImporterTests.MailService
{
    public class StubSmtpClient: ISmtpClient
    {
        private MailMessage _message;

        public MailMessage Message
        {
            get { return _message; }
        }

        public void Send(MailMessage message)
        {
            _message = message;
        }
    }
}
