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

        [SetUp]
        public void Setup()
        {
            Config.Environment = Config.Test;
            _mockMailer = new Mock<ISmtpClient>();
            
            _notification = "Test notification for NotificationService testing.";
            _targetNotificationService = new NotificationService(_mockMailer.Object);
        }

        [Test]
        public void ShouldSendMailMessage()
        {
            _mockMailer.Setup(mock => mock.Send(It.IsAny<MailMessage>()));
            _targetNotificationService.SendNotification(_notification);
            _mockMailer.Verify();
        }

        [Test]
        [Category("IntegrationTest")]
        public void ShouldSendSecureMailMessage()
        {
            var notificationService = new NotificationService(new EmailService());
            notificationService.SendNotification(_notification);   
        }

    }
}
