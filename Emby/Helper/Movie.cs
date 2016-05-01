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

                if (currentFolder == "movies")
                {
                    var rest = new Rest.Movie();

                    var movies = await rest.All(cancellationToken);

                    foreach (var movie in movies)
                    {
                        var mediaSources = new List<ChannelMediaInfo>();

                        foreach (var video in movie.Videos)
                        {
                            mediaSources.Add(new ChannelMediaInfo
                            {
                                Path = "http://playon.local/url/video?id=" + WebUtility.UrlEncode(video.Path),
                                Protocol = MediaProtocol.Http
                            });
                        }

                        channelItemInfos.Add(new ChannelItemInfo
                        {
                            Id = currentFolder + "|" + movie.Name.ToLower(),
                            Name = movie.Name,
                            Type = ChannelItemType.Media,
                            ContentType = ChannelMediaContentType.Clip,
                            MediaType = ChannelMediaType.Video,
                            MediaSources = mediaSources
                        });
                    }
                }
                else
                {
                    var terms = currentFolder.Split(Convert.ToChar("|"));
                    var name = terms[1];
                }

                return channelItemInfos;
            }, cancellationToken);
        }
    }
}