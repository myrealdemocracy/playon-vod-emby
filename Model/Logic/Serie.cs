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
        public static List<Tools.Scaffold.Serie> All()
        {
            var series = new List<Tools.Scaffold.Serie>();

            using (var db = new Ado.PlayOnEntities())
            {
                foreach (var serie in db.Series)
                {
                    var videos = new List<Tools.Scaffold.SeriesVideo>();

                    foreach (var video in serie.VideoSeries)
                    {
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
                        Path = serie.Path,
                        Videos = videos
                    });
                }
            }

            return series;
        }

        public static void Save(Ado.Video video, Ado.Serie serie)
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
                if(db.VideoSeries.Any(q => q.IdVideo == video.Id && q.IdSerie == serie.Id)) return;

                db.VideoSeries.Add(new Ado.VideoSerie
                {
                    Season = season,
                    Episode = episode,
                    Video = video,
                    Serie = serie
                });

                db.SaveChanges();
            }
        }

        public static Ado.Serie Save(string seriesName)
        {
            seriesName = seriesName.Trim();

            using (var db = new Ado.PlayOnEntities())
            {
                var adoSerie = db.Series.FirstOrDefault(q => q.Name == seriesName) ?? new Ado.Serie();

                if (adoSerie.Id == 0)
                {
                    adoSerie.Name = seriesName;
                    adoSerie.Path = seriesName.ToLower() + "|";

                    db.Series.Add(adoSerie);
                }

                db.SaveChanges();

                return adoSerie;
            }
        }
    }
}
