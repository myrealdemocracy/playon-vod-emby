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
                            Type = ChannelItemType.Folder,
                            ImageUrl = "http://playon.local/img/movies.png"
                        },
                        new ChannelItemInfo
                        {
                            Id = "series",
                            Name = "TV Shows",
                            Type = ChannelItemType.Folder,
                            ImageUrl = "http://playon.local/img/series.png"
                        },
                        //new ChannelItemInfo
                        //{
                        //    Id = "categories",
                        //    Name = "Categories",
                        //    Type = ChannelItemType.Folder
                        //}
                    };
                }
                else if (currentFolder == "movies" || currentFolder.Contains("movies|"))
                    channelItemInfos = await Movie.Items(currentFolder, cancellationToken);
                else if (currentFolder == "series" || currentFolder.Contains("series|"))
                    channelItemInfos = await Series.Items(currentFolder, cancellationToken);
                else if (currentFolder == "categories" || currentFolder.Contains("categories|"))
                    channelItemInfos = await Category.Items(currentFolder, cancellationToken);

                return channelItemInfos;
            }, cancellationToken);
        }
    }
}
