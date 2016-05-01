using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using MediaBrowser.Providers.TV;

namespace PlayOn.Emby.Helper.Provider
{
    public class Series
    {
        public static async Task<Scaffold.Series> Info(string name, CancellationToken cancellationToken, int seasonNumber = 0, int episodeNumber = 0)
        {
            return await Task.Run(async () =>
            {
                var seriesItem = new MediaBrowser.Controller.Entities.TV.Series();
                var episodeItem = new MediaBrowser.Controller.Entities.TV.Episode();
                var series = new Scaffold.Series();
                var seriesDataPath = "";
                var seriesInfo = new SeriesInfo
                {
                    Name = name,
                    MetadataLanguage = "en",
                };

                try
                {
                    if (seasonNumber > 0) seriesInfo.ParentIndexNumber = seasonNumber;
                    if (episodeNumber > 0) seriesInfo.IndexNumber = episodeNumber;

                    var tvdbSerie = await TvdbSeriesProvider.Current.GetMetadata(seriesInfo, cancellationToken);
                    seriesItem = tvdbSerie.Item;
                    var serieId = seriesItem.GetProviderId(MetadataProviders.Tvdb);
                    seriesDataPath = TvdbSeriesProvider.GetSeriesDataPath(Emby.Channel.Config.ApplicationPaths,
                        new Dictionary<string, string> { { MetadataProviders.Tvdb.ToString(), serieId } });
                }
                catch (Exception exception)
                {}

                try
                {
                    if (seasonNumber > 0 && episodeNumber > 0)
                    {
                        var episodeInfo = new EpisodeInfo
                        {
                            ParentIndexNumber = seasonNumber,
                            IndexNumber = episodeNumber,
                            SeriesProviderIds = seriesItem.ProviderIds,
                            MetadataLanguage = seriesItem.GetPreferredMetadataLanguage(),
                            MetadataCountryCode = seriesItem.GetPreferredMetadataCountryCode()
                        };

                        var tvdbEpisode = await TvdbEpisodeProvider.Current.GetMetadata(episodeInfo, cancellationToken);
                        episodeItem = tvdbEpisode.Item;

                        series.Name = "S" + seasonNumber + " E" + episodeNumber + " - " + episodeItem.Name;
                        series.Overview = episodeItem.Overview;
                    }
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
                        tvdbImages = TvdbSeasonImageProvider.GetImages(Path.Combine(seriesDataPath, "banners.xml"), "en", seasonNumber, cancellationToken);
                    }
                    else if (seasonNumber > 0 && episodeNumber > 0)
                    {
                        var tvdbImageProvider = new TvdbEpisodeImageProvider(Emby.Channel.Config, Emby.Channel.HttpClient, Emby.Channel.FileSystem);
                        tvdbImages = tvdbImageProvider.GetImages(episodeItem, cancellationToken).Result;
                    }

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
