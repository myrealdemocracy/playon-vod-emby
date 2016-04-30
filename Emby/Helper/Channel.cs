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
                else if (currentFolder == "movies" || currentFolder.Contains("movies|"))
                {
                    if (currentFolder == "movies")
                    {
                        var rest = new Rest.Movie();

                        var movies = await rest.All(cancellationToken);

                        foreach (var item in movies)
                        {
                            channelItemInfos.Add(new ChannelItemInfo
                            {
                                Id = "movies|" + item.Name.ToLower(),
                                Name = item.Name,
                                Type = ChannelItemType.Folder
                            });
                        }
                    }
                    else
                    {

                    }
                }
                else if (currentFolder == "series" || currentFolder.Contains("series|"))
                {
                    if (currentFolder == "series")
                    {
                        var rest = new Rest.Series();

                        var series = await rest.All(cancellationToken);

                        foreach (var item in series)
                        {
                            channelItemInfos.Add(new ChannelItemInfo
                            {
                                Id = "series|" + item.Name.ToLower(),
                                Name = item.Name,
                                Type = ChannelItemType.Folder
                            });
                        }
                    }
                    else
                    {
                        
                    }
                }

                return channelItemInfos;
            }, cancellationToken);
        }
    }
}
