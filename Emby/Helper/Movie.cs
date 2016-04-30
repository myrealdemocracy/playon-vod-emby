using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Controller.Channels;
using MediaBrowser.Model.Logging;

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

                    foreach (var item in movies)
                    {
                        channelItemInfos.Add(new ChannelItemInfo
                        {
                            Id = "movies|" + item.Name.ToLower(),
                            Name = item.Name,
                            Type = ChannelItemType.Folder
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