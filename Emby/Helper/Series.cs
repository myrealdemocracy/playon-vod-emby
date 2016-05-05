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

        public static async Task<Scaffold.ChannelList> Items(InternalChannelItemQuery query, CancellationToken cancellationToken)
        {
            return await Task.Run(async () =>
            {
                var rest = new Rest.Series();
                var channelItemInfos = new List<ChannelItemInfo>();
                var totalRecordCount = 0;

                if (query.FolderId == "series")
                {
                    var result = await rest.All(query.StartIndex, query.Limit, cancellationToken);

                    totalRecordCount = result.TotalRecordCount;

                    foreach (var series in result.Series)
                    {
                        var info = await Provider.Series.Info(series.Name, 0, 0, cancellationToken);

                        channelItemInfos.Add(new ChannelItemInfo
                        {
                            Id = "series|" + series.Name.ToLower(),
                            Name = series.Name,
                            Type = ChannelItemType.Folder,
                            Genres = info.Genres,
                            ImageUrl = info.Image,
                            ProductionYear = info.ProductionYear,
                            Studios = info.Studios,
                            ProviderIds = info.ProviderIds,
                            DateCreated = info.PremiereDate
                        });
                    }
                }
                else
                {
                    var terms = query.FolderId.Split(Convert.ToChar("|"));
                    var name = terms[1];

                    var seasonNumber = terms.Length > 2 ? Convert.ToInt32(terms[2]) : 0;
                    var episodeNumber = terms.Length > 3 ? Convert.ToInt32(terms[3]) : 0;

                    if (seasonNumber == 0 && episodeNumber == 0)
                    {
                        var seasons = await rest.Seasons(name, cancellationToken);

                        foreach (var season in seasons)
                        {
                            var info = await Provider.Series.Info(name, season.SeasonNumber, 0, cancellationToken);

                            channelItemInfos.Add(new ChannelItemInfo
                            {
                                Id = query.FolderId + "|" + season.SeasonNumber,
                                Name = "Season " + season.SeasonNumber,
                                Type = ChannelItemType.Folder,
                                ImageUrl = info.Image
                            });
                        }
                    }
                    else if (seasonNumber > 0 && episodeNumber == 0)
                    {
                        var episodes = await rest.Episodes(name, seasonNumber, cancellationToken);

                        foreach (var episode in episodes)
                        {
                            if (episode.EpisodeNumber == 0)
                            {
                                foreach (var video in episode.Videos)
                                {
                                    channelItemInfos.Add(new ChannelItemInfo
                                    {
                                        Id = query.FolderId + "|" + video.Name,
                                        Name = video.Name,
                                        Overview = video.Overview,
                                        Type = ChannelItemType.Media,
                                        ContentType = ChannelMediaContentType.Clip,
                                        MediaType = ChannelMediaType.Video,
                                        ImageUrl = "http://playon.local/url/image?path=" + WebUtility.UrlEncode(video.Path),
                                        MediaSources = new List<ChannelMediaInfo>
                                        {
                                            new ChannelMediaInfo
                                            {
                                                Path = "http://playon.local/url/video?path=" + WebUtility.UrlEncode(video.Path),
                                                Protocol = MediaProtocol.Http,
                                                SupportsDirectPlay = true
                                            }
                                        }
                                    });
                                }
                            }
                            else
                            {
                                var info = await Provider.Series.Info(name, seasonNumber, episode.EpisodeNumber, cancellationToken);

                                channelItemInfos.Add(new ChannelItemInfo
                                {
                                    Id = query.FolderId + "|" + episode.EpisodeNumber,
                                    Name = info.Name,
                                    Overview = info.Overview,
                                    Type = ChannelItemType.Media,
                                    ContentType = ChannelMediaContentType.Clip,
                                    MediaType = ChannelMediaType.Video,
                                    ImageUrl = info.Image,
                                    PremiereDate = info.PremiereDate,
                                    DateCreated = info.PremiereDate,
                                    MediaSources = new List<ChannelMediaInfo>
                                    {
                                        new ChannelMediaInfo
                                        {
                                            Path = "http://playon.local/series/video/s/" + seasonNumber + "/e/" + episode.EpisodeNumber + "?name=" + WebUtility.UrlEncode(name),
                                            Protocol = MediaProtocol.Http,
                                            SupportsDirectPlay = true
                                        }
                                    }
                                });
                            }
                        }
                    }

                    totalRecordCount = channelItemInfos.Count;
                }

                return new Scaffold.ChannelList
                {
                    ChannelItemInfos = channelItemInfos,
                    TotalRecordCount = totalRecordCount
                };
            }, cancellationToken);
        }
    }
}
