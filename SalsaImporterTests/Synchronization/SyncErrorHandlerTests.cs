using System;
using System.IO;
using NUnit.Framework;
using SalsaImporter.Aft;
using SalsaImporter.Exceptions;
using SalsaImporter.Repositories;
using SalsaImporter.Salsa;
using SalsaImporter.Synchronization;

namespace SalsaImporterTests.Synchronization
{
    [TestFixture]
    public class SyncErrorHandlerTests
    {

        [Test]
        public void ShouldAllowContinueToAddUntilFailureTimesExceedThreshold()
        {
            var handler = new SyncErrorHandler(2);
            var exception1 = new ApplicationException("testing");
            var exception2 = new ApplicationException("testing");
            var exception3 = new ApplicationException("testing");
            var data1 = new Supporter {Email = "foo1@abc.com"};
            var data2 = new Supporter {Email = "foo2@abc.com"};
            var data3 = new Supporter {Email = "foo3@abc.com"};

            Assert.DoesNotThrow(() => handler.HandleSyncObjectFailure(data1, null, exception1));
            Assert.DoesNotThrow(() => handler.HandleSyncObjectFailure(data2, null, exception2));
            Assert.Throws<SyncAbortedException>(() => handler.HandleSyncObjectFailure(data3, null, exception3));

            Assert.AreEqual(exception1, handler.Failures[data1]);
            Assert.AreEqual(exception2, handler.Failures[data2]);
            Assert.AreEqual(exception3, handler.Failures[data3]);
        }


        [Test]
        public void ShouldNotifySyncEvent()
        {

            int expectedId = 123;
            int expectedExternalId = 456; 
            
            var handler = new SyncErrorHandler(2);
            var exception = new ApplicationException("testing");

            var syncObject = new Supporter { Id = expectedId, ExternalId = expectedExternalId, Email = "foo1@abc.com" };
            var localRepositoy = new LocalRepository();
            SyncEventArgs syncEventArgs = null;
            handler.NotifySyncEvent += (sender, args) => syncEventArgs = args;

            handler.HandleSyncObjectFailure(syncObject, localRepositoy, exception);

            Assert.IsNotNull(syncEventArgs);
            Assert.AreEqual(SyncEventType.Error, syncEventArgs.EventType);
            Assert.AreEqual(localRepositoy, syncEventArgs.Destination);
            Assert.AreEqual(exception, syncEventArgs.Error);


            Assert.AreEqual(syncObject.ToString(), syncEventArgs.Data);
            Assert.AreEqual(expectedId, syncEventArgs.ObjectId);
            Assert.AreEqual(expectedExternalId, syncEventArgs.ExternalId);
            Assert.AreEqual("Supporter", syncEventArgs.ObjectType);
        }
    }
}