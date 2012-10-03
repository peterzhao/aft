using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using System.Net.Mail;
using SalsaImporter;
using SalsaImporter.Service;

namespace SalsaImporterTests.MailService
{
    [TestFixture]
    public class MailServiceTests
    {

        private StubSmtpClient _smtpClientStub;
        private NotificationService _targetNotificationService;
        private string _notification;

        [SetUp]
        public void Setup()
        {
            Config.Environment = Config.Test;
            _smtpClientStub = new StubSmtpClient();
            
            _notification = "Test notification for NotificationService testing.";
            _targetNotificationService = new NotificationService(_smtpClientStub);
        }

        [Test]
        public void ShouldSendMailMessageWithHtmlFormat()
        {
            var body = "line 1" + Environment.NewLine + "line 2";
            _targetNotificationService.SendNotification(body);
            Assert.AreEqual("line 1<br/>line 2", _smtpClientStub.Message.Body);
        }

        

        [Test]
        [Category("FunctionalTest")]
        public void ShouldSendMailMessageUsingRealGmailCredentialsAndSslEnabled()
        {
            var notificationService = new NotificationService(new EmailService());
            notificationService.SendNotification(_notification);   
        }

    }
}
