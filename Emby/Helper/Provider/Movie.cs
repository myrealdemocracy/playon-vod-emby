using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Logging;
using MediaBrowser.Providers.Movies;

namespace PlayOn.Emby.Helper.Provider
{
    public class Movie
    {
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
                    MetadataLanguage = "en"
                };

                try
                {
                    var movieDb = await MovieDbProvider.Current.GetMetadata(movieInfo, cancellationToken);

                    movieItem = movieDb.Item;

                    var movieId = movieItem.GetProviderId(MetadataProviders.Tmdb);

                    movieDataPath = MovieDbProvider.GetMovieDataPath(Emby.Channel.Config.ApplicationPaths, MetadataProviders.Tmdb.ToString());
                }
                catch (Exception exception)
                {
                }

                try
                {

                }
                catch (Exception exception)
                {}

                return movie;
            });
        }
    }
}