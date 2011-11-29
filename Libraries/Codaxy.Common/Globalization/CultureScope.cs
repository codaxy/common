using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Threading;

namespace Codaxy.Common.Globalization
{
    public class CultureScope : IDisposable
    {
        CultureInfo initialCulture;
        public CultureScope(CultureInfo culture)
        {
            initialCulture = Thread.CurrentThread.CurrentCulture;
            SetCulture(culture);
        }

        public CultureScope(String name) : this(new CultureInfo(name)) { }

        public void Dispose()
        {
            SetCulture(initialCulture);
        }

        private void SetCulture(CultureInfo culture)
        {
            Thread.CurrentThread.CurrentCulture = culture;
        }
    }

    public class UICultureScope : IDisposable
    {
        CultureInfo initialCulture;
        public UICultureScope(CultureInfo culture)
        {
            initialCulture = Thread.CurrentThread.CurrentUICulture;
            SetCulture(culture);
        }

        public UICultureScope(String name) : this(new CultureInfo(name)) { }

        public void Dispose()
        {
            SetCulture(initialCulture);
        }

        private void SetCulture(CultureInfo culture)
        {
            Thread.CurrentThread.CurrentUICulture = culture;
        }
    }
}
