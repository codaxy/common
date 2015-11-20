using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Codaxy.Common.Reflection;
using System.Xml;
using System.Diagnostics;

namespace Codaxy.Common.Localization
{
    class EmbeddedXmlLocalizationStore : ILocalizationStore
    {
        string LangCode { get; set; }

        public EmbeddedXmlLocalizationStore(String langCode)
        {
            LangCode = langCode;
            cache = new Dictionary<Type, object>();
            loadedAssemblies = new HashSet<Assembly>();
            localizationData = new LocalizationData();
        }

        Dictionary<Type, object> cache;
        HashSet<Assembly> loadedAssemblies;
        LocalizationData localizationData;

        public T Get<T>() where T : new()
        {
            var type = typeof(T);
            object cres;
            if (cache.TryGetValue(type, out cres))
                return (T)cres;

            var res = new T();
            var locName = type.FullName;

            Field[] fields = GetTypeLocalizationData(type);

            if (fields != null)
                foreach (var f in fields)
                {
                    var finfo = type.GetField(f.FieldName);
                    if (finfo != null)
                        finfo.SetValue(res, f.LocalizedText);
                }


            lock (cache)
            {
                cache[type] = res;
            }

            return res;
        }

        public Field[] GetTypeLocalizationData(Type type)
        {
            var locName = type.FullName;
            Field[] res;
            if (localizationData.TryGetValue(locName, out res))
                return res;
            LoadAssemblyLocalizationData(type.Assembly);
            if (localizationData.TryGetValue(locName, out res))
                return res;
            return null;
        }

        void LoadAssemblyLocalizationData(Assembly assembly)
        {
            if (loadedAssemblies.Contains(assembly))
                return;
            var assemblyLocData = GetLocalizationData(assembly);
            localizationData.Include(assemblyLocData);
            loadedAssemblies.Add(assembly);
        }

        public LocalizationData GetLocalizationData(Assembly assembly)
        {
            var embeddedPath = String.Format(@"Localization\{0}.xml", LangCode);
            try
            {
                using (var fs = AssemblyHelper.ReadEmbeddedFile(assembly, embeddedPath))
                {
                    if (fs == null)
                        return new LocalizationData();
                    using (var xr = new XmlTextReader(fs))
                    {
                        var data = LocalizationData.ReadXml(xr);
                        Debug.WriteLine("Embedded Localization '{1}' for assembly '{0}' successfully loaded.", assembly, LangCode);
                        return data;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Read Xml localization data error: {0}", ex);
            }
            return new LocalizationData();
        }
    }
}
