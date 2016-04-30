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
                        channelItemInfos.Add(new ChannelItemInfo
                        {
                            Id = "series|" + serie.Name.ToLower(),
                            Name = serie.Name,
                            Type = ChannelItemType.Folder
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
                            channelItemInfos.Add(new ChannelItemInfo
                            {
                                Id = currentFolder + "|" + season.SeasonNumber,
                                Name = "Season " + season.SeasonNumber,
                                Type = ChannelItemType.Folder
                            });
                        }
                    }
                    else if (seasonNumber > 0 && episodeNumber == 0)
                    {
                        var episodes = await rest.Episodes(name, seasonNumber, cancellationToken);

                        foreach (var episode in episodes)
                        {
                            var mediaSources = new List<ChannelMediaInfo>();

                            foreach (var video in episode.Videos)
                            {
                                mediaSources.Add(new ChannelMediaInfo
                                {
                                    Path = "http://playon.local/url/video?id=" + WebUtility.UrlEncode(video.Path),
                                    Protocol = MediaProtocol.Http
                                });
                            }

                            channelItemInfos.Add(new ChannelItemInfo
                            {
                                Id = currentFolder + "|" + episode.EpisodeNumber,
                                Name = "S" + seasonNumber + "E" + episode.EpisodeNumber,
                                Type = ChannelItemType.Media,
                                ContentType = ChannelMediaContentType.Clip,
                                MediaType = ChannelMediaType.Video,
                                MediaSources = mediaSources
                            });
                        }
                    }
                    else if(seasonNumber > 0 && episodeNumber > 0)
                    {
                    }
                }

                return channelItemInfos;
            }, cancellationToken);
        }
    }
}
