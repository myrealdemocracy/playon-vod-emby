using System;
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

        public static List<Tools.Scaffold.Series> All()
        {
            var series = new List<Tools.Scaffold.Series>();

            using (var db = new Ado.PlayOnEntities())
            {
                foreach (var serie in db.Series)
                {
                    series.Add(new Tools.Scaffold.Series
                    {
                        Id = serie.Id,
                        Name = serie.Name
                    });
                }
            }

            return series;
        }

        public static List<Tools.Scaffold.Season> ByName(string name)
        {
            var seasons = new List<Tools.Scaffold.Season>();

            try
            {
                using (var db = new Ado.PlayOnEntities())
                {
                    var series = db.Series.FirstOrDefault(q => q.Name == name);
                    var seasonsList = series.VideoSeries.GroupBy(g => g.Season).Select(s => s.First());

                    foreach (var season in seasonsList)
                    {
                        seasons.Add(new Tools.Scaffold.Season
                        {
                            SeasonNumber = season.Season
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

        public static List<Tools.Scaffold.Episode> BySeason(string name, int? season)
        {
            var episodes = new List<Tools.Scaffold.Episode>();

            try
            {
                using (var db = new Ado.PlayOnEntities())
                {
                    var series = db.Series.FirstOrDefault(q => q.Name == name);
                    var episodeList = series.VideoSeries.Where(q => q.Season == season).GroupBy(g => g.Episode).Select(s => s.First());

                    foreach (var episode in episodeList)
                    {
                        var episodeNumber = Convert.ToInt32(episode.Episode);
                        var videos = new List<Tools.Scaffold.Video>();

                        if (episodeNumber == 0)
                        {
                            var dbVideos = db.Videos.Where(q => q.VideoSeries.Any(a => a.IdSerie == series.Id && a.Season == season && a.Episode == episodeNumber));

                            foreach (var video in dbVideos)
                            {
                                videos.Add(new Tools.Scaffold.Video
                                {
                                    Name = video.Name,
                                    Overview = video.Overview,
                                    Path = video.Path
                                });
                            }
                        }

                        episodes.Add(new Tools.Scaffold.Episode
                        {
                            EpisodeNumber = episodeNumber,
                            Videos = videos
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

        public static string VideoByNameSeasonEpisode(string name, int? season, int? episode)
        {
            var url = String.Empty;

            try
            {
                using (var db = new Ado.PlayOnEntities())
                {
                    var videos = db.Videos.Where(q => q.VideoSeries.Any(a => a.Serie.Name == name && a.Season == season && a.Episode == episode));

                    foreach (var video in videos)
                    {
                        url = Tools.Helper.Url.Generate(video.Path + "video|");

                        Logger.Debug("url: " + url);

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

        public static Ado.Serie Save(string seriesName)
        {
            var adoSerie = new Ado.Serie();

            try
            {
                seriesName = seriesName.Trim();

                using (var db = new Ado.PlayOnEntities())
                {
                    adoSerie = db.Series.FirstOrDefault(q => q.Name == seriesName);

                    if (adoSerie == null) adoSerie = new Ado.Serie();

                    if (adoSerie.Id == 0)
                    {
                        adoSerie.Name = seriesName;

                        db.Series.Add(adoSerie);

                        db.SaveChanges();
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
