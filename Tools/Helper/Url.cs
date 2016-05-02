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

            Logger.Debug("baseUrl:" + baseUrl);

            return baseUrl;
        }

        public static string Find(string baseUrl, string path)
        {
            var count = 0;
            var items = Xml.Extractor.Items<Scaffold.Xml.Group>(baseUrl).Items;
            var subItem = new Scaffold.Xml.Item();
            var terms = path.Split(Convert.ToChar("|"));

            Logger.Debug("path:" + path);

            foreach (var term in terms)
            {
                Logger.Debug("count:" + count);
                Logger.Debug("term:" + term);

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

                        subItem = items.FirstOrDefault(q => String.Equals(q.Name, term, StringComparison.CurrentCultureIgnoreCase));

                        if (subItem != null)
                        {
                            baseUrl = subItem.Href;

                            Logger.Debug("subItem.Href:" + subItem.Href);
                        }
                        break;
                }

                count++;
            }

            Logger.Debug("baseUrl:" + baseUrl);

            return baseUrl;
        }
    }
}
