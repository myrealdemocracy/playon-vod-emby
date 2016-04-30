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
                var channelItemInfos = new List<ChannelItemInfo>();

                if (currentFolder == "series")
                {
                    var rest = new Rest.Series();

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
                }

                return channelItemInfos;
            }, cancellationToken);
        }
    }
}
