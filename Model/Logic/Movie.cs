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
            return new List<Tools.Scaffold.Movie>();
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
