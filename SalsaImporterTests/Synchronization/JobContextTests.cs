
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
        }

        [Test]
        public void ShouldTrackIdenticalObjectEvent()
        {
            var context = new JobContext();

            var countOfEventRaised = 0;
            context.JobContextChanged += (obj, args) => countOfEventRaised++;
            context.CountIdenticalObject();
            Assert.AreEqual(1, context.IdenticalObjectCount);
            context.CountIdenticalObject();
            Assert.AreEqual(2, context.IdenticalObjectCount);
        }


    }
}
