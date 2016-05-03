using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Providers;
using MediaBrowser.Providers.Omdb;
using MediaBrowser.Providers.TV;

namespace PlayOn.Emby.Helper.Provider
{
    public class Series
    {
        protected static MemoryCache Cache = new MemoryCache("PlayOnSeries");
        protected static ILogger Logger = Emby.Channel.Logger;

        public static async Task<Scaffold.Series> Info(string name, CancellationToken cancellationToken, int? seasonNumber = 0, int? episodeNumber = 0)
        {
            return await Task.Run(async () =>
            {
                seasonNumber = seasonNumber ?? 0;
                episodeNumber = episodeNumber ?? 0;

                var seriesItem = new MediaBrowser.Controller.Entities.TV.Series();
                var episodeItem = new MediaBrowser.Controller.Entities.TV.Episode();
                var series = new Scaffold.Series();
                var seriesDataPath = "";
                var seriesInfo = new SeriesInfo
                {
                    Name = name,
                    MetadataLanguage = "en",
                    ParentIndexNumber = seasonNumber,
                    IndexNumber = episodeNumber
                };

                Logger.Debug("name: " + name);
                Logger.Debug("seasonNumber: " + seasonNumber);
                Logger.Debug("episodeNumber: " + episodeNumber);

                try
                {
                    MetadataResult<MediaBrowser.Controller.Entities.TV.Series> omdbSeries;

                    if (Cache["omdb" + name] == null)
                    {
                        var omdbItemProvider = new OmdbItemProvider(Emby.Channel.JsonSerializer, Emby.Channel.HttpClient, Emby.Channel.Logger, Emby.Channel.LibraryManager);

                        omdbSeries = await omdbItemProvider.GetMetadata(seriesInfo, cancellationToken);

                        Cache.Add("omdb" + name, omdbSeries, DateTimeOffset.Now.AddDays(1));
                    }
                    else omdbSeries = Cache["omdb" + name] as MetadataResult<MediaBrowser.Controller.Entities.TV.Series>;

                    seriesInfo.ProviderIds = omdbSeries.Item.ProviderIds;

                    if (Cache["tvdb" + name] == null)
                    {
                        var tvdbSeries = await TvdbSeriesProvider.Current.GetMetadata(seriesInfo, cancellationToken);

                        seriesItem = tvdbSeries.Item;

                        Cache.Add("tvdb" + name, seriesItem, DateTimeOffset.Now.AddDays(1));
                    }
                    else seriesItem = Cache["tvdb" + name] as MediaBrowser.Controller.Entities.TV.Series;

                    seriesDataPath = TvdbSeriesProvider.GetSeriesDataPath(
                        Emby.Channel.Config.ApplicationPaths,
                        new Dictionary<string, string>
                        {
                            {
                                MetadataProviders.Tvdb.ToString(),
                                 seriesItem.GetProviderId(MetadataProviders.Tvdb)
                            }
                        });
                }
                catch (Exception exception)
                {}

                try
                {
                    var episodeInfo = new EpisodeInfo
                    {
                        ParentIndexNumber = seasonNumber,
                        IndexNumber = episodeNumber,
                        SeriesProviderIds = seriesItem.ProviderIds,
                        ProviderIds = seriesItem.ProviderIds,
                        MetadataLanguage = seriesItem.GetPreferredMetadataLanguage(),
                        MetadataCountryCode = seriesItem.GetPreferredMetadataCountryCode()
                    };

                    var tvdbEpisode = await TvdbEpisodeProvider.Current.GetMetadata(episodeInfo, cancellationToken);

                    episodeItem = tvdbEpisode.Item;

                    Logger.Debug("episodeItem null?: " + (episodeItem == null));
                    Logger.Debug("episodeItem.Name: " + episodeItem.Name);

                    if (episodeItem.Series != null) Logger.Debug("episodeItem.Series.Name: " + episodeItem.Series.Name);

                    series.Name = "S" + seasonNumber + " E" + episodeNumber + " - " + episodeItem.Name;
                    series.Overview = episodeItem.Overview;
                }
                catch (Exception exception)
                {}

                try
                {
                    var image = new RemoteImageInfo();
                    IEnumerable<RemoteImageInfo> tvdbImages = new List<RemoteImageInfo>();

                    if (seasonNumber == 0 && episodeNumber == 0)
                    {
                        var tvdbImageProvider = new TvdbSeriesImageProvider(Emby.Channel.Config, Emby.Channel.HttpClient, Emby.Channel.FileSystem);

                        tvdbImages = tvdbImageProvider.GetImages(Path.Combine(seriesDataPath, "banners.xml"), "en", cancellationToken);
                    }
                    else if (seasonNumber > 0 && episodeNumber == 0)
                    {
                        tvdbImages = TvdbSeasonImageProvider.GetImages(Path.Combine(seriesDataPath, "banners.xml"), "en", Convert.ToInt32(seasonNumber), cancellationToken);
                    }
                    else if (seasonNumber > 0 && episodeNumber > 0)
                    {
                        var tvdbImageProvider = new TvdbEpisodeImageProvider(Emby.Channel.Config, Emby.Channel.HttpClient, Emby.Channel.FileSystem);

                        tvdbImages = await tvdbImageProvider.GetImages(episodeItem, cancellationToken);
                    }

                    Logger.Debug("tvdbimages?: " + (tvdbImages == null));
                    if (tvdbImages != null) Logger.Debug("tvdbimages.Count: " + tvdbImages.Count());

                    image = tvdbImages.OrderByDescending(o => o.VoteCount).FirstOrDefault(q => q.Type == ImageType.Primary);

                    series.Image = image.Url;
                }
                catch (Exception exception)
                {}

                return series;
            }, cancellationToken);
        }
    }
}
