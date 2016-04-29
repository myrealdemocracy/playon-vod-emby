using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Controller.Channels;
using MediaBrowser.Model.Channels;
using MediaBrowser.Model.Logging;

namespace PlayOn.Emby.Helper
{
    public class Channel
    {
        protected static ILogger Logger = Emby.Channel.Logger;

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
            else if(currentFolder == "movies" || currentFolder == "series")
            {
                var categories = Task.Run(async () =>
                {
                    var rest = new Rest.Category();

                    return await rest.All(Emby.Channel.CancellationToken);
                });

                foreach (var item in categories.Result.All)
                {
                    channelItemInfos.Add(new ChannelItemInfo
                    {
                        Id = currentFolder + "|" + item.Key.ToLower(),
                        Name = item.Key,
                        Type = ChannelItemType.Folder
                    });
                }
            }

            return channelItemInfos;
        }
    }
}
