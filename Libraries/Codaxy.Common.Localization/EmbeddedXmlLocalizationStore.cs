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
            cache = new Dictionary<string, object>();
            loadedAssemblies = new HashSet<Assembly>();
            localizationData = new LocalizationData();
        }

        Dictionary<string, object> cache;
        HashSet<Assembly> loadedAssemblies;
        LocalizationData localizationData;

        public T Get<T>() where T : new()
        {
            return Get<T>(string.Empty);
        }

        public T Get<T>(string typeNameSuffix) where T : new()
        {
            var type = typeof(T);
            var cacheKey = type.FullName + typeNameSuffix;
            object cres;
            if (cache.TryGetValue(cacheKey, out cres))
                return (T)cres;

            var res = new T();            

            Field[] fields = GetTypeLocalizationData(type, typeNameSuffix);

            if (fields != null)
                foreach (var f in fields)
                {
                    var finfo = type.GetField(f.FieldName);
                    if (finfo != null)
                        finfo.SetValue(res, f.LocalizedText);
                }


            lock (cache)
            {
                cache[cacheKey] = res;
            }

            return res;
        }

        public Field[] GetTypeLocalizationData(Type type)
        {
            return GetTypeLocalizationData(type.FullName, type.Assembly);
        }

        public Field[] GetTypeLocalizationData(Type type, string typeNameSuffix)
        {
            return GetTypeLocalizationData(type.FullName + typeNameSuffix, type.Assembly);
        }

        public Field[] GetTypeLocalizationData(string typeName, Assembly assembly)
        {         
            Field[] res;
            if (localizationData.TryGetValue(typeName, out res))
                return res;
            LoadAssemblyLocalizationData(assembly);
            if (localizationData.TryGetValue(typeName, out res))
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
