using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayOn.Tools.Helper
{
    public class Image
    {
        public static string Mapper(Scaffold.Item item)
        {
            var image = "";

            if (!String.IsNullOrEmpty(item.Art)) image = item.Art.Replace("tiny", "large");

            if (String.IsNullOrEmpty(image)) image = Default(item.Href);

            return Url.Base + image;
        }

        public static string Default(string href, bool addBase = false)
        {
            var providerId = Id.Mapper(href);
            var providerImage = "/images/provider.png?id=" + providerId;

            return (addBase ? Url.Base : "") + providerImage;
        }
    }
}
