using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Controller.Channels;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Channels;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.MediaInfo;
using MediaBrowser.Providers.TV;

namespace PlayOn.Emby.Helper
{
    public class Series
    {
        protected static ILogger Logger = Emby.Channel.Logger;

        public static async Task<Scaffold.Result.Channel> Items(InternalChannelItemQuery query, CancellationToken cancellationToken)
        {
            return await Task.Run(async () =>
            {
                var rest = new Rest.Series();
                var items = new List<ChannelItemInfo>();
                var totalRecordCount = 0;

                if (query.FolderId == "series")
                {
                    var result = await rest.All(query.StartIndex, query.Limit, cancellationToken);

                    totalRecordCount = result.TotalRecordCount;

                    foreach (var series in result.Items)
                    {
                        var info = await Provider.Series.Info(series.ImdbId, 0, 0, cancellationToken);

                        items.Add(new ChannelItemInfo
                        {
                            Id = "series|" + series.ImdbId,
                            Type = ChannelItemType.Folder,
                            MediaType = ChannelMediaType.Video,
                            ContentType = ChannelMediaContentType.Episode,
                            Name = series.Name,
                            SeriesName = series.Name,
                            ImageUrl = info.Image,
                            ProviderIds = info.ProviderIds,
                            ProductionYear = info.ProductionYear,
                            PremiereDate = info.PremiereDate,
                            Genres = info.Genres,
                            Studios = info.Studios,
                            OfficialRating = info.OfficialRating,
                            DateCreated = info.PremiereDate,
                        });
                    }
                }
                else
                {
                    var terms = query.FolderId.Split(Convert.ToChar("|"));
                    var imdbId = terms[1];

                    var seasonNumber = terms.Length > 2 ? Convert.ToInt32(terms[2]) : 0;
                    var episodeNumber = terms.Length > 3 ? Convert.ToInt32(terms[3]) : 0;

                    if (seasonNumber == 0 && episodeNumber == 0)
                    {
                        var seasons = await rest.Seasons(imdbId, cancellationToken);

                        foreach (var season in seasons)
                        {
                            var info = await Provider.Series.Info(imdbId, season.SeasonNumber, 0, cancellationToken);

                            items.Add(new ChannelItemInfo
                            {
                                Id = query.FolderId + "|" + season.SeasonNumber,
                                Type = ChannelItemType.Folder,
                                MediaType = ChannelMediaType.Video,
                                ContentType = ChannelMediaContentType.Episode,
                                SeriesName = season.SeriesName,
                                IndexNumber = season.SeasonNumber,
                                Name = "Season " + season.SeasonNumber,
                                ImageUrl = info.Image
                            });
                        }
                    }
                    else if (seasonNumber > 0 && episodeNumber == 0)
                    {
                        var episodes = await rest.Episodes(imdbId, seasonNumber, cancellationToken);

                        foreach (var episode in episodes)
                        {
                            var info = await Provider.Series.Info(imdbId, seasonNumber, episode.EpisodeNumber, cancellationToken);

                            items.Add(new ChannelItemInfo
                            {
                                Id = query.FolderId + "|" + episode.EpisodeNumber,
                                ParentIndexNumber = seasonNumber,
                                IndexNumber = episode.EpisodeNumber,
                                SeriesName = episode.SeriesName,
                                Name = "S" + seasonNumber + ", E" + episode.EpisodeNumber + " - " + info.Name,
                                Overview = info.Overview,
                                People = info.People,
                                ImageUrl = info.Image,
                                ProviderIds = info.ProviderIds,
                                //ProductionYear = info.ProductionYear,
                                PremiereDate = info.PremiereDate,
                                DateCreated = info.PremiereDate,
                                RunTimeTicks = info.RunTimeTicks,
                                Type = ChannelItemType.Media,
                                ContentType = ChannelMediaContentType.Clip,
                                MediaType = ChannelMediaType.Video
                            });
                        }
                    }

                    totalRecordCount = items.Count;
                }

                return new Scaffold.Result.Channel
                {
                    Items = items,
                    TotalRecordCount = totalRecordCount
                };
            }, cancellationToken);
        }
    }
}
