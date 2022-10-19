using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Codaxy.Common
{
    public static class Numeric
    {
        private static HashSet<Type> numericTypes = new HashSet<Type>
        {
            typeof(byte), typeof(sbyte), typeof(short), typeof(ushort),
            typeof(int), typeof(uint), typeof(long), typeof(ulong),
            typeof(float), typeof(double), typeof(decimal)
        };

        public static bool IsNumericType(Type type)
        {
            return numericTypes.Contains(type) ||
                numericTypes.Contains(Nullable.GetUnderlyingType(type));
        }
    }
}
