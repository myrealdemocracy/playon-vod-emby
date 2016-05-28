using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Controller.Channels;
using MediaBrowser.Model.Channels;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.MediaInfo;

namespace PlayOn.Emby.Helper
{
    public class Movie
    {
        protected static ILogger Logger = Emby.Channel.Logger;

        public static async Task<Scaffold.Result.Channel> Items(InternalChannelItemQuery query, CancellationToken cancellationToken)
        {
            return await Task.Run(async () =>
            {
                var items = new List<ChannelItemInfo>();
                var rest = new Rest.Movie();

                var result = await rest.All(query.StartIndex, query.Limit, cancellationToken);

                foreach (var movie in result.Items)
                {
                    var info = new Scaffold.Movie();

                    if (!movie.Deleted)
                    {
                        info = await Provider.Movie.Info(movie.Name, movie.ImdbId, cancellationToken);
                    }

                    if (movie.Deleted || info.ProviderIds.Count == 0)
                        items.Add(new ChannelItemInfo
                        {
                            Id = "movies|" + movie.ImdbId,
                            Deleted = true
                        });
                    else
                    {
                        var overview = String.IsNullOrEmpty(info.Overview) ? movie.Overview : info.Overview;

                        items.Add(new ChannelItemInfo
                        {
                            Id = "movies|" + movie.ImdbId,
                            Name = movie.Name,
                            Overview = overview,
                            Type = ChannelItemType.Media,
                            ContentType = ChannelMediaContentType.Movie,
                            MediaType = ChannelMediaType.Video,
                            ImageUrl = info.Image,
                            Genres = info.Genres,
                            OfficialRating = info.OfficialRating,
                            ProductionYear = info.ProductionYear,
                            Studios = info.Studios,
                            ProviderIds = info.ProviderIds,
                            DateCreated = info.PremiereDate,
                            RunTimeTicks = info.RunTimeTicks
                        });
                    }
                }

                return new Scaffold.Result.Channel
                {
                    Items = items,
                    TotalRecordCount = result.TotalRecordCount
                };
            }, cancellationToken);
        }
    }
}