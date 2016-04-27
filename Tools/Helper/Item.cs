using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaBrowser.Controller.Channels;

namespace PlayOn.Tools.Helper
{
    public class Item
    {
        public static List<ChannelItemInfo> List(string currentFolder)
        {
            var url = Url.Generate(currentFolder);

            var channelItemInfos = new List<ChannelItemInfo>();

            var items = String.IsNullOrEmpty(currentFolder)
                ? Xml.Items<Scaffold.Catalog>(url).Items
                : Xml.Items<Scaffold.Group>(url).Items;

            foreach (var item in items)
            {
                channelItemInfos.AddRange(Generate(item, items, currentFolder));
            }

            return channelItemInfos;
        }

        public static List<ChannelItemInfo> Generate(Scaffold.Item item, List<Scaffold.Item> items, string currentFolder)
        {
            var channelItemInfos = new List<ChannelItemInfo>();
            var hasClips = items.Any(a => a.Name == "Clips" || a.Name == "Video Clips" || a.Name == "Clips & Extras" || a.Name == "Episode Highlights");

            if (item.Href.Contains("playon") ||
                item.Href.Contains("playmark") ||
                item.Href.Contains("playlater") ||
                item.Name == "Clips" ||
                item.Name == "Video Clips" ||
                item.Name == "Clips & Extras" ||
                item.Name == "Episode Highlights" ||
                item.Name.Contains("This folder contains no content"))

                return channelItemInfos;

            if (item.Type == "video")
            {
                channelItemInfos.Add(Video.Generate(item, currentFolder));
            }
            else
            {
                if ((item.Name != "Clips" && item.Name != "Video Clips" && item.Name != "Clips & Extras" && item.Name != "Episode Highlights" && hasClips && items.Count <= 4) ||
                    (item.Name != "Clips" && item.Name != "Video Clips" && item.Name != "Clips & Extras" && item.Name != "Episode Highlights" && items.Count <= 1))
                {
                    foreach (var subItem in Xml.Items<Scaffold.Group>(item.Href).Items)
                    {
                        if (subItem.Name != "Clips" &&
                            subItem.Name != "Video Clips" &&
                            subItem.Name != "Clips & Extras" &&
                            subItem.Name != "Episode Highlights" &&
                            !subItem.Name.Contains("This folder contains no content"))
                        {
                            var parentFolder = item.Name.ToLower() + "|";

                            channelItemInfos.Add(subItem.Type == "video"
                                ? Video.Generate(subItem, currentFolder, parentFolder)
                                : Folder.Mapper(subItem, currentFolder, parentFolder));
                        }
                    }
                }
                else
                {
                    channelItemInfos.Add(Folder.Mapper(item, currentFolder));
                }
            }

            return channelItemInfos;
        }
    }
}
