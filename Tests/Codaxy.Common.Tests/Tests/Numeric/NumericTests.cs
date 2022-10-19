using PetaTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codaxy.Common.Tests.Tests.Numeric
{
    [TestFixture]
    public class NumericTests
    {
        [Test]
        public void IsNumericWorksProperly()
        {
            Assert.IsTrue(Common.Numeric.IsNumericType(typeof(byte)));
            Assert.IsTrue(Common.Numeric.IsNumericType(typeof(sbyte)));
            Assert.IsTrue(Common.Numeric.IsNumericType(typeof(short)));
            Assert.IsTrue(Common.Numeric.IsNumericType(typeof(ushort)));
            Assert.IsTrue(Common.Numeric.IsNumericType(typeof(int)));
            Assert.IsTrue(Common.Numeric.IsNumericType(typeof(long)));
            Assert.IsTrue(Common.Numeric.IsNumericType(typeof(ulong)));
            Assert.IsTrue(Common.Numeric.IsNumericType(typeof(decimal)));
            Assert.IsTrue(Common.Numeric.IsNumericType(typeof(float)));
            Assert.IsTrue(Common.Numeric.IsNumericType(typeof(double)));

            Assert.IsTrue(Common.Numeric.IsNumericType(typeof(byte?)));
            Assert.IsTrue(Common.Numeric.IsNumericType(typeof(sbyte?)));
            Assert.IsTrue(Common.Numeric.IsNumericType(typeof(short?)));
            Assert.IsTrue(Common.Numeric.IsNumericType(typeof(ushort?)));
            Assert.IsTrue(Common.Numeric.IsNumericType(typeof(int?)));
            Assert.IsTrue(Common.Numeric.IsNumericType(typeof(long?)));
            Assert.IsTrue(Common.Numeric.IsNumericType(typeof(ulong?)));
            Assert.IsTrue(Common.Numeric.IsNumericType(typeof(decimal?)));
            Assert.IsTrue(Common.Numeric.IsNumericType(typeof(float?)));
            Assert.IsTrue(Common.Numeric.IsNumericType(typeof(double?)));

            Assert.IsFalse(Common.Numeric.IsNumericType(typeof(char)));
            Assert.IsFalse(Common.Numeric.IsNumericType(typeof(char?)));
            Assert.IsFalse(Common.Numeric.IsNumericType(typeof(string)));
            Assert.IsFalse(Common.Numeric.IsNumericType(typeof(object)));
            Assert.IsFalse(Common.Numeric.IsNumericType(typeof(NumericTests)));
        }
    }
}
