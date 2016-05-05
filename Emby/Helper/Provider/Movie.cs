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

        public static async Task<Scaffold.Movie> Info(string name, CancellationToken cancellationToken)
        {
            return await Task.Run(async () =>
            {
                var movie = new Scaffold.Movie();
                var movieItem = new MediaBrowser.Controller.Entities.Movies.Movie();
                var movieDataPath = "";
                var movieInfo = new MovieInfo
                {
                    Name = name,
                    MetadataLanguage = "en",
                };

                try
                {
                    MetadataResult<MediaBrowser.Controller.Entities.Movies.Movie> omdbMovie;

                    var omdbItemProvider = new OmdbItemProvider(Emby.Channel.JsonSerializer, Emby.Channel.HttpClient, Emby.Channel.Logger, Emby.Channel.LibraryManager);

                    omdbMovie = await omdbItemProvider.GetMetadata(movieInfo, cancellationToken);

                    movieItem = omdbMovie.Item;

                    Logger.Debug("omdbMovie.Item.Name: " + movieItem.Name);

                    movieInfo.ProviderIds = movieItem.ProviderIds;

                    MetadataResult<MediaBrowser.Controller.Entities.Movies.Movie> moviedb;

                    var moviedbProvider = new MovieDbProvider(Emby.Channel.JsonSerializer, Emby.Channel.HttpClient, Emby.Channel.FileSystem, Emby.Channel.Config, Emby.Channel.Logger, Emby.Channel.Localization, Emby.Channel.LibraryManager, Emby.Channel.AppHost);

                    moviedb = await moviedbProvider.GetMetadata(movieInfo, cancellationToken);

                    movieItem = moviedb.Item;

                    Logger.Debug("movieDb.Item.Name: " + movieItem.Name);
                    Logger.Debug("movieDb.Item.Overview: " + movieItem.Overview);

                    movie.Name = movieItem.Name;
                    movie.Overview = movieItem.Overview;
                    movie.ProductionYear = movieItem.ProductionYear;
                    movie.Genres = movieItem.Genres;
                    movie.OfficialRating = movieItem.OfficialRating;
                    movie.Studios = movieItem.Studios;
                    movie.ProviderIds = movieItem.ProviderIds;
                }
                catch (Exception exception)
                {}

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
                }
                catch (Exception exception)
                {}

                return movie;
            });
        }
    }
}