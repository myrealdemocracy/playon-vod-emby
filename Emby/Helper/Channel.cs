using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaBrowser.Controller.Channels;
using MediaBrowser.Model.Channels;

namespace PlayOn.Emby.Helper
{
    public class Channel
    {
        public static List<ChannelItemInfo> Items(string currentFolder)
        {
            var channelItemInfos = new List<ChannelItemInfo>();

            if (String.IsNullOrEmpty(currentFolder))
            {
                channelItemInfos = new List<ChannelItemInfo>
                {
                    new ChannelItemInfo
                    {
                        Id = "movies",
                        Name = "Movies",
                        Type = ChannelItemType.Folder
                    },
                    new ChannelItemInfo
                    {
                        Id = "series",
                        Name = "TV Shows",
                        Type = ChannelItemType.Folder
                    }
                };
            }
            else
            {
                
            }

            return channelItemInfos;
        }
    }
}
