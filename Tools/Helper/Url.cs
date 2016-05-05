using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace PlayOn.Tools.Helper
{
    public class Url
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static string Generate(string path = null)
        {
            var baseUrl = Constant.Url.Xml;

            Logger.Debug("path:" + path);

            try
            {
                if (!String.IsNullOrEmpty(path))
                {
                    var terms = path.Split(Convert.ToChar("|"));

                    baseUrl += "?id=" + terms[0];

                    if (terms.Length > 0)
                    {
                        path = path.Replace(terms[0] + "|", "");

                        path = path.Length > 1 ? path.Substring(0, path.Length - 1) : path;

                        baseUrl = Find(baseUrl, path);
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }

            Logger.Debug("baseUrl:" + baseUrl);

            return baseUrl;
        }

        public static string Find(string baseUrl, string path)
        {
            Scaffold.Xml.Item subItem = null;
            var count = 0;
            var items = Xml.Extractor.Items<Scaffold.Xml.Group>(baseUrl).Items;
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
                            baseUrl = Constant.Url.Base + "/" + Xml.Extractor.Items<Scaffold.Xml.Video>(baseUrl).Item.Src;
                            break;
                        case "image":
                            baseUrl = count == 0 ? Image.Default(baseUrl, true) : Image.Mapper(subItem);
                            break;
                        default:
                            if (count > 0) items = Xml.Extractor.Items<Scaffold.Xml.Group>(baseUrl).Items;

                            subItem = null;

                            foreach (var item in items)
                            {
                                Logger.Debug("item.Name:" + item.Name);
                                Logger.Debug("item.Href:" + item.Href);

                                if (!String.Equals(item.Name, term, StringComparison.InvariantCultureIgnoreCase)) continue;

                                subItem = item;
                                baseUrl = subItem.Href;
                            }

                            if(subItem == null) throw new Exception("Can't find term " + term);
                            break;
                    }

                    Logger.Debug("baseUrl:" + baseUrl);

                    count++;
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }

            Logger.Debug("return baseUrl:" + baseUrl);

            return baseUrl;
        }
    }
}
