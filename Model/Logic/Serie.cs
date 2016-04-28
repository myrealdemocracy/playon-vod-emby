using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PlayOn.Model.Logic
{
    public class Serie
    {
        public static List<Tools.Scaffold.Serie> All(int season = 0)
        {
            var series = new List<Tools.Scaffold.Serie>();

            using (var db = new Ado.PlayOnEntities())
            {
                foreach (var serie in db.Series)
                {
                    var videos = new List<Tools.Scaffold.SeriesVideo>();

                    foreach (var video in serie.VideoSeries)
                    {
                        if(season != 0 && season != video.Season) continue;

                        videos.Add(new Tools.Scaffold.SeriesVideo
                        {
                            Id = video.IdVideo,
                            Name = video.Video.Name,
                            Overview = video.Video.Overview,
                            Path = video.Video.Path,
                            Season = video.Season,
                            Episode = video.Episode
                        });
                    }

                    series.Add(new Tools.Scaffold.Serie
                    {
                        Id = serie.Id,
                        Name = serie.Name,
                        Videos = videos
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
            }
        }
    }
}
