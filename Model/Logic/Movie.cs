﻿using System;
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

        public static Tools.Scaffold.Result.Movie All(int start, int end)
        {
            var movies = new List<Tools.Scaffold.Movie>();
            var totalRecordCount = 0;

            end = end == 0 ? 10000 : end;

            try
            {
                using (var db = new Ado.PlayOnEntities())
                {
                    totalRecordCount = db.Movies.Count();

                    foreach (var movie in db.Movies.OrderBy(o => o.Name).Skip(start).Take(end))
                    {
                        var yesterday = DateTime.UtcNow.AddDays(-1);
                        var lastWeek = DateTime.UtcNow.AddDays(-7);
                        var videos = db.Videos.Count(q => q.VideoMovies.Any(a => a.IdMovie == movie.Id) && ((q.UpdatedAt >= yesterday && q.Provider.Code != "netflix") || (q.UpdatedAt >= lastWeek && q.Provider.Code == "netflix")));

                        movies.Add(new Tools.Scaffold.Movie
                        {
                            Id = movie.Id,
                            Name = movie.Name,
                            ImdbId = movie.Imdb,
                            Deleted = videos < 1
                        });
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }

            return new Tools.Scaffold.Result.Movie
            {
                Items = movies,
                TotalRecordCount = totalRecordCount
            };
        }

        public static string VideoByImdbId(string imdbId)
        {
            var url = String.Empty;

            try
            {
                using (var db = new Ado.PlayOnEntities())
                {
                    var videos = db.Videos.Where(q => q.VideoMovies.Any(a => a.Movie.Imdb == imdbId)).OrderBy(o => o.FailingCount);

                    url = Video.Url(videos);
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }

            return url;
        }

        public static Ado.Movie Save(string name, int? minutes)
        {
            var adoMovie = new Ado.Movie();

            try
            {
                using (var db = new Ado.PlayOnEntities())
                {
                    adoMovie = db.Movies.FirstOrDefault(q => q.Name == name);

                    if (adoMovie == null)
                    {
                        var imdb = "";
                        var omdbList = Tools.Helper.Omdb.Search(name, "movie").Search;

                        foreach (var movie in omdbList.OrderBy(o => o.YearStarted))
                        {
                            var min = minutes - 10;
                            var max = minutes + 10;

                            if (!String.Equals(movie.Title, name, StringComparison.InvariantCultureIgnoreCase) || movie.Minutes < min || movie.Minutes > max) continue;

                            imdb = movie.ImdbId;
                        }

                        if (!String.IsNullOrWhiteSpace(imdb))
                        {

                            adoMovie = new Ado.Movie
                            {
                                Name = name,
                                Imdb = imdb
                            };

                            db.Movies.Add(adoMovie);

                            db.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }

            return adoMovie;
        }

        public static void Save(Ado.Video video, Ado.Movie movie)
        {
            try
            {
                using (var db = new Ado.PlayOnEntities())
                {
                    video = db.Videos.FirstOrDefault(q => q.Id == video.Id);

                    movie = db.Movies.FirstOrDefault(q => q.Id == movie.Id);

                    var adoVideoMovie = db.VideoMovies.FirstOrDefault(q => q.IdVideo == video.Id && q.IdMovie == movie.Id);

                    if (adoVideoMovie != null) return;

                    movie.VideoMovies.Add(new Ado.VideoMovie
                    {
                        IdVideo = video.Id,
                        IdMovie = movie.Id
                    });
                    
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

                        if (movie.Categories.Any(q => q.Id == category.Id)) continue;

                        movie.Categories.Add(category);
                    }
                    
                    db.SaveChanges();
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        }
    }
}
