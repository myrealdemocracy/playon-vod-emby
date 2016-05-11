using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace PlayOn.Tools.Helper
{
    public class Url
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        protected static MemoryCache Cache = new MemoryCache("PlayOnXML");

        public static string Generate(string path = null)
        {
            var url = Constant.Url.Xml;

            Logger.Debug("path:" + path);

            try
            {
                if (!String.IsNullOrEmpty(path))
                {
                    var terms = path.Split(Convert.ToChar("|"));

                    url += "?id=" + terms[0];

                    if (terms.Length > 0)
                    {
                        path = path.Replace(terms[0] + "|", "");

                        path = path.Length > 1 ? path.Substring(0, path.Length - 1) : path;

                        url = Find(url, path);
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }

            Logger.Debug("url:" + url);

            return url;
        }

        public static string Find(string url, string path)
        {
            Scaffold.Xml.Item subItem = null;
            var count = 0;
            var items = Xml.Extractor.Items<Scaffold.Xml.Group>(url, true).Items;
            var terms = path.Split(Convert.ToChar("|"));

            try
            {
                foreach (var term in terms)
                {
                    Logger.Debug("count:" + count);
                    Logger.Debug("term:" + term);

                    //if(Ignore.Item(new List<Scaffold.Xml.Item>(), new Scaffold.Xml.Item(), term)) continue;

                    switch (term)
                    {
                        case "video":
                            url = Constant.Url.Base + "/" + Xml.Extractor.Items<Scaffold.Xml.Video>(url, true).Item.Src;
                            break;
                        case "image":
                            url = count == 0 ? Image.Default(url, true) : Image.Mapper(subItem);
                            break;
                        default:
                            if (count > 0) items = Xml.Extractor.Items<Scaffold.Xml.Group>(url, true).Items;

                            subItem = null;

                            foreach (var item in items)
                            {
                                Logger.Debug("item: " + item.Name + " - " + item.Href);

                                if (!String.Equals(item.Name, term, StringComparison.InvariantCultureIgnoreCase)) continue;

                                subItem = item;
                                url = subItem.Href;
                            }

                            if(subItem == null) throw new Exception("Can't find term " + term);
                            break;
                    }

                    Logger.Debug("url:" + url);

                    count++;
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }

            Logger.Debug("return url:" + url);

            return url;
        }

        public static string Search(string provider, string name, string series = null)
        {
            var url = "";
            var type = String.IsNullOrEmpty(series) ? "movie-" : "series-" + series + "-";
            var searchTerm = (String.IsNullOrEmpty(series) ? name : series).ToLower();
            var searchUrl = Constant.Url.Xml + "?id=" + provider + "&searchterm=" + WebUtility.UrlEncode(searchTerm);
            var key = type + name;

            Logger.Debug("key: " + key);

            try
            {
                Logger.Debug("searchUrl: " + searchUrl);

                var items = Xml.Extractor.Items<Scaffold.Xml.Group>(searchUrl).Items;

                SearchItems(items, name, type);

                url = Cache[key].ToString();
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }

            return url;
        }

        public static void SearchItems(List<Scaffold.Xml.Item> items, string name, string type)
        {
            var key = type + name;

            try
            {
                foreach (var item in items)
                {
                    Logger.Debug("item: " + item.Name + " - " + item.Href);

                    if (Cache[key] != null) break;

                    if (item.Type == "folder")
                        SearchItems(Xml.Extractor.Items<Scaffold.Xml.Group>(item.Href).Items, name, type);

                    if (!String.Equals(item.Name, name, StringComparison.InvariantCultureIgnoreCase)) continue;

                    var url = Constant.Url.Base + "/" + Xml.Extractor.Items<Scaffold.Xml.Video>(item.Href).Item.Src;

                    Cache.Add(key, url, DateTimeOffset.Now.AddMinutes(1));

                    break;
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        }
    }
}
