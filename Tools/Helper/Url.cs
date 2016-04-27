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
        public const string Video = "http://playon.local/video?id=";
        public const string Image = "http://playon.local/image?id=";

        public static string Generate(string query = null)
        {
            var url = Xml;

            if (!String.IsNullOrEmpty(query))
            {
                query = Clean(query);

                var terms = query.Split(Convert.ToChar("|"));

                url += "?id=" + terms[0];

                if (terms.Length > 0)
                {
                    query = query.Replace(terms[0] + "|", "");

                    query = query.Length > 1 ? query.Substring(0, query.Length - 1) : query;

                    url = Find(url, query);
                }
            }

            return url;
        }

        public static string Find(string url, string query)
        {
            var count = 0;
            var items = Helper.Xml.Items<Scaffold.Group>(url).Items;
            var subItem = new Scaffold.Item();

            foreach (var item in query.Split(Convert.ToChar("|")))
            {
                switch (item)
                {
                    case "video":
                        url = Base + "/" + Helper.Xml.Items<Scaffold.Video>(url).Item.Src;
                        break;
                    case "image":
                        url = count == 0 ? Helper.Image.Default(url, true) : Helper.Image.Mapper(subItem);
                        break;
                    default:
                        if (count > 0) items = Helper.Xml.Items<Scaffold.Group>(url).Items;

                        subItem = items.FirstOrDefault(q => String.Equals(Clean(q.Name), item, StringComparison.CurrentCultureIgnoreCase));

                        if (subItem != null) url = subItem.Href;
                        break;
                }

                count++;
            }

            return url;
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
