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

        public static async Task<List<ChannelItemInfo>> Items(string currentFolder, CancellationToken cancellationToken)
        {
            return await Task.Run(async () =>
            {
                var rest = new Rest.Series();
                var channelItemInfos = new List<ChannelItemInfo>();

                if (currentFolder == "series")
                {
                    var series = await rest.All(cancellationToken);

                    foreach (var serie in series)
                    {
                        var info = await Provider.Series.Info(serie.Name, cancellationToken);

                        if(String.IsNullOrWhiteSpace(info.Image)) continue;

                        channelItemInfos.Add(new ChannelItemInfo
                        {
                            Id = "series|" + serie.Name.ToLower(),
                            Name = serie.Name,
                            Type = ChannelItemType.Folder,
                            Genres = info.Genres,
                            ImageUrl = info.Image,
                            ProductionYear = info.ProductionYear,
                            Studios = info.Studios,
                            ProviderIds = info.ProviderIds,
                            DateCreated = DateTime.UtcNow
                        });
                    }
                }
                else
                {
                    var terms = currentFolder.Split(Convert.ToChar("|"));
                    var name = terms[1];

                    var seasonNumber = terms.Length > 2 ? Convert.ToInt32(terms[2]) : 0;
                    var episodeNumber = terms.Length > 3 ? Convert.ToInt32(terms[3]) : 0;

                    if (seasonNumber == 0 && episodeNumber == 0)
                    {
                        var seasons = await rest.Seasons(name, cancellationToken);

                        foreach (var season in seasons)
                        {
                            var info = await Provider.Series.Info(name, cancellationToken, season.SeasonNumber);

                            channelItemInfos.Add(new ChannelItemInfo
                            {
                                Id = currentFolder + "|" + season.SeasonNumber,
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
                                        Id = currentFolder + "|" + video.Name,
                                        Name = video.Name,
                                        Overview = video.Overview,
                                        Type = ChannelItemType.Media,
                                        ContentType = ChannelMediaContentType.Clip,
                                        MediaType = ChannelMediaType.Video,
                                        ImageUrl = "http://playon.local/url/image?path=" + WebUtility.UrlEncode(video.Path),
                                        DateCreated = DateTime.UtcNow,
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
                                var info = await Provider.Series.Info(name, cancellationToken, seasonNumber, episode.EpisodeNumber);

                                channelItemInfos.Add(new ChannelItemInfo
                                {
                                    Id = currentFolder + "|" + episode.EpisodeNumber,
                                    Name = info.Name,
                                    Overview = info.Overview,
                                    Type = ChannelItemType.Media,
                                    ContentType = ChannelMediaContentType.Clip,
                                    MediaType = ChannelMediaType.Video,
                                    ImageUrl = info.Image,
                                    PremiereDate = info.PremiereDate,
                                    DateCreated = DateTime.UtcNow,
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
                }

                return channelItemInfos;
            }, cancellationToken);
        }
    }
}
