using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using CommonIO;
using MediaBrowser.Common;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Channels;
using MediaBrowser.Controller.Configuration;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Localization;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Channels;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.MediaInfo;
using MediaBrowser.Model.Serialization;

namespace PlayOn.Emby
{
    public class Channel : IChannel//, ISearchableChannel
    {
        public static IServerConfigurationManager Config;
        public static ILibraryManager LibraryManager;
        public static IFileSystem FileSystem;
        public static IHttpClient HttpClient;
        public static ILogger Logger;
        public static IJsonSerializer JsonSerializer;
        public static ILocalizationManager Localization;
        public static IApplicationHost AppHost;

        public Channel(IServerConfigurationManager configuration, ILibraryManager libraryManager, IFileSystem fileSystem, IHttpClient httpClient, ILogManager logManager, IJsonSerializer jsonSerializer, ILocalizationManager localization, IApplicationHost appHost)
        {
            Config = configuration;
            LibraryManager = libraryManager;
            FileSystem = fileSystem;
            HttpClient = httpClient;
            Logger = logManager.GetLogger(GetType().Name);
            JsonSerializer = jsonSerializer;
            Localization = localization;
            AppHost = appHost;
        }

        public InternalChannelFeatures GetChannelFeatures()
        {
            return new InternalChannelFeatures
            {
                ContentTypes = new List<ChannelMediaContentType>
                {
                    ChannelMediaContentType.Clip,
                    ChannelMediaContentType.Episode,
                    ChannelMediaContentType.Movie
                },

                MediaTypes = new List<ChannelMediaType>
                {
                    ChannelMediaType.Video
                },

                SupportsContentDownloading = true,
                SupportsSortOrderToggle = false,
                AutoRefreshLevels = 2
            };
        }

        public bool IsEnabledFor(string userId)
        {
            return true;
        }

        public Task<ChannelItemResult> GetChannelItems(InternalChannelItemQuery query, CancellationToken cancellationToken)
        {
            return Task.Run(async () =>
            {
                var channelItemInfos = await Helper.Channel.Items(query.FolderId, cancellationToken);

                return new ChannelItemResult
                {
                    Items = channelItemInfos.ToList(),
                    TotalRecordCount = channelItemInfos.Count
                };
            }, cancellationToken);
        }

        public Task<DynamicImageResponse> GetChannelImage(ImageType type, CancellationToken cancellationToken)
        {
            return Task.Run(() => new DynamicImageResponse(), cancellationToken);
        }

        public IEnumerable<ImageType> GetSupportedChannelImages()
        {
            return new List<ImageType>
            {
                ImageType.Primary
            };
        }

        public string Name
        {
            get { return "Video On Demand"; }
        }

        public string Description
        {
            get { return ""; }
        }

        public string DataVersion
        {
            get { return "2016-04-26"; }
        }

        public string HomePageUrl
        {
            get { return "http://www.playon.tv"; }
        }

        public ChannelParentalRating ParentalRating
        {
            get { return ChannelParentalRating.GeneralAudience; }
        }
    }
}
