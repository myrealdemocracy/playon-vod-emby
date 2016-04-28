using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayOn.Tools.Helper
{
    public class Url
    {
        public const string Base = "http://192.168.3.200:54479";
        public const string Xml = "/data/data.xml";
        public const string Video = "http://playon.local/url/video?id=";
        public const string Image = "http://playon.local/url/image?id=";

        public static string Generate(string path = null)
        {
            var baseUrl = Xml;

            if (!String.IsNullOrEmpty(path))
            {
                path = Clean(path);

                var terms = path.Split(Convert.ToChar("|"));

                baseUrl += "?id=" + terms[0];

                if (terms.Length > 0)
                {
                    path = path.Replace(terms[0] + "|", "");

                    path = path.Length > 1 ? path.Substring(0, path.Length - 1) : path;

                    baseUrl = Find(baseUrl, path);
                }
            }

            return baseUrl;
        }

        public static string Find(string baseUrl, string path)
        {
            var count = 0;
            var items = Helper.Xml.Items<Scaffold.Xml.Group>(baseUrl).Items;
            var subItem = new Scaffold.Xml.Item();
            var terms = path.Split(Convert.ToChar("|"));

            foreach (var term in terms)
            {
                switch (term)
                {
                    case "video":
                        baseUrl = Base + "/" + Helper.Xml.Items<Scaffold.Xml.Video>(baseUrl).Item.Src;
                        break;
                    case "image":
                        baseUrl = count == 0 ? Helper.Image.Default(baseUrl, true) : Helper.Image.Mapper(subItem);
                        break;
                    default:
                        if (count > 0) items = Helper.Xml.Items<Scaffold.Xml.Group>(baseUrl).Items;

                        subItem = items.FirstOrDefault(q => String.Equals(Clean(q.Name), term, StringComparison.CurrentCultureIgnoreCase));

                        if (subItem != null) baseUrl = subItem.Href;
                        break;
                }

                count++;
            }

            return baseUrl;
        }

        public static string Clean(string url)
        {
            url = url.Replace(" ", "+");
            url = url.Replace("-", "+");
            url = url.Replace(":", "+");
            url = url.Replace("?", "");
            url = url.Replace("(", "+");
            url = url.Replace(")", "+");
            url = url.Replace("\"", "+");
            url = url.Replace("'", "+");
            url = url.Replace("@", "+");
            url = url.Replace("#", "");

            return url;
        }
    }
}
