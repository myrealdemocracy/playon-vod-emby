using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using MediaBrowser.Common.Net;
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
        protected static MemoryCache Cache = new MemoryCache("TvdbSeriesInfo");
        protected static ILogger Logger = Emby.Channel.Logger;

        public static async Task<Scaffold.Series> Info(string imdbId, int? seasonNumber
            , int? episodeNumber, CancellationToken cancellationToken)
        {
            return await Task.Run(async () =>
            {
                seasonNumber = seasonNumber ?? 0;
                episodeNumber = episodeNumber ?? 0;

                var seriesItem = new MediaBrowser.Controller.Entities.TV.Series();
                var episodeItem = new MediaBrowser.Controller.Entities.TV.Episode();
                var seriesId = "";
                var seriesDataPath = "";
                var episodeInfo = new EpisodeInfo();

                var series = new Scaffold.Series
                {
                    Genres = new List<string>(),
                    Studios = new List<string>(),
                    ProviderIds = new Dictionary<string, string>()
                };

                var seriesInfo = new SeriesInfo
                {
                    MetadataLanguage = "en",
                    ParentIndexNumber = seasonNumber,
                    IndexNumber = episodeNumber
                };

                Logger.Debug("imdbId: " + imdbId);
                Logger.Debug("seasonNumber: " + seasonNumber);
                Logger.Debug("episodeNumber: " + episodeNumber);

                try
                {
                    if (Cache[imdbId] == null)
                    {
                        using (var result = await Emby.Channel.HttpClient.Get(new HttpRequestOptions
                        {
                            Url = "http://www.thetvdb.com/api/GetSeriesByRemoteID.php?language=en&imdbid=" + imdbId,
                            ResourcePool = new SemaphoreSlim(2, 2),
                            CancellationToken = cancellationToken
                        }).ConfigureAwait(false))
                        {
                            var doc = new XmlDocument();
                            doc.Load(result);

                            if (doc.HasChildNodes)
                            {
                                var node = doc.SelectSingleNode("//Series/seriesid");

                                if (node != null)
                                {
                                    seriesId = node.InnerText;
                                }
                            }
                        }

                        Logger.Debug("seriesId: " + seriesId);

                        seriesInfo.ProviderIds = new Dictionary<string, string>
                        {
                            {
                                MetadataProviders.Tvdb.ToString(),
                                seriesId
                            },
                            {
                                MetadataProviders.Imdb.ToString(),
                                imdbId
                            }
                        };

                        var tvdbSeries = await TvdbSeriesProvider.Current.GetMetadata(seriesInfo, cancellationToken);

                        seriesItem = tvdbSeries.Item;

                        Cache.Add(imdbId, seriesItem, DateTimeOffset.Now.AddDays(1));
                    }
                    else seriesItem = Cache[imdbId] as MediaBrowser.Controller.Entities.TV.Series;

                    foreach (var providerId in seriesItem.ProviderIds)
                    {
                        Logger.Debug("seriesItem.ProviderId: " + providerId);
                    }

                    seriesDataPath = TvdbSeriesProvider.GetSeriesDataPath(
                        Emby.Channel.Config.ApplicationPaths,
                        new Dictionary<string, string>
                        {
                            {
                                MetadataProviders.Tvdb.ToString(),
                                seriesItem.GetProviderId(MetadataProviders.Tvdb)
                            }
                        });

                    Logger.Debug("seriesItem.Name: " + seriesItem.Name);
                    Logger.Debug("seriesItem.PremiereDate: " + seriesItem.PremiereDate);
                    Logger.Debug("seriesDataPath:" + seriesDataPath);

                    series.ProductionYear = seriesItem.ProductionYear;
                    series.PremiereDate = seriesItem.PremiereDate;
                    series.Studios = seriesItem.Studios;
                    series.Genres = seriesItem.Genres;
                    series.OfficialRating = seriesItem.OfficialRating;
                    series.ProviderIds = seriesItem.ProviderIds;
                }
                catch (Exception exception)
                {
                    Logger.ErrorException("seriesItem", exception);
                }

                try
                {
                    episodeInfo = new EpisodeInfo
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

                    foreach (var providerId in episodeItem.ProviderIds)
                    {
                        Logger.Debug("episodeItem.ProviderId: " + providerId);
                    }

                    Logger.Debug("episodeItem null?: " + (episodeItem == null));
                    Logger.Debug("episodeItem.Name: " + episodeItem.Name);
                    Logger.Debug("episodeItem.PremiereDate: " + episodeItem.PremiereDate);

                    series.Name = "S" + seasonNumber + " E" + episodeNumber + " - " + episodeItem.Name;
                    series.Overview = episodeItem.Overview;
                    series.PremiereDate = episodeItem.PremiereDate;
                    series.OfficialRating = episodeItem.OfficialRating;
                }
                catch (Exception exception)
                {
                    Logger.ErrorException("episodeItem", exception);
                }

                try
                {
                    IEnumerable<RemoteImageInfo> tvdbImages = null;

                    if (seasonNumber == 0 && episodeNumber == 0)
                    {
                        var tvdbImageProvider = new TvdbSeriesImageProvider(Emby.Channel.Config, Emby.Channel.HttpClient, Emby.Channel.FileSystem);

                        tvdbImages = tvdbImageProvider.GetImages(Path.Combine(seriesDataPath, "banners.xml"), "en", cancellationToken);

                        tvdbImages = tvdbImages.Where(q => q.Language == "en");
                    }
                    else if (seasonNumber > 0 && episodeNumber == 0)
                    {
                        tvdbImages = TvdbSeasonImageProvider.GetImages(Path.Combine(seriesDataPath, "banners.xml"), "en", Convert.ToInt32(seasonNumber), cancellationToken);

                        tvdbImages = tvdbImages.Where(q => q.Language == "en");
                    }
                    else if (seasonNumber > 0 && episodeNumber > 0)
                    {
                        var tvdbImageProvider = new TvdbEpisodeImageProvider(Emby.Channel.Config, Emby.Channel.HttpClient, Emby.Channel.FileSystem);

                        //tvdbImages = await tvdbImageProvider.GetImages(episodeItem, cancellationToken);

                        var nodes = TvdbEpisodeProvider.Current.GetEpisodeXmlNodes(seriesDataPath, episodeInfo);

                        tvdbImages = nodes.Select(i => tvdbImageProvider.GetImageInfo(i, cancellationToken)).Where(i => i != null).ToList();
                    }

                    Logger.Debug("tvdbimages?: " + (tvdbImages == null));

                    if (tvdbImages != null)
                    {
                        Logger.Debug("tvdbimages.Count: " + tvdbImages.Count());

                        var image = new RemoteImageInfo();

                        foreach (var imageItem in tvdbImages.OrderByDescending(o => o.VoteCount))
                        {
                            Logger.Debug("tvdbimages.Type: " + imageItem.Type);
                            Logger.Debug("tvdbimages.Url: " + imageItem.Url);
                            Logger.Debug("tvdbimages.Language: " + imageItem.Language);

                            if (imageItem.Type != ImageType.Primary) continue;

                            image = imageItem;
                        }

                        series.Image = image.Url;
                    }
                }
                catch (Exception exception)
                {
                    Logger.ErrorException("tvdbimages", exception);
                }

                return series;
            }, cancellationToken);
        }
    }
}
