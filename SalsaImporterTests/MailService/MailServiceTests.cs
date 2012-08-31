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

        private Mock<ISmtpClient> _mockMailer;
        private NotificationService _targetNotificationService;
        private string _notification;
        private string _sender;
        private string _recipient;
        private string _subject;
        private string _host;
        private int _port;

        [SetUp]
        public void Setup()
        {
            Config.Environment = Config.Test;
            _mockMailer = new Mock<ISmtpClient>();
            
            _notification = "Test notification for NotificationService testing.";
            _sender = "kyle.hodgson@gmail.com";
            _recipient = "khdogson@thoughtworks.com";
            _subject = "SalsaSync Notification";
            _host = "smtp.gmail.com";
            _port = 25;

            _targetNotificationService = new NotificationService(new MailConfig(_sender, _recipient, _subject, _host, _port), _mockMailer.Object);
        }

        [Test]
        public void ShouldSendMailMessage()
        {
            _mockMailer.Setup(mock => mock.Send(It.IsAny<MailMessage>()));
            _targetNotificationService.SendNotification(_notification);
            _mockMailer.Verify();
        }
    }
}
