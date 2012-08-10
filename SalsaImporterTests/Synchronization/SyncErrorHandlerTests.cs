using System;
using System.IO;
using NUnit.Framework;
using SalsaImporter.Aft;
using SalsaImporter.Exceptions;
using SalsaImporter.Repositories;
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
        public void ShouldAllowRetrySpecificTimes()
        {
            int ountOfCalled;
            ountOfCalled = 0;
            Func<string> func = () =>
                           {
                               ountOfCalled += 1;
                               Console.WriteLine(ountOfCalled);
                               if (ountOfCalled <= 2)
                                    throw new InvalidDataException("testing error");
                               
                               return "OK";
                           };
            Assert.DoesNotThrow(() => SyncErrorHandler.Try<string, InvalidDataException>(func, 3));
        }

        [Test]
        public void ShoulOnlyAllowRetryForSpecificError()
        {
            int ountOfCalled;
            ountOfCalled = 0;
            Func<string> func = () =>
            {
                ountOfCalled += 1;
                Console.WriteLine(ountOfCalled);
                if (ountOfCalled <= 2)
                    throw new InvalidDataException("testing error");

                return "OK";
            };
            Assert.Throws<InvalidDataException>(() => SyncErrorHandler.Try<string, InvalidOperationException>(func, 3));
        }

        [Test]
        public void ShouldRethrowTheErrorAferRetrySpecificTimesButStillGetError()
        {
            int ountOfCalled;
            ountOfCalled = 0;
            Func<string> func = () =>
            {
                ountOfCalled += 1;
                Console.WriteLine(ountOfCalled);
                if (ountOfCalled <= 3)
                    throw new InvalidDataException("testing error");

                return "OK";
            };
            Assert.Throws<ApplicationException>(() => SyncErrorHandler.Try<string, InvalidDataException>(func, 3));
        }

        [Test]
        public void ShouldNotifySyncEvent()
        {
            var handler = new SyncErrorHandler(2);
            var exception = new ApplicationException("testing");
            var syncObject = new Supporter { Email = "foo1@abc.com" };
            var localRepositoy = new LocalRepository();
            SyncEventArgs syncEventArgs = null;
            handler.NotifySyncEvent += (sender, args) => syncEventArgs = args;

            handler.HandleSyncObjectFailure(syncObject, localRepositoy, exception);

            Assert.IsNotNull(syncEventArgs);
            Assert.AreEqual(SyncEventType.Error, syncEventArgs.EventType);
            Assert.AreEqual(syncObject, syncEventArgs.SyncObject);
            Assert.AreEqual(localRepositoy, syncEventArgs.Destination);
            Assert.AreEqual(exception, syncEventArgs.Error);
        }
    }
}