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

        public static List<Tools.Scaffold.Movie> All(string letter = null)
        {
            var movies = new List<Tools.Scaffold.Movie>();

            try
            {
                using (var db = new Ado.PlayOnEntities())
                {
                    IQueryable<Ado.Movie> dbMovies = db.Movies;

                    if (!String.IsNullOrEmpty(letter))
                    {
                        dbMovies = letter != "nmb" ?
                            db.Movies.Where(q => q.Name.StartsWith(letter)) :
                            db.Movies.Where(q => q.Name.StartsWith("0") || q.Name.StartsWith("1") || q.Name.StartsWith("2") || q.Name.StartsWith("3") || q.Name.StartsWith("4") || q.Name.StartsWith("5") || q.Name.StartsWith("6") || q.Name.StartsWith("7") || q.Name.StartsWith("8") || q.Name.StartsWith("9"));
                    }

                    foreach (var movie in dbMovies)
                    {
                        movies.Add(new Tools.Scaffold.Movie
                        {
                            Id = movie.Id,
                            Name = movie.Name,
                            Overview = movie.VideoMovies.FirstOrDefault(q => q.Movie.Id == movie.Id).Video.Overview
                        });
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }

            return movies;
        }

        public static string VideoByName(string name)
        {
            var url = String.Empty;

            try
            {
                using (var db = new Ado.PlayOnEntities())
                {
                    var videos = db.Videos.Where(q => q.VideoMovies.Any(a => a.Movie.Name == name));

                    foreach (var video in videos)
                    {
                        url = Tools.Helper.Url.Generate(video.Path + "video|");

                        if (url.Contains("m3u8")) break;
                    }

                    url = url.Contains("xml") ? "" : url;
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }

            return url;
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
