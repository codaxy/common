using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaTest;
using Codaxy.Common.Logging;

namespace Codaxy.Common.Tests.Tests.Logging
{
    [TestFixture]
    class LoggingTests
    {
        class DummyLogAppender : ILogAppender
        {

            public void Log(LogEntry entry)
            {
                
            }
        }

        [Test]
        public void NullExceptionLoggingDoesNotThrow()
        {
            var logger = new Logger(new DummyLogAppender());
            Assert.DoesNotThrow(() => { logger.Exception(null); });
            Assert.DoesNotThrow(() => { logger.Exception(null, null); });
        }
    }
}
