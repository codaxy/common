using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Codaxy.Common.Localization
{
    class EmbeddedXmlLocalizer : ILocalizer
    {
        public EmbeddedXmlLocalizer()
        {
            
        }

        public ILocalizationStore GetLocalizationStore(string langCode)
        {
            if (String.IsNullOrEmpty(langCode))
                throw new UnsupportedLanguageException();

            ILocalizationStore store;
            if (cache.TryGetValue(langCode, out store))
                return store;

            store = new EmbeddedXmlLocalizationStore(langCode);

            lock (cache)
            {
                cache[langCode] = store;
            }

            return store;
        }

        Dictionary<String, ILocalizationStore> cache = new Dictionary<string, ILocalizationStore>();

        public T Get<T>(string langCode) where T : new()
        {
            return GetLocalizationStore(langCode).Get<T>();
        }
    }
}
