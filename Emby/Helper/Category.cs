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
    public class Category
    {
        protected static ILogger Logger = Emby.Channel.Logger;

        public static async Task<List<ChannelItemInfo>> Items(string currentFolder, CancellationToken cancellationToken)
        {
            return await Task.Run(async () =>
            {
                var channelItemInfos = new List<ChannelItemInfo>();

                if (currentFolder == "categories")
                {
                    var rest = new Rest.Category();

                    var categories = await rest.All(cancellationToken);

                    foreach (var item in categories.All)
                    {
                        channelItemInfos.Add(new ChannelItemInfo
                        {
                            Id = "category|" + item.ToLower(),
                            Name = item,
                            Type = ChannelItemType.Folder
                        });
                    }
                }
                else
                {

                }

                return channelItemInfos;
            }, cancellationToken);
        }
    }
}
