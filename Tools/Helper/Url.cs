using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace PlayOn.Tools.Helper
{
    public class Url
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static Dictionary<string, string> Videos = new Dictionary<string, string>();  

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
            var items = Xml.Extractor.Items<Scaffold.Xml.Group>(url).Items;
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
                            url = Constant.Url.Base + "/" + Xml.Extractor.Items<Scaffold.Xml.Video>(url).Item.Src;
                            break;
                        case "image":
                            url = count == 0 ? Image.Default(url, true) : Image.Mapper(subItem);
                            break;
                        default:
                            if (count > 0) items = Xml.Extractor.Items<Scaffold.Xml.Group>(url).Items;

                            subItem = null;

                            foreach (var item in items)
                            {
                                Logger.Debug("item.Name:" + item.Name);
                                Logger.Debug("item.Href:" + item.Href);

                                if (!String.Equals(item.Name, term, StringComparison.InvariantCultureIgnoreCase)) continue;

                                subItem = item;
                                url = subItem.Href;
                            }

                            if(subItem == null) throw new Exception("Can't find term " + term);
                            break;
                    }

                    Logger.Debug("baseUrl:" + url);

                    count++;
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }

            Logger.Debug("return baseUrl:" + url);

            return url;
        }

        public static string Search(string provider, string name, string series = null)
        {
            var url = "";
            var searchTerm = (String.IsNullOrEmpty(series) ? name : series).ToLower();
            var searchUrl = Constant.Url.Xml + "?id=" + provider + "&searchterm=" + WebUtility.UrlEncode(searchTerm);

            try
            {
                Logger.Debug("searchUrl: " + searchUrl);

                var items = Xml.Extractor.Items<Scaffold.Xml.Group>(searchUrl).Items;

                SearchItems(items, name);

                url = Videos.FirstOrDefault(q => q.Key == name).Value;
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }

            return url;
        }

        public static void SearchItems(List<Scaffold.Xml.Item> items, string name)
        {
            try
            {
                foreach (var item in items)
                {
                    Logger.Debug("item.Name:" + item.Name);
                    Logger.Debug("item.Href:" + item.Href);

                    if (Videos.Any(a => a.Key == name)) break;

                    if (item.Type == "folder")
                        SearchItems(Xml.Extractor.Items<Scaffold.Xml.Group>(item.Href).Items, name);

                    if (!String.Equals(item.Name, name, StringComparison.InvariantCultureIgnoreCase)) continue;

                    var url = Constant.Url.Base + "/" + Xml.Extractor.Items<Scaffold.Xml.Video>(item.Href).Item.Src;

                    Videos.Add(name, url);

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
