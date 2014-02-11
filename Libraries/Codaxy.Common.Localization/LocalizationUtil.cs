using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Codaxy.Common.Localization
{
    public static class LocalizationUtil
    {
        public static Dictionary<String, String> GetDictionary(object localization)
        {
            var type = localization.GetType();
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

        static ILocalizer globalLocalizer = new EmbeddedXmlLocalizer();
        public static void SetGlobalLocalizer(ILocalizer localizer)
        {
            if (localizer == null)
                throw new ArgumentNullException("localizer");
            globalLocalizer = localizer;
        }

        public static ILocalizationStore CurrentCulture { get { return Localizer.GetLocalizationStore(Thread.CurrentThread.CurrentCulture.Name); } }
        public static ILocalizationStore CurrentUICulture { get { return Localizer.GetLocalizationStore(Thread.CurrentThread.CurrentUICulture.Name); } }

        public static ILocalizer Localizer { get { return globalLocalizer; } set { SetGlobalLocalizer(value); } }

        public static T Localize<T>() where T : new()
        {
            return CurrentCulture.Get<T>();
        }

        public static T LocalizeUI<T>() where T : new()
        {
            return CurrentUICulture.Get<T>();
        }        

        public static void WriteLocalizationFiles(String outputDirectoryPath, IEnumerable<ILocalizationDataProvider> providers)
        {
            LocalizationAssemblyHelper.WriteDefaultLocalizationData(outputDirectoryPath, providers);
        }
    }
}
