using System;
using System.IO;
using NUnit.Framework;
using SalsaImporter.Aft;
using SalsaImporter.Exceptions;
using SalsaImporter.Synchronization;

namespace SalsaImporterTests.Synchronization
{
    [TestFixture]
    public class SyncErrorHandlerTests
    {

        [Test]
        public void ShouldAllowContinueToAddUntilFailureTimesExceedThreshold()
        {
            var handler = new SyncErrorHandler(2, 1);
            var exception1 = new ApplicationException("testing");
            var exception2 = new ApplicationException("testing");
            var exception3 = new ApplicationException("testing");
            var data1 = new Supporter {Email = "foo1@abc.com"};
            var data2 = new Supporter {Email = "foo2@abc.com"};
            var data3 = new Supporter {Email = "foo3@abc.com"};

            Assert.DoesNotThrow(() => handler.HandleAddObjectFailure(data1, exception1));
            Assert.DoesNotThrow(() => handler.HandleAddObjectFailure(data2, exception2));
            Assert.Throws<SyncAbortedException>(() => handler.HandleAddObjectFailure(data3, exception3));

            Assert.AreEqual(exception1, handler.AddFailure[data1]);
            Assert.AreEqual(exception2, handler.AddFailure[data2]);
            Assert.AreEqual(exception3, handler.AddFailure[data3]);
        }

        [Test]
        public void ShouldAllowContinueToDeleteUntilFailureTimesExceedThreshold()
        {
            var handler = new SyncErrorHandler(1, 2);
            var key1 = "key1";
            var key2 = "key2";
            var key3 = "key3";

            Assert.DoesNotThrow(() => handler.HandleDeleteObjectFailure(key1));
            Assert.DoesNotThrow(() => handler.HandleDeleteObjectFailure(key2));
            Assert.Throws<SyncAbortedException>(() => handler.HandleDeleteObjectFailure(key3));

            Assert.AreEqual(key1, handler.DeleteFailure["key1"]);
            Assert.AreEqual(key2, handler.DeleteFailure["key2"]);
            Assert.AreEqual(key3, handler.DeleteFailure["key3"]);
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