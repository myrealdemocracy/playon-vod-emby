using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MediaBrowser.Controller.Channels;
using MediaBrowser.Model.Channels;

namespace PlayOn.Tools.Helper
{
    public class Folder
    {
        public static ChannelItemInfo Mapper(Scaffold.Item item, string currentFolder, string parentFolder = "")
        {
            var providerId = Id.Mapper(item.Href);

            var id = Url.Clean(currentFolder + parentFolder + (item.Href.Contains("-") ? item.Name : providerId).ToLower() + "|");

            return new ChannelItemInfo
            {
                Id = id,
                Name = item.Name,
                Type = ChannelItemType.Folder,
                MediaType = ChannelMediaType.Video,
                ImageUrl = Url.Image + WebUtility.UrlEncode(id)
            };
        }
    }
}
