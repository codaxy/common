using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Reflection;
using Codaxy.Common.Logging;
using System.Collections.Concurrent;

namespace Codaxy.Common.Localization
{
    public class UnsupportedLanguageException : Exception  {}

    public class XmlLocalizer : ILocalizer
    {
        public XmlLocalizer(String path, string[] langCodes, params ILocalizationDataProvider[] specialProviders)
        {
            Path = path + (path.EndsWith("\\") ? "" : "\\");
            LangCodes = langCodes;
            providers = new List<ILocalizationDataProvider>();
            providers.Add(new DefaultLocalizationDataProvider());
            providers.AddRange(specialProviders);
        }

        public String Path { get; private set; }
        public String[] LangCodes { get; private set; }

        List<ILocalizationDataProvider> providers;

        public Logger Logger { get; set; }

        public ILocalizationStore GetLocalizationStore(string langCode)
        {
            return GetXmlLocalizationStore(langCode);
        }

        public XmlLocalizationStore GetXmlLocalizationStore(string langCode)
        {
            if (String.IsNullOrEmpty(langCode))
                throw new UnsupportedLanguageException();


            XmlLocalizationStore store;
            if (cache.TryGetValue(langCode, out store))
                return store;

            if (!LangCodes.Contains(langCode))
                throw new UnsupportedLanguageException();

            store = new XmlLocalizationStore(langCode, Path, providers)
            {
                Logger = Logger
            };

            lock (cache)
            {                
                cache[langCode] = store;
            }

            return store;
        }

        Dictionary<String, XmlLocalizationStore> cache = new Dictionary<string, XmlLocalizationStore>();

        public T Get<T>(string langCode) where T : new()
        {
            return GetLocalizationStore(langCode).Get<T>();
        }        

        public String[] GetLocalizedAssemblyNames()
        {
            return LocalizationAssemblyHelper.GetLocalizedAssemblyNames();      
        }

        public void WriteDefaultLocalizationData()
        {
            foreach (var a in LocalizationAssemblyHelper.GetLocalizedAssemblies())
            {
                var data = LocalizationAssemblyHelper.GetDefaultLocalizationData(a, providers);
                using (var xw = new XmlTextWriter(XmlPathUtil.GetDefaultLocalizationFilePath(Path, a), Encoding.UTF8))
                {
                    xw.Formatting = Formatting.Indented;
                    data.WriteXml(xw);
                }
            }
        }
    }

    

    
}
