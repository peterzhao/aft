using System;
using System.IO;
using System.Xml.Linq;
using NUnit.Framework;
using SalsaImporter.Exceptions;
using SalsaImporter.Repositories;
using SalsaImporter.Salsa;
using SalsaImporter.Synchronization;

namespace SalsaImporterTests.Synchronization
{
    [TestFixture]
    public class SyncErrorHandlerTests
    {
        private const string ObjectType = "supporter";

        [Test]
        public void ShouldThrowExceptionWhenFailureExceedsThreshold()
        {
            var handler = new SyncErrorHandler(2);
            var exception1 = new ApplicationException("testing");
            var exception2 = new ApplicationException("testing");
            var exception3 = new ApplicationException("testing");
            var data1 = new SyncObject(ObjectType);
            var data2 = new SyncObject(ObjectType);
            var data3 = new SyncObject(ObjectType);

            Assert.DoesNotThrow(() => handler.HandleSyncObjectFailure(data1, null, exception1));
            Assert.DoesNotThrow(() => handler.HandleSyncObjectFailure(data2, null, exception2));
            Assert.Throws<SyncAbortedException>(() => handler.HandleSyncObjectFailure(data3, null, exception3));
        }


        [Test]
        public void ShouldNotifySyncEventForSyncObjectFailure()
        {
            const int expectedSalsaKey = 123;

            var handler = new SyncErrorHandler(2);

            SyncEventArgs syncEventArgs = null;
            handler.NotifySyncEvent += (sender, args) => syncEventArgs = args;

            var syncObject = new SyncObject(ObjectType);
            syncObject.SalsaKey = expectedSalsaKey;

            var destination = new object();
            var exception =  new ApplicationException("some exception");
            handler.HandleSyncObjectFailure(syncObject, destination, exception);


            Assert.AreEqual(SyncEventType.Error, syncEventArgs.EventType);
            Assert.AreEqual(expectedSalsaKey, syncEventArgs.SalsaKey);
            Assert.AreEqual(exception, syncEventArgs.Error);
            Assert.AreEqual(destination, syncEventArgs.Destination);
        }

        [Test]
        public void HandleSalsaClientException()
        {
            const int expectedSalsaKey = 123;

            var handler = new SyncErrorHandler(2);

            SyncEventArgs syncEventArgs = null;
            handler.NotifySyncEvent += (sender, args) => syncEventArgs = args;

            var syncObject = new SyncObject(ObjectType);
            syncObject.SalsaKey = expectedSalsaKey;

            var destination = new object();
            var exception = new ApplicationException("some exception");
            handler.HandleSalsaClientException(ObjectType,expectedSalsaKey, destination, exception);


            Assert.AreEqual(SyncEventType.Error, syncEventArgs.EventType);
            Assert.AreEqual(ObjectType, syncEventArgs.ObjectType);
            Assert.AreEqual(expectedSalsaKey, syncEventArgs.SalsaKey);
            Assert.AreEqual(exception, syncEventArgs.Error);
            Assert.AreEqual(destination, syncEventArgs.Destination);
        }


        [Test]
        public void ShouldHandleMappingFailure()
        {

            var handler = new SyncErrorHandler(2);

            SyncEventArgs syncEventArgs = null;
            handler.NotifySyncEvent += (sender, args) => syncEventArgs = args;

            var xml = XElement.Parse("<item/>");

            var destination = new object();
            var exception = new ApplicationException("some exception");
            handler.HandleMappingFailure(ObjectType, xml, destination, exception);


            Assert.AreEqual(SyncEventType.Error, syncEventArgs.EventType);
            Assert.AreEqual(exception, syncEventArgs.Error);
            Assert.AreEqual(destination, syncEventArgs.Destination);
        }
    }
}