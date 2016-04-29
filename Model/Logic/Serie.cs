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
    public class Serie
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static List<Tools.Scaffold.Serie> All()
        {
            var series = new List<Tools.Scaffold.Serie>();

            using (var db = new Ado.PlayOnEntities())
            {
                foreach (var serie in db.Series)
                {
                    var seasons = new List<Tools.Scaffold.Season>();

                    for (var season = 0; season < 3000; season++)
                    {
                        var videosSeason = serie.VideoSeries.Where(q => q.Season == season);

                        if(videosSeason.Count() == 0) continue;

                        var videos = new List<Tools.Scaffold.SeriesVideo>();

                        foreach (var video in videosSeason)
                        {
                            videos.Add(new Tools.Scaffold.SeriesVideo
                            {
                                Id = video.IdVideo,
                                Episode = video.Episode,
                                Name = video.Video.Name,
                                Overview = video.Video.Overview,
                                Path = video.Video.Path
                            });
                        }

                        seasons.Add(new Tools.Scaffold.Season
                        {
                            Number = season,
                            Videos = videos
                        });
                    }

                    series.Add(new Tools.Scaffold.Serie
                    {
                        Id = serie.Id,
                        Name = serie.Name,
                        Seasons = seasons
                    });
                }
            }

            return series;
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
                var rule = new Regex(@"s(?<season>[0-9]+)e(?<episode>[0-9]+)\s*");
                var regex = rule.Match(video.Name);
                var season = 0;
                var episode = 0;

                if (regex.Success)
                {
                    season = Convert.ToInt32(regex.Groups["season"].Value);
                    episode = Convert.ToInt32(regex.Groups["episode"].Value);
                }

                using (var db = new Ado.PlayOnEntities())
                {
                    var adoVideoSerie = db.VideoSeries.FirstOrDefault(q => q.IdVideo == video.Id && q.IdSerie == serie.Id);

                    if (adoVideoSerie != null) return;

                    db.VideoSeries.Add(new Ado.VideoSerie
                    {
                        IdVideo = video.Id,
                        IdSerie = serie.Id,
                        Season = season,
                        Episode = episode,
                        //Video = video,
                        //Serie = serie
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
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        }
    }
}
