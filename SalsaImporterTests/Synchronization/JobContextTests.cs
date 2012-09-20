using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SalsaImporter.Synchronization;

namespace SalsaImporterTests.Synchronization
{
    [TestFixture]
    public class JobContextTests
    {
        [Test]
        public void ShouldTrackSuccessEvent()
        {
            var context = new JobContext();
            var countOfEventRaised = 0;
            context.JobContextChanged += (obj, args) => countOfEventRaised++;
            context.CountSuccess();
            Assert.AreEqual(1, context.SuccessCount);
            context.CountSuccess();
            Assert.AreEqual(2, context.SuccessCount);
            Assert.AreEqual(2, countOfEventRaised);
        }

        [Test]
        public void ShouldTrackErrorEvent()
        {
            var context = new JobContext();

            var countOfEventRaised = 0;
            context.JobContextChanged += (obj, args) => countOfEventRaised++;
            context.CountError();
            Assert.AreEqual(1, context.ErrorCount);
            context.CountError();
            Assert.AreEqual(2, context.ErrorCount);
            Assert.AreEqual(2, countOfEventRaised);
        }


    }
}
