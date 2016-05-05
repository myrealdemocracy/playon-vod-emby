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

        public static async Task<List<ChannelItemInfo>> Items(string currentFolder, CancellationToken cancellationToken)
        {
            return await Task.Run(async () =>
            {
                var channelItemInfos = new List<ChannelItemInfo>();
                var rest = new Rest.Movie();

                if (currentFolder == "movies")
                {
                    channelItemInfos.Add(new ChannelItemInfo
                    {
                        Id = currentFolder + "|nmb",
                        Name = "#",
                        Type = ChannelItemType.Folder
                    });

                    for (var letter = 'A'; letter <= 'Z'; letter++)
                    {
                        channelItemInfos.Add(new ChannelItemInfo
                        {
                            Id = currentFolder + "|" + letter.ToString().ToLower(),
                            Name = letter.ToString(),
                            Type = ChannelItemType.Folder
                        });
                    }
                }
                else
                {
                    var terms = currentFolder.Split(Convert.ToChar("|"));
                    var letter = terms[1];

                    var movies = await rest.All(letter, cancellationToken);

                    foreach (var movie in movies)
                    {
                        var info = await Provider.Movie.Info(movie.Name, cancellationToken);

                        if (String.IsNullOrWhiteSpace(info.Image)) continue;

                        channelItemInfos.Add(new ChannelItemInfo
                        {
                            Id = "movies|" + movie.Name.ToLower(),
                            Name = movie.Name,
                            Overview = info.Overview,
                            Type = ChannelItemType.Media,
                            ContentType = ChannelMediaContentType.Clip,
                            MediaType = ChannelMediaType.Video,
                            ImageUrl = info.Image,
                            Genres = info.Genres,
                            OfficialRating = info.OfficialRating,
                            ProductionYear = info.ProductionYear,
                            Studios = info.Studios,
                            ProviderIds = info.ProviderIds,
                            DateCreated = info.PremiereDate,
                            MediaSources = new List<ChannelMediaInfo>
                            {
                                new ChannelMediaInfo
                                {
                                    Path = "http://playon.local/movie/video?name=" + WebUtility.UrlEncode(movie.Name),
                                    Protocol = MediaProtocol.Http,
                                    SupportsDirectPlay = true
                                }
                            }
                        });
                    }
                }

                return channelItemInfos;
            }, cancellationToken);
        }
    }
}