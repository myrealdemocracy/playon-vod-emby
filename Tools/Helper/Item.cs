using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayOn.Tools.Helper
{
    public class Item
    {
        public static List<Scaffold.Video> List(string currentFolder)
        {
            var url = Url.Generate(currentFolder);

            var videos = new List<Scaffold.Video>();

            var items = String.IsNullOrEmpty(currentFolder)
                ? Xml.Items<Scaffold.Xml.Catalog>(url).Items
                : Xml.Items<Scaffold.Xml.Group>(url).Items;

            foreach (var item in items)
            {
                videos.AddRange(Generate(item, items, currentFolder));
            }

            return videos;
        }

        public static List<Scaffold.Video> Generate(Scaffold.Xml.Item item, List<Scaffold.Xml.Item> items, string currentFolder)
        {
            var videos = new List<Scaffold.Video>();

            if (item.Href.Contains("playon") ||
                item.Href.Contains("playmark") ||
                item.Href.Contains("playlater") ||
                item.Name == "Clips" ||
                item.Name == "Video Clips" ||
                item.Name == "Clips & Extras" ||
                item.Name == "Episode Highlights" ||
                item.Name.Contains("This folder contains no content"))

                return videos;

            if (item.Type == "video")
            {   
            }
            else
            {
            }

            return videos;
        }
    }
}
