using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Controller.Channels;
using MediaBrowser.Model.Channels;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.MediaInfo;

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
                                ImageUrl = "http://192.168.3.40/img/movies.png"
                            },
                            new ChannelItemInfo
                            {
                                Id = "series",
                                Name = "TV Shows",
                                Type = ChannelItemType.Folder,
                                ImageUrl = "http://192.168.3.40/img/series.png"
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

        public static async Task<IEnumerable<ChannelMediaInfo>> Item(string id, CancellationToken cancellationToken)
        {
            return await Task.Run(async () =>
            {
                var media = new List<ChannelMediaInfo>();
                var terms = id.Split(Convert.ToChar("|"));
                var type = terms[0];
                var imdbId = terms[1];

                Logger.Debug("type: " + type);
                Logger.Debug("imdbId: " + imdbId);

                if (type == "movies")
                {
                    var restMovie = new Rest.Movie();
                    var movies = await restMovie.Videos(imdbId, cancellationToken);

                    media.AddRange(movies.Select(movie => new ChannelMediaInfo
                    {
                        Path = movie.Path,
                        Protocol = MediaProtocol.Http,
                        SupportsDirectPlay = true
                    }));
                }
                else if (type == "series")
                {
                    var season = Convert.ToInt32(terms[2]);
                    var episode = Convert.ToInt32(terms[3]);
                    var restSeries = new Rest.Series();
                    var series = await restSeries.Videos(imdbId, season, episode, cancellationToken);

                    Logger.Debug("season: " + season);
                    Logger.Debug("episode: " + episode);

                    media.AddRange(series.Select(item => new ChannelMediaInfo
                    {
                        Path = item.Path,
                        Protocol = MediaProtocol.Http,
                        SupportsDirectPlay = true
                    }));
                }

                return media.ToList();
            }, cancellationToken);
        }
    }
}
