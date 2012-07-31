using System;
using System.Collections.Specialized;
using NUnit.Framework;
using SalsaImporter;

namespace SalsaImporterTests
{
    [TestFixture]
    internal class ImporterErrorHandlerTests
    {
        [Test]
        public void ShouldAllowContinueUntilFailureTimesExceedThreshold()
        {
            var handler = new ImporterErrorHandler(2);
            var data1 = new NameValueCollection {{"uid", "1"}};
            var data2 = new NameValueCollection {{"uid", "2"}};
            var data3 = new NameValueCollection {{"uid", "3"}};

            Assert.DoesNotThrow(() => handler.CanContinueToCreate(data1));
            Assert.DoesNotThrow(() => handler.CanContinueToCreate(data2));
            Assert.Throws<OperationCanceledException>(() => handler.CanContinueToCreate(data3));

            Assert.AreEqual(data1, handler.FailedRecordsToCreate["1"]);
            Assert.AreEqual(data2, handler.FailedRecordsToCreate["2"]);
            Assert.AreEqual(data3, handler.FailedRecordsToCreate["3"]);
        }
    }
}