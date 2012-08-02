using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SalsaImporter.Synchronization;

namespace SalsaImporterTests.Synchronization
{
    class PullSalsaToLocalTests
    {
        //[Test]
        public void ShouldRun()
        {
            var pullSalsaToLocal = new PullSalsaToLocal();
            pullSalsaToLocal.run();
        }
    }
}
