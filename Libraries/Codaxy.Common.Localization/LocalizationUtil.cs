using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Codaxy.Common.Localization
{
    public static class LocalizationUtil
    {
        public static Dictionary<String, String> GetDictionary<T>(T localization)
        {
            var type = typeof(T);
            var stringType = typeof(String);
            Dictionary<String, String> res = new Dictionary<string, string>();
            foreach (var f in type.GetFields())
                if (f.FieldType == stringType)
                {
                    var v = f.GetValue(localization);
                    if (v != null)
                        res.Add(f.Name, (String)v);
                }
            return res;
        }

        static ILocalizer globalLocalizer;

        public static void SetGlobalLocalizer(ILocalizer localizer)
        {
            globalLocalizer = localizer;
        }

        public static T Localize<T>() where T : new()
        {
            if (globalLocalizer == null)
                throw new InvalidOperationException("Global localizer not set. Use LocalizationUtil.SetGlobalLocalizer to define global localizer.");
            return globalLocalizer.Get<T>(Thread.CurrentThread.CurrentCulture.Name);
        }
    }
}
