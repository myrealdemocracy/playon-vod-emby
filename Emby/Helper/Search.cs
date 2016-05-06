using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Controller.Channels;

namespace PlayOn.Emby.Helper
{
    public class Search
    {
        public static async Task<IEnumerable<ChannelItemInfo>> Items(ChannelSearchInfo searchInfo, CancellationToken cancellationToken)
        {
            return await Task.Run(async () =>
            {
                var channelItemInfo = new List<ChannelItemInfo>();
                var rest = new Rest.Search();

                var videos = await rest.Name(searchInfo.SearchTerm, cancellationToken);

                foreach (var video in videos)
                {
                    channelItemInfo.Add(new ChannelItemInfo
                    {
                        Id = video.Type + "|" + video.ImdbId,
                        Name = video.Name
                    });
                }

                return channelItemInfo.ToList();
            }, cancellationToken);
        }
    }
}
