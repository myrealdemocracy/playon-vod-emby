using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PlayOn.Tools.Helper.Xml
{
    public class Extractor
    {
        protected static MemoryCache Cache = new MemoryCache("PlayOnXML");

        public static T Items<T>(string url) where T : class, new()
        {
            var finalUrl = Url.Base + url + (url.Contains("?") ? "&" : "?") + "flm=long";

            if (url.Contains("error")) return new T();

            if (Cache[url] != null) return Cache[url] as T;

            var client = new WebClient();

            var data = client.DownloadString(finalUrl);

            var xmlSerializer = new XmlSerializer(typeof (T));

            var xml = xmlSerializer.Deserialize(Stream.GenerateFromString(data)) as T;

            if (xml == null) return new T();

            Cache.Add(url, xml, DateTimeOffset.Now.AddDays(1));

            return xml;
        }
    }
}
