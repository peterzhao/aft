using System;
using System.Collections.Specialized;
using System.IO;
using NUnit.Framework;
using SalsaImporter;
using SalsaImporter.Aft;
using SalsaImporter.Exceptions;
using SalsaImporter.Synchronization;

namespace SalsaImporterTests
{
    [TestFixture]
    public class ImporterErrorHandlerTests
    {

        [Test]
        public void ShouldAllowContinueToPullUntilFailureTimesExceedThreshold()
        {
            var handler = new SyncErrorHandler(2, 500, 1);
            var exception1 = new ApplicationException("testing");
            var exception2 = new ApplicationException("testing");
            var exception3 = new ApplicationException("testing");
            var data1 = new Supporter {Email = "foo1@abc.com"};
            var data2 = new Supporter {Email = "foo2@abc.com"};
            var data3 = new Supporter {Email = "foo3@abc.com"};

            Assert.DoesNotThrow(() => handler.HandlePullObjectFailure(data1, exception1));
            Assert.DoesNotThrow(() => handler.HandlePullObjectFailure(data2, exception2));
            Assert.Throws<SyncAbortedException>(() => handler.HandlePullObjectFailure(data3, exception3));

            Assert.AreEqual(exception1, handler.PullingFailure[data1]);
            Assert.AreEqual(exception2, handler.PullingFailure[data2]);
            Assert.AreEqual(exception3, handler.PullingFailure[data3]);
        }

        [Test]
        public void ShouldAllowContinueToPushUntilFailureTimesExceedThreshold()
        {
            var handler = new SyncErrorHandler(200, 2, 1);
            var exception1 = new ApplicationException("testing");
            var exception2 = new ApplicationException("testing");
            var exception3 = new ApplicationException("testing");
            var data1 = new Supporter { Email = "foo1@abc.com" };
            var data2 = new Supporter { Email = "foo2@abc.com" };
            var data3 = new Supporter { Email = "foo3@abc.com" };

            Assert.DoesNotThrow(() => handler.HandlePushObjectFailure(data1, exception1));
            Assert.DoesNotThrow(() => handler.HandlePushObjectFailure(data2, exception2));
            Assert.Throws<SyncAbortedException>(() => handler.HandlePushObjectFailure(data3, exception3));

            Assert.AreEqual(exception1, handler.PushingFailure[data1]);
            Assert.AreEqual(exception2, handler.PushingFailure[data2]);
            Assert.AreEqual(exception3, handler.PushingFailure[data3]);
        }

        [Test]
        public void ShouldAllowContinueToDeleteUntilFailureTimesExceedThreshold()
        {
            var handler = new SyncErrorHandler(1, 500, 2);
            var key1 = "key1";
            var key2 = "key2";
            var key3 = "key3";

            Assert.DoesNotThrow(() => handler.HandleDeleteObjectFailure(key1));
            Assert.DoesNotThrow(() => handler.HandleDeleteObjectFailure(key2));
            Assert.Throws<SyncAbortedException>(() => handler.HandleDeleteObjectFailure(key3));

            Assert.AreEqual(key1, handler.DeletionFailure["key1"]);
            Assert.AreEqual(key2, handler.DeletionFailure["key2"]);
            Assert.AreEqual(key3, handler.DeletionFailure["key3"]);
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
    }
}