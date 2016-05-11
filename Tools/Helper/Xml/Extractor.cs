using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using NLog;

namespace PlayOn.Tools.Helper.Xml
{
    public class Extractor
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        protected static MemoryCache Cache = new MemoryCache("PlayOnXML");

        public static T Items<T>(string url) where T : class, new()
        {
            var xml = new T();

            try
            {
                var finalUrl = Constant.Url.Base + url + (url.Contains("?") ? "&" : "?") + "flm=long";

                if (url.Contains("error")) RemoveCache();

                if (Cache[url] != null) return Cache[url] as T;

                xml = DownloadXml<T>(finalUrl);

                if (!url.Contains("searchterm")) Cache.Add(url, xml, DateTimeOffset.Now.AddMinutes(10));
            }
            catch (Exception exception)
            {
                Logger.Error(exception);

                RemoveCache();
            }

            return xml;
        }

        public static T DownloadXml<T>(string url) where T : class, new()
        {
            var xml = new T();

            try
            {
                var client = new WebClient();
                var data = client.DownloadString(url);
                var xmlSerializer = new XmlSerializer(typeof(T));

                xml = xmlSerializer.Deserialize(Stream.GenerateFromString(data)) as T;
            }
            catch (Exception exception)
            {
                Logger.Error(exception);

                RemoveCache();
            }

            return xml;
        }

        public static void RemoveCache()
        {
            foreach (var cache in Cache.Select(s => s.Key))
            {
                Cache.Remove(cache);
            }
        }
    }
}
