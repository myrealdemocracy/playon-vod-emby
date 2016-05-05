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

        public static async Task<Scaffold.ChannelList> Items(InternalChannelItemQuery query, CancellationToken cancellationToken)
        {
            return await Task.Run(async () =>
            {
                var channelList = new Scaffold.ChannelList();

                if (String.IsNullOrEmpty(query.FolderId))
                {
                    channelList = new Scaffold.ChannelList
                    {
                        ChannelItemInfos = new List<ChannelItemInfo>
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
                        },
                        TotalRecordCount = 2
                    };
                }
                else if (query.FolderId == "movies" || query.FolderId.Contains("movies|"))
                    channelList = await Movie.Items(query, cancellationToken);
                else if (query.FolderId == "series" || query.FolderId.Contains("series|"))
                    channelList = await Series.Items(query, cancellationToken);
                //else if (query.FolderId == "categories" || query.FolderId.Contains("categories|"))
                //    channelList = await Category.Items(query, cancellationToken);

                return channelList;
            }, cancellationToken);
        }
    }
}
