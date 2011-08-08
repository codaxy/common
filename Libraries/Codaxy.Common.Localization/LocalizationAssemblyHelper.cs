using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Codaxy.Common.Reflection;
using System.Xml;

namespace Codaxy.Common.Localization
{
    class LocalizationAssemblyHelper
    {
        public static Type[] GetLocalizationTypesForAssembly(params Assembly[] assemblies)
        {
            List<Type> res = new List<Type>();
            foreach (var assembly in assemblies)
                foreach (var theType in assembly.GetTypes())
                    foreach (var a in theType.GetCustomAttributes(false))
                        if (a is LocalizationAttribute)
                            res.Add(theType);

            return res.ToArray();
        }

        public static IEnumerable<Assembly> GetLocalizedAssemblies()
        {
            return AssemblyHelper.GetAttributedAssemblies(typeof(LocalizationAttribute));            
        }

        public static String[] GetLocalizedAssemblyNames()
        {
            return GetLocalizedAssemblies().Select(a => a.GetName().Name).ToArray();
        }

        public static Assembly GetAssembly(String assemblyName)
        {
            return AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault(a => a.GetName().Name == assemblyName);
        }

        public static String GetAssemblyName(Assembly assembly) { return assembly.GetName().Name; }

        public static LocalizationData GetDefaultLocalizationData(Assembly assembly, IEnumerable<ILocalizationDataProvider> providers)
        {
            LocalizationData res = new LocalizationData();
            foreach (var p in providers)
            {
                var data = p.ReadDefaultData(assembly);
                res.Include(data);
            }
            return res;
        }

        public static void WriteDefaultLocalizationData(String outputDirectoryPath, IEnumerable<ILocalizationDataProvider> providers)
        {
            foreach (var a in LocalizationAssemblyHelper.GetLocalizedAssemblies())
            {
                var data = LocalizationAssemblyHelper.GetDefaultLocalizationData(a, providers);
                using (var xw = new XmlTextWriter(XmlPathUtil.GetDefaultLocalizationFilePath(outputDirectoryPath, a), Encoding.UTF8))
                {
                    xw.Formatting = Formatting.Indented;
                    data.WriteXml(xw);
                }
            }
        }
    }
}
