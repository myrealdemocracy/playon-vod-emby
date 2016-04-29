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

        public static async Task<List<ChannelItemInfo>> Items(string currentFolder, CancellationToken cancellationToken)
        {
            return await Task.Run(async () =>
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
                else if (currentFolder == "movies" || currentFolder == "series")
                {
                    var rest = new Rest.Category();

                    var categories = await rest.All(cancellationToken);

                    foreach (var item in categories.All)
                    {
                        channelItemInfos.Add(new ChannelItemInfo
                        {
                            Id = currentFolder + "|" + item.ToLower(),
                            Name = item,
                            Type = ChannelItemType.Folder
                        });
                    }
                }
                else if (currentFolder.Contains("movies|"))
                {

                }
                else if (currentFolder.Contains("series|"))
                {

                }

                return channelItemInfos;
            }, cancellationToken);
        }
    }
}
