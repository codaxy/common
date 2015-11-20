using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Codaxy.Common.Logging;
using System.IO;
using System.Reflection;

namespace Codaxy.Common.Localization
{
    public class XmlLocalizationStore : ILocalizationStore
    {
        public XmlLocalizationStore(String langCode, String path, IEnumerable<ILocalizationDataProvider> providers)
        {
            Path = path;
            LangCode = langCode;
            cache = new Dictionary<string, object>();
            loadedAssemblies = new HashSet<string>();
            localizationData = new LocalizationData();
            this.providers = providers;
        }

        IEnumerable<ILocalizationDataProvider> providers;

        public String Path { get; private set; }
        public String LangCode { get; private set; }
        public Logger Logger { get; set; }

        String GetXmlFilePath(String assemblyName)
        {
            return XmlPathUtil.GetLocalizationFilePath(Path, assemblyName, LangCode);
        }        

        LocalizationData ReadAssemblyLocalization(String assemblyName)
        {
            var xmlPath = GetXmlFilePath(assemblyName);
            if (System.IO.File.Exists(xmlPath))
            {
                try
                {
                    using (var fs = new FileStream(xmlPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        using (var xr = new XmlTextReader(fs))
                        {
                            var data = LocalizationData.ReadXml(xr);
                            Logger.InfoFormat("Localization '{1}' for assembly '{0}' successfully loaded.", assemblyName, LangCode);
                            return data;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Exception("Read Xml localization data error.", ex);
                }
            }
            return new LocalizationData();
        }

        Dictionary<string, object> cache;
        HashSet<String> loadedAssemblies;
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
            return GetTypeLocalizationData(type, string.Empty);
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
            var assemblyName = LocalizationAssemblyHelper.GetAssemblyName(assembly);
            if (loadedAssemblies.Contains(assemblyName))
                return;
            var assemblyLocData = GetLocalizationData(assembly);
            localizationData.Include(assemblyLocData);
            loadedAssemblies.Add(assemblyName);
        }

        public LocalizationData GetLocalizationData(Assembly assembly)
        {
            var assemblyLocData = LocalizationAssemblyHelper.GetDefaultLocalizationData(assembly, providers);
            var overrideData = ReadAssemblyLocalization(LocalizationAssemblyHelper.GetAssemblyName(assembly));
            assemblyLocData.Override(overrideData);
            return assemblyLocData;
        }

        public LocalizationData GetLocalizationData(String assemblyName)
        {
            var assembly = LocalizationAssemblyHelper.GetAssembly(assemblyName);
            return GetLocalizationData(assembly);
        }

        public void SetLocalizationData(String assemblyName, LocalizationData data)
        {
            using (var xw = new XmlTextWriter(GetXmlFilePath(assemblyName), Encoding.UTF8))
            {
                xw.Formatting = Formatting.Indented;
                data.WriteXml(xw);
            }
        }

        public void RemoveLocalizationData(String assemblyName)
        {
            var filePath = GetXmlFilePath(assemblyName);
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);
        }
    }
}
