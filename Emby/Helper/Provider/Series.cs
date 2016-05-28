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
using MediaBrowser.Controller.Entities.TV;
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
                var seasonInfo = new SeasonInfo();
                var episodeInfo = new EpisodeInfo();

                var info = new Scaffold.Series
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

                    foreach (var studio in seriesItem.Studios)
                    {
                        Logger.Debug("seriesItem.Studio: " + studio);
                    }

                    foreach (var genre in seriesItem.Genres)
                    {
                        Logger.Debug("seriesItem.Genre: " + genre);
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

                    Logger.Debug("seriesItem.ExternalId: " + seriesItem.ExternalId);
                    Logger.Debug("seriesItem.ExternalEtag: " + seriesItem.ExternalEtag);
                    Logger.Debug("seriesItem.Name: " + seriesItem.Name);
                    Logger.Debug("seriesItem.PremiereDate: " + seriesItem.PremiereDate);
                    Logger.Debug("seriesDataPath:" + seriesDataPath);

                    info.SeriesName = seriesItem.Id.ToString();
                    info.ProductionYear = seriesItem.ProductionYear;
                    info.PremiereDate = seriesItem.PremiereDate;
                    info.Studios = seriesItem.Studios;
                    info.Genres = seriesItem.Genres;
                    info.OfficialRating = seriesItem.OfficialRating;
                    info.ProviderIds = seriesItem.ProviderIds;
                    info.HomePageUrl = seriesItem.HomePageUrl;

                    if (info.PremiereDate == null) info.PremiereDate = DateTime.UtcNow.AddYears(-10);
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
                        MetadataLanguage = seriesItem.GetPreferredMetadataLanguage(),
                        MetadataCountryCode = seriesItem.GetPreferredMetadataCountryCode()
                    };

                    var tvdbEpisode = await TvdbEpisodeProvider.Current.GetMetadata(episodeInfo, cancellationToken);

                    episodeItem = tvdbEpisode.Item;

                    foreach (var providerId in episodeItem.ProviderIds)
                    {
                        Logger.Debug("episodeItem.ProviderId: " + providerId);
                    }

                    foreach (var studio in episodeItem.Studios)
                    {
                        Logger.Debug("episodeItem.Studio: " + studio);
                    }

                    foreach (var genre in episodeItem.Genres)
                    {
                        Logger.Debug("episodeItem.Genre: " + genre);
                    }

                    Logger.Debug("episodeItem null?: " + (episodeItem == null));
                    Logger.Debug("episodeItem.ExternalId: " + episodeItem.ExternalId);
                    Logger.Debug("episodeItem.ExternalEtag: " + episodeItem.ExternalEtag);
                    Logger.Debug("episodeItem.Name: " + episodeItem.Name);
                    Logger.Debug("episodeItem.PremiereDate: " + episodeItem.PremiereDate);

                    info.Name = episodeItem.Name;
                    info.Overview = episodeItem.Overview;
                    info.ProviderIds = episodeItem.ProviderIds;
                    //info.Genres = episodeItem.Genres;
                    //info.Studios = episodeItem.Studios;
                    info.PremiereDate = episodeItem.PremiereDate;
                    info.ProductionYear = episodeItem.ProductionYear;
                    info.RunTimeTicks = episodeItem.RunTimeTicks;
                    info.OfficialRating = episodeItem.OfficialRating;

                    if (info.PremiereDate == null) info.PremiereDate = DateTime.UtcNow.AddYears(-10);
                }
                catch (Exception exception)
                {
                    Logger.ErrorException("episodeItem", exception);
                }

                try
                {
                    IEnumerable<RemoteImageInfo> images = null;

                    if (seasonNumber == 0 && episodeNumber == 0)
                    {
                        var fanartSeriesProvider = new FanartSeriesProvider(Emby.Channel.Config, Emby.Channel.HttpClient, Emby.Channel.FileSystem, Emby.Channel.JsonSerializer);

                        images = await fanartSeriesProvider.GetImages(seriesItem, cancellationToken);

                        images = images.Where(q => q.Language == "en");

                        if (images.Count(c => c.Type == ImageType.Primary) == 0)
                        {
                            var tvdbSeriesImageProvider = new TvdbSeriesImageProvider(Emby.Channel.Config, Emby.Channel.HttpClient, Emby.Channel.FileSystem);

                            images = await tvdbSeriesImageProvider.GetImages(seriesItem, cancellationToken);
                        }
                    }
                    else if (seasonNumber > 0 && episodeNumber == 0)
                    {
                        var fanArtSeasonProvider = new FanArtSeasonProvider(Emby.Channel.Config, Emby.Channel.HttpClient, Emby.Channel.FileSystem, Emby.Channel.JsonSerializer);

                        var season = new Season();
                        season.SetParent(seriesItem);

                        images = await fanArtSeasonProvider.GetImages(season, cancellationToken);

                        images = images.Where(q => q.Language == "en");
                    }
                    else if (seasonNumber > 0 && episodeNumber > 0)
                    {
                        var tvdbEpisodeImageProvider = new TvdbEpisodeImageProvider(Emby.Channel.Config, Emby.Channel.HttpClient, Emby.Channel.FileSystem);

                        var nodes = TvdbEpisodeProvider.Current.GetEpisodeXmlNodes(seriesDataPath, episodeInfo);

                        images = nodes.Select(i => tvdbEpisodeImageProvider.GetImageInfo(i, cancellationToken)).Where(i => i != null).ToList();
                    }

                    Logger.Debug("tvdbimages?: " + (images == null));

                    if (images != null)
                    {
                        Logger.Debug("tvdbimages.Count: " + images.Count());

                        var image = new RemoteImageInfo();

                        foreach (var imageItem in images.OrderByDescending(o => o.VoteCount))
                        {
                            Logger.Debug("tvdbimages.Type: " + imageItem.Type);
                            Logger.Debug("tvdbimages.Url: " + imageItem.Url);
                            Logger.Debug("tvdbimages.Language: " + imageItem.Language);

                            if (imageItem.Type != ImageType.Primary) continue;

                            image = imageItem;
                        }

                        info.Image = image.Url;
                    }
                }
                catch (Exception exception)
                {
                    Logger.ErrorException("images", exception);
                }

                return info;
            }, cancellationToken);
        }
    }
}
