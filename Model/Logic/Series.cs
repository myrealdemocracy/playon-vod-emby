﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NLog;

namespace PlayOn.Model.Logic
{
    public class Series
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static Tools.Scaffold.Result.Series All(int start, int end)
        {
            var series = new List<Tools.Scaffold.Series>();
            var totalRecordCount = 0;

            end = end == 0 ? 10000 : end;

            using (var db = new Ado.PlayOnEntities())
            {
                totalRecordCount = db.Series.Count();

                foreach (var seriesItem in db.Series.OrderBy(o => o.Name).Skip(start).Take(end))
                {
                    var yesterday = DateTime.UtcNow.AddDays(-1);
                    var lastWeek = DateTime.UtcNow.AddDays(-7);
                    var videos = db.Videos.Count(q => q.VideoSeries.Any(a => a.IdSerie == seriesItem.Id) && ((q.UpdatedAt >= yesterday && q.Provider.Code != "netflix") || (q.UpdatedAt >= lastWeek && q.Provider.Code == "netflix")));

                    series.Add(new Tools.Scaffold.Series
                    {
                        Id = seriesItem.Id,
                        Name = seriesItem.Name,
                        ImdbId = seriesItem.Imdb,
                        Deleted = videos < 1
                    });
                }
            }

            return new Tools.Scaffold.Result.Series
            {
                Items = series,
                TotalRecordCount = totalRecordCount
            };
        }

        public static List<Tools.Scaffold.Season> ByName(string imdbId)
        {
            var seasons = new List<Tools.Scaffold.Season>();

            try
            {
                using (var db = new Ado.PlayOnEntities())
                {
                    var series = db.Series.FirstOrDefault(q => q.Imdb == imdbId);
                    var seasonsList = series.VideoSeries.GroupBy(g => g.Season).Select(s => s.First());

                    foreach (var season in seasonsList)
                    {
                        var seasonNumber = Convert.ToInt32(season.Season);

                        if (Convert.ToInt32(seasonNumber) == 0) continue;

                        var yesterday = DateTime.UtcNow.AddDays(-1);
                        var lastWeek = DateTime.UtcNow.AddDays(-7);
                        var videos = db.Videos.Count(q => q.VideoSeries.Any(a => a.IdSerie == series.Id && a.Season == seasonNumber) && ((q.UpdatedAt >= yesterday && q.Provider.Code != "netflix") || (q.UpdatedAt >= lastWeek && q.Provider.Code == "netflix")));

                        seasons.Add(new Tools.Scaffold.Season
                        {
                            SeriesName = series.Name,
                            SeasonNumber = seasonNumber,
                            Deleted = videos < 1
                        });
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }

            return seasons;
        }

        public static List<Tools.Scaffold.Episode> BySeason(string imdbId, int? season)
        {
            var episodes = new List<Tools.Scaffold.Episode>();

            try
            {
                using (var db = new Ado.PlayOnEntities())
                {
                    var series = db.Series.FirstOrDefault(q => q.Imdb == imdbId);
                    var episodeList = series.VideoSeries.Where(q => q.Season == season).GroupBy(g => g.Episode).Select(s => s.First());

                    foreach (var episode in episodeList)
                    {
                        var episodeNumber = Convert.ToInt32(episode.Episode);

                        if (episodeNumber == 0) continue;

                        var yesterday = DateTime.UtcNow.AddDays(-1);
                        var lastWeek = DateTime.UtcNow.AddDays(-7);
                        var videos = db.Videos.Count(q => q.VideoSeries.Any(a => a.IdSerie == series.Id && a.Season == season && a.Episode == episodeNumber) && ((q.UpdatedAt >= yesterday && q.Provider.Code != "netflix") || (q.UpdatedAt >= lastWeek && q.Provider.Code == "netflix")));

                        episodes.Add(new Tools.Scaffold.Episode
                        {
                            SeriesName = series.Name,
                            EpisodeNumber = episodeNumber,
                            Deleted = videos < 1
                        });
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }

            return episodes;
        }

        public static string VideoByImdbIdSeasonEpisode(string imdbId, int? season, int? episode)
        {
            var url = String.Empty;

            try
            {
                using (var db = new Ado.PlayOnEntities())
                {
                    var videos = db.Videos.Where(q => q.VideoSeries.Any(a => a.Serie.Imdb == imdbId && a.Season == season && a.Episode == episode)).OrderBy(o => o.FailingCount);

                    url = Video.Url(videos);
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }

            return url;
        }

        public static Ado.Serie Save(string seriesName, int? minutes)
        {
            var adoSerie = new Ado.Serie();

            try
            {
                seriesName = seriesName.Trim();

                using (var db = new Ado.PlayOnEntities())
                {
                    adoSerie = db.Series.FirstOrDefault(q => q.Name == seriesName);

                    if (adoSerie == null)
                    {
                        var imdb = "";
                        var omdbList = Tools.Helper.Omdb.Search(seriesName, "series").Search;

                        foreach (var series in omdbList.OrderBy(o => o.YearStarted))
                        {
                            var min = minutes - 20;
                            var max = minutes * 2;

                            if (series.Runtime == @"N/A" && omdbList.Count == 1)
                            {
                                min = series.Minutes;
                                max = series.Minutes;
                            }

                            if (!String.Equals(series.Title, seriesName, StringComparison.InvariantCultureIgnoreCase) ||
                                series.Minutes < min || 
                                series.Minutes > max) continue;

                            imdb = series.ImdbId;
                        }

                        if (!String.IsNullOrWhiteSpace(imdb))
                        {
                            adoSerie = new Ado.Serie
                            {
                                Name = seriesName,
                                Imdb = imdb
                            };

                            db.Series.Add(adoSerie);

                            db.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }

            return adoSerie;
        }

        public static void Save(Ado.Video video, Ado.Serie serie)
        {
            try
            {
                var seasonEpisode = Tools.Helper.Series.SeasonEpisodeExtract(video.Name, video.Path);

                using (var db = new Ado.PlayOnEntities())
                {
                    video = db.Videos.FirstOrDefault(q => q.Id == video.Id);

                    serie = db.Series.FirstOrDefault(q => q.Id == serie.Id);

                    var adoVideoSerie = db.VideoSeries.FirstOrDefault(q => q.IdVideo == video.Id && q.IdSerie == serie.Id);

                    if (adoVideoSerie != null) return;

                    db.VideoSeries.Add(new Ado.VideoSerie
                    {
                        IdVideo = video.Id,
                        IdSerie = serie.Id,
                        Season = seasonEpisode.Season,
                        Episode = seasonEpisode.Episode
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

                    serie = db.Series.FirstOrDefault(q => q.Id == serie.Id);

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

                        if (serie.Categories.Any(q => q.Id == category.Id)) continue;

                        serie.Categories.Add(category);
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
