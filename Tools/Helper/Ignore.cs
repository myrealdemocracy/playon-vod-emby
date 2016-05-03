using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayOn.Tools.Helper
{
    public class Ignore
    {
        public static bool Item(List<Scaffold.Xml.Item> items, Scaffold.Xml.Item item, string name = null)
        {
            var itemsCount = items.Count;
            var itemName = item.Name.ToLower();

            if (!String.IsNullOrEmpty(name)) itemName = name.ToLower();

            return item.Href.Contains("playon") ||
                item.Href.Contains("playmark") ||
                item.Href.Contains("playlater") ||
                itemName == "clips" ||
                itemName == "video clips" ||
                itemName == "clips & extras" ||
                itemName == "episode highlights" ||
                itemName == "playback options" ||
                itemName == "backstage & interviews" ||
                itemName == "just for kids" ||
                itemName == "family" ||
                itemName == "kids" ||
                itemName == "recaps" ||
                itemName == "previews" ||
                itemName == "webisodes" ||
                itemName == "minisodes" ||
                itemName.Contains("s00") ||
                itemName.Contains("e00") ||
                itemName.Contains("this folder contains no content") ||
                (itemsCount > 1 && !items.Any(a => a.Name.ToLower() == "shows") && itemName != "full episodes") ||
                String.IsNullOrEmpty(itemName);
        }
    }
}
