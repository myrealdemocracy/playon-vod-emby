using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayOn.Tools.Helper
{
    public class Image
    {
        public static string Mapper(Scaffold.Xml.Item item)
        {
            var image = "";

            if (!String.IsNullOrEmpty(item.Art)) image = item.Art.Replace("tiny", "large");

            if (String.IsNullOrEmpty(image)) image = Default(item.Href);

            return Constant.Url.Base + image;
        }

        public static string Default(string href, bool addBase = false)
        {
            var providerId = href.Split(Convert.ToChar("="))[1];

            if (providerId.Contains("-"))
            {
                providerId = providerId.Split(Convert.ToChar("-"))[0];
            }

            var providerImage = "/images/provider.png?id=" + providerId;

            return (addBase ? Constant.Url.Base : "") + providerImage;
        }
    }
}
