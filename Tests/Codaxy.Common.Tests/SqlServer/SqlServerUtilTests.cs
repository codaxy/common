using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaTest;
using Codaxy.Common.SqlServer;

namespace Codaxy.Common.Tests.SqlServer
{
    [TestFixture]
    class SqlServerUtilTests
    {
        [Test]
        public void ReadEmbeddedScripts()
        {
            var scripts = SqlScriptUtil.ReadEmbeddedDirectory(this.GetType().Assembly, "Embedded");
            Assert.AreEqual(2, scripts.Length);
            Assert.IsTrue(scripts[0].Name.StartsWith("0001"));
            Assert.IsTrue(scripts[1].Name.StartsWith("0002"));
        }
    }
}
