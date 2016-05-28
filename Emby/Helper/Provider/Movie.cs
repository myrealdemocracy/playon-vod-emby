using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Providers;
using MediaBrowser.Providers.Movies;
using MediaBrowser.Providers.Omdb;

namespace PlayOn.Emby.Helper.Provider
{
    public class Movie
    {
        protected static MemoryCache Cache = new MemoryCache("PlayOnMovies");
        protected static ILogger Logger = Emby.Channel.Logger;

        public static async Task<Scaffold.Movie> Info(string name, string imdbId, CancellationToken cancellationToken)
        {
            return await Task.Run(async () =>
            {
                var movieItem = new MediaBrowser.Controller.Entities.Movies.Movie();
                var movieDataPath = "";

                var movie = new Scaffold.Movie
                {
                    Genres = new List<string>(),
                    Studios = new List<string>(),
                    ProviderIds = new Dictionary<string, string>()
                };

                var movieInfo = new MovieInfo
                {
                    Name = name,
                    MetadataLanguage = "en",
                    ProviderIds = new Dictionary<string, string>
                    {
                        {MetadataProviders.Imdb.ToString(), imdbId}
                    }
                };

                try
                {
                    var moviedbProvider = new MovieDbProvider(Emby.Channel.JsonSerializer, Emby.Channel.HttpClient, Emby.Channel.FileSystem, Emby.Channel.Config, Emby.Channel.Logger, Emby.Channel.Localization, Emby.Channel.LibraryManager, Emby.Channel.AppHost);

                    var moviedb = await moviedbProvider.GetMetadata(movieInfo, cancellationToken);

                    movieItem = moviedb.Item;

                    Logger.Debug("movieDb.Item.Name: " + movieItem.Name);
                    Logger.Debug("movieDb.Item.Overview: " + movieItem.Overview);

                    movie.Name = movieItem.Name;
                    movie.Overview = movieItem.Overview;
                    movie.PremiereDate = movieItem.PremiereDate;
                    movie.ProductionYear = movieItem.ProductionYear;
                    movie.RunTimeTicks = movieItem.RunTimeTicks;
                    movie.Genres = movieItem.Genres;
                    movie.OfficialRating = movieItem.OfficialRating;
                    movie.Studios = movieItem.Studios;
                    movie.ProviderIds = movieItem.ProviderIds;

                    if (movie.PremiereDate == null) movie.PremiereDate = DateTime.UtcNow.AddYears(-10);
                }
                catch (Exception exception)
                {
                    Logger.ErrorException("moviedbProvider", exception);
                }

                try
                {
                    var image = new RemoteImageInfo();
                    IEnumerable<RemoteImageInfo> fanartImages = new List<RemoteImageInfo>();

                    var fanartMovieImageProvider = new FanartMovieImageProvider(Emby.Channel.Config, Emby.Channel.HttpClient, Emby.Channel.FileSystem, Emby.Channel.JsonSerializer);

                    fanartImages = await fanartMovieImageProvider.GetImages(movieItem, cancellationToken);

                    Logger.Debug("fanartImages?: " + (fanartImages == null));
                    if (fanartImages != null) Logger.Debug("fanartImages.Count: " + fanartImages.Count());

                    image = fanartImages.OrderByDescending(o => o.VoteCount).FirstOrDefault(q => q.Type == ImageType.Primary);

                    movie.Image = image.Url;

                    if (String.IsNullOrWhiteSpace(movie.Image))
                    {
                        var movieDbImageProvider = new MovieDbImageProvider(Emby.Channel.JsonSerializer, Emby.Channel.HttpClient);

                        var images = await movieDbImageProvider.GetImages(movieItem, cancellationToken);

                        image = images.OrderByDescending(o => o.VoteCount).FirstOrDefault(q => q.Type == ImageType.Primary);
                    }
                }
                catch (Exception exception)
                {
                    Logger.ErrorException("images", exception);
                }

                return movie;
            }, cancellationToken);
        }
    }
}