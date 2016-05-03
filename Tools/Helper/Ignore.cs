using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayOn.Tools.Helper
{
    public class Ignore
    {
        public static bool Item(List<Scaffold.Xml.Item> items, Scaffold.Xml.Item item)
        {
            return item.Href.Contains("playon") ||
                item.Href.Contains("playmark") ||
                item.Href.Contains("playlater") ||
                item.Name.ToLower() == "clips" ||
                item.Name.ToLower() == "video clips" ||
                item.Name.ToLower() == "clips & extras" ||
                item.Name.ToLower() == "episode highlights" ||
                item.Name.ToLower() == "playback options" ||
                item.Name.ToLower() == "backstage & interviews" ||
                item.Name.ToLower() == "just for kids" ||
                item.Name.ToLower() == "family" ||
                item.Name.ToLower() == "kids" ||
                item.Name.ToLower() == "recaps" ||
                item.Name.ToLower() == "previews" ||
                item.Name.ToLower() == "webisodes" ||
                item.Name.ToLower() == "minisodes" ||
                item.Name.ToLower().Contains("s00") ||
                item.Name.ToLower().Contains("e00") ||
                item.Name.ToLower().Contains("this folder contains no content") ||
                (items.Count > 1 && !items.Any(a => a.Name.ToLower() == "shows") && item.Name.ToLower() != "full episodes") ||
                String.IsNullOrEmpty(item.Name);
        }
    }
}
