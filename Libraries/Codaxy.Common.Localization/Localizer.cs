using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Reflection;

namespace Codaxy.Common.Localization
{
    public class UnsupportedLocalizationTypeException : Exception { }

    public interface ILocalizationStore
    {
        T Get<T>() where T : new();
        T Get<T>(string typeNameSuffix) where T : new();
        Field[] GetTypeLocalizationData(Type t);
        Field[] GetTypeLocalizationData(Type t, string typeNameSuffix);
        Field[] GetTypeLocalizationData(string typeName, Assembly assembly);
    }

    public interface ILocalizer
    {
        ILocalizationStore GetLocalizationStore(String langCode);
        T Get<T>(String langCode) where T : new();
        T Get<T>(String langCode, string typeNameSuffix) where T : new();
    }

    public class DummyLocalizer : ILocalizer
    {
        DummyLocalizationStore store = new DummyLocalizationStore();

        #region ILocalizer Members

        public ILocalizationStore GetLocalizationStore(string langCode)
        {
            return store;
        }

        public T Get<T>(string langCode) where T : new()
        {
            return GetLocalizationStore(langCode).Get<T>();
        }

        public T Get<T>(string langCode, string typeNameSuffix) where T : new()
        {
            return GetLocalizationStore(langCode).Get<T>(typeNameSuffix);
        }

        #endregion
    }

    public class DummyLocalizationStore : ILocalizationStore
    {
        #region ILocalizationStore Members

        public T Get<T>() where T : new()
        {
            return new T();
        }

        public T Get<T>(string typeNameSuffix) where T : new()
        {
            return new T();
        }

        public Field[] GetTypeLocalizationData(Type t) { return null; }
        public Field[] GetTypeLocalizationData(Type t, string typeNameSuffix) { return null; }
        public Field[] GetTypeLocalizationData(string localizationTypeName, Assembly assembly) { return null; }
                
        #endregion
    }
}