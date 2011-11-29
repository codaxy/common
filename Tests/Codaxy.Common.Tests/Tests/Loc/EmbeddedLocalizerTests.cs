using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaTest;
using Codaxy.Common.Globalization;
using Codaxy.Common.Localization;

namespace Codaxy.Common.Tests.Tests.Loc
{
    [TestFixture]
    class EmbeddedLocalizerTests
    {
        [Localization]
        class Messages
        {
            public String TestMessage = "This is a test message.";
        }

        [Test]
        public void Test1()
        {
            using (var cs = new CultureScope("da-DK"))
            {
                Assert.AreEqual("This is a test message in Danish.", LocalizationUtil.Localize<Messages>().TestMessage);
            }
        }
    }
}
