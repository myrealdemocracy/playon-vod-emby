using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayOn.Tools.Helper
{
    public class Ignore
    {
        public static bool Item(Scaffold.Xml.Item item)
        {
            return item.Href.Contains("playon") ||
                item.Href.Contains("playmark") ||
                item.Href.Contains("playlater") ||
                item.Name.ToLower() == "clips" ||
                item.Name.ToLower() == "video clips" ||
                item.Name.ToLower() == "clips & extras" ||
                item.Name.ToLower() == "episode highlights" ||
                item.Name.ToLower() == "your history" ||
                item.Name.ToLower() == "your queue" ||
                item.Name.ToLower() == "your subscriptions" ||
                item.Name.ToLower() == "playback options" ||
                item.Name.ToLower() == "suggestions for you" ||
                item.Name.ToLower() == "my list" ||
                item.Name.ToLower() == "recently added" ||
                item.Name.StartsWith("Top Picks for") ||
                item.Name.Contains("This folder contains no content") ||
                String.IsNullOrEmpty(item.Name);
        }
    }
}
