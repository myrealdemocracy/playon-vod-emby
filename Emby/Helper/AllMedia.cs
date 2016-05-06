using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Controller.Channels;

namespace PlayOn.Emby.Helper
{
    public class AllMedia
    {
        public static async Task<ChannelItemResult> Items(InternalAllChannelMediaQuery query, CancellationToken cancellationToken)
        {
            return await Task.Run(async () =>
            {
                var channelItemInfo = new List<ChannelItemInfo>();
                var rest = new Rest.Video();

                var videos = await rest.All(cancellationToken);

                foreach (var video in videos)
                {
                    channelItemInfo.Add(new ChannelItemInfo
                    {
                        Id = video.Type + "|" + video.ImdbId,
                        Name = video.Name
                    });
                }

                return new ChannelItemResult
                {
                    Items = channelItemInfo.ToList(),
                    TotalRecordCount = channelItemInfo.Count
                };
            }, cancellationToken);
        }
    }
}
