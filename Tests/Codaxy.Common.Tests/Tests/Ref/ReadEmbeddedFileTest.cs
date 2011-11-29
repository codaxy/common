using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaTest;
using Codaxy.Common.Globalization;
using System.Threading;

namespace Codaxy.Common.Tests.Tests.Ref
{
    [TestFixture]
    class ReadEmbeddedFileTest
    {
        [Test(Active = false)]
        public void EmbeddedFileCanBeReadTest1()
        {
            var assembly = this.GetType().Assembly;            
            var fs = Codaxy.Common.Reflection.AssemblyHelper.ReadEmbeddedFile(assembly, "Embedded.txt");
            Assert.IsNotNull(fs);
            fs.Dispose();
        }

        [Test(Active = false)]
        public void EmbeddedFileCanBeReadTest2()
        {
            var assembly = this.GetType().Assembly;
            var files = assembly.GetManifestResourceNames();
            var fs = Codaxy.Common.Reflection.AssemblyHelper.ReadEmbeddedFile(assembly, @"Localization\da-DK.xml");
            Assert.IsNotNull(fs);
            fs.Dispose();
        }

    }
}
