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

                    foreach (var item in series)
                    {
                        channelItemInfos.Add(new ChannelItemInfo
                        {
                            Id = "series|" + item.Name.ToLower(),
                            Name = item.Name,
                            Type = ChannelItemType.Folder
                        });
                    }
                }
                else
                {
                    var terms = currentFolder.Split(Convert.ToChar("|"));
                    var name = terms[1];
                    var season = Convert.ToInt32(terms[2]);
                    var episode = Convert.ToInt32(terms[3]);

                    if (season == 0 && episode == 0)
                    {
                        var seasons = await rest.Seasons(name, cancellationToken);
                    }
                    else if (season > 0 && episode == 0)
                    {
                        var episodes = await rest.Episodes(name, season, cancellationToken);
                    }
                    else if(season > 0 && episode > 0)
                    {
                    }
                }

                return channelItemInfos;
            }, cancellationToken);
        }
    }
}
