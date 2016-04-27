using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MediaBrowser.Controller.Channels;
using MediaBrowser.Model.Channels;
using MediaBrowser.Model.MediaInfo;

namespace PlayOn.Tools.Helper
{
    public class Video
    {
        public static ChannelItemInfo Mapper(Scaffold.Video video, string currentFolder, string parentFolder = "")
        {
            if (video.Item == null) return new ChannelItemInfo();

            var id = Url.Clean(currentFolder + parentFolder + video.Item.Name.ToLower() + "|");

            if (video.Series == null) video.Series = new Scaffold.Info();

            if (video.Description == null) video.Description = new Scaffold.Info();

            var name = video.Item.Name;

            if (name.Contains(video.Series.Name)) name = name.Replace(video.Series.Name + " - ", "");

            return new ChannelItemInfo
            {
                Id = id,
                Name = name,
                SeriesName = video.Series.Name,
                Overview = video.Description.Name,
                Type = ChannelItemType.Media,
                ContentType = ChannelMediaContentType.Clip,
                MediaType = ChannelMediaType.Video,
                ImageUrl = Url.Image + WebUtility.UrlEncode(id),
                MediaSources = new List<ChannelMediaInfo>
                {
                    new ChannelMediaInfo
                    {
                        Path = Url.Video + WebUtility.UrlEncode(id),
                        Protocol = MediaProtocol.Http
                    }
                }
            };
        }

        public static ChannelItemInfo Generate(Scaffold.Item item, string currentFolder, string parentFolder = "")
        {
            var video = Mapper(Xml.Items<Scaffold.Video>(item.Href), currentFolder, parentFolder);

            return video ?? new ChannelItemInfo();
        }
    }
}
