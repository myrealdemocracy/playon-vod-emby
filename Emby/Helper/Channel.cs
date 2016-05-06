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

        public static async Task<ChannelItemResult> Items(InternalChannelItemQuery query, CancellationToken cancellationToken)
        {
            return await Task.Run(async () =>
            {
                var channelResult = new Scaffold.Result.Channel();

                if (String.IsNullOrEmpty(query.FolderId))
                {
                    channelResult = new Scaffold.Result.Channel
                    {
                        Items = new List<ChannelItemInfo>
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
                            }
                        },
                        TotalRecordCount = 2
                    };
                }
                else if (query.FolderId == "movies" || query.FolderId.Contains("movies|"))
                    channelResult = await Movie.Items(query, cancellationToken);
                else if (query.FolderId == "series" || query.FolderId.Contains("series|"))
                    channelResult = await Series.Items(query, cancellationToken);

                return new ChannelItemResult
                {
                    Items = channelResult.Items.ToList(),
                    TotalRecordCount = channelResult.TotalRecordCount
                };
            }, cancellationToken);
        }
    }
}
