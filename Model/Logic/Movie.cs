using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using NLog.Fluent;

namespace PlayOn.Model.Logic
{
    public class Movie
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static List<Tools.Scaffold.Movie> All()
        {
            var movies = new List<Tools.Scaffold.Movie>();

            using (var db = new Ado.PlayOnEntities())
            {
                foreach (var movie in db.Movies)
                {
                    var videos = new List<Tools.Scaffold.Video>();

                    foreach (var video in movie.Videos)
                    {
                        videos.Add(new Tools.Scaffold.Video
                        {
                            Name = video.Name,
                            Path = video.Path
                        });
                    }

                    movies.Add(new Tools.Scaffold.Movie
                    {
                        Id = movie.Id,
                        Name = movie.Name,
                        Videos = videos
                    });
                }
            }

            return movies;
        }

        public static List<Tools.Scaffold.Movie> ByCategory(string category)
        {
            throw new NotImplementedException();
        }

        public static List<Tools.Scaffold.Video> ByName(string category)
        {
            throw new NotImplementedException();
        }

        public static void Save(Ado.Video video, Ado.Movie movie)
        {
            try
            {
                using (var db = new Ado.PlayOnEntities())
                {
                    video = db.Videos.FirstOrDefault(q => q.Id == video.Id);

                    movie = db.Movies.FirstOrDefault(q => q.Id == movie.Id);
                    movie.Videos.Add(video);

                    db.Entry(movie).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }

            try
            {
                using (var db = new Ado.PlayOnEntities())
                {
                    video = db.Videos.FirstOrDefault(q => q.Id == video.Id);

                    movie = db.Movies.FirstOrDefault(q => q.Id == movie.Id);

                    foreach (var item in Tools.Constant.Category.Items)
                    {
                        var isIn = false;

                        foreach (var name in item.Value)
                        {
                            isIn = video.Path.Contains(name);

                            if (isIn) break;
                        }

                        if (!isIn) continue;

                        var category = db.Categories.FirstOrDefault(q => q.Name == item.Key);

                        if(movie.Categories.Any(q => q.Id == category.Id)) continue;

                        movie.Categories.Add(category);
                    }

                    db.Entry(movie).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        }

        public static Ado.Movie Save(string name)
        {
            var adoMovie = new Ado.Movie();

            try
            {
                using (var db = new Ado.PlayOnEntities())
                {
                    adoMovie = db.Movies.FirstOrDefault(q => q.Name == name);

                    if (adoMovie == null)
                    {
                        adoMovie = new Ado.Movie
                        {
                            Name = name
                        };

                        db.Movies.Add(adoMovie);

                        db.SaveChanges();
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }

            return adoMovie;
        }
    }
}
