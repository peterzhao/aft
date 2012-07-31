using System;
using System.Collections.Specialized;
using System.IO;
using NUnit.Framework;
using SalsaImporter;

namespace SalsaImporterTests
{
    [TestFixture]
    public class ImporterErrorHandlerTests
    {
        [Test]
        public void ShouldAllowContinueToCreateUntilFailureTimesExceedThreshold()
        {
            var handler = new ImporterErrorHandler(2, 1);
            var data1 = new NameValueCollection {{"uid", "1"}};
            var data2 = new NameValueCollection {{"uid", "2"}};
            var data3 = new NameValueCollection {{"uid", "3"}};

            Assert.DoesNotThrow(() => handler.HandleCreateObjectFailure(data1));
            Assert.DoesNotThrow(() => handler.HandleCreateObjectFailure(data2));
            Assert.Throws<OperationCanceledException>(() => handler.HandleCreateObjectFailure(data3));

            Assert.AreEqual(data1, handler.FailedRecordsToCreate["1"]);
            Assert.AreEqual(data2, handler.FailedRecordsToCreate["2"]);
            Assert.AreEqual(data3, handler.FailedRecordsToCreate["3"]);
        }

        [Test]
        public void ShouldAllowContinueToDeleteUntilFailureTimesExceedThreshold()
        {
            var handler = new ImporterErrorHandler(1, 2);
            var key1 = "key1";
            var key2 = "key2";
            var key3 = "key3";

            Assert.DoesNotThrow(() => handler.HandleDeleteObjectFailure(key1));
            Assert.DoesNotThrow(() => handler.HandleDeleteObjectFailure(key2));
            Assert.Throws<OperationCanceledException>(() => handler.HandleDeleteObjectFailure(key3));

            Assert.AreEqual(key1, handler.FailedRecordsToDelete["key1"]);
            Assert.AreEqual(key2, handler.FailedRecordsToDelete["key2"]);
            Assert.AreEqual(key3, handler.FailedRecordsToDelete["key3"]);
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
            Assert.DoesNotThrow(() => ImporterErrorHandler.Try<string, InvalidDataException>(func, 3));
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
            Assert.Throws<InvalidDataException>(() => ImporterErrorHandler.Try<string, InvalidOperationException>(func, 3));
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
            Assert.Throws<ApplicationException>(() => ImporterErrorHandler.Try<string, InvalidDataException>(func, 3));
        }
    }
}