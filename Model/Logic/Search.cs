using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace PlayOn.Model.Logic
{
    public class Search
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static List<Tools.Scaffold.Video> ByName(Tools.Scaffold.Form.Search search)
        {
            var videos = new List<Tools.Scaffold.Video>();

            try
            {
                using (var db = new Ado.PlayOnEntities())
                {
                    foreach (var movie in db.Movies)
                    {
                        videos.Add(new Tools.Scaffold.Video
                        {
                            Type = "movies",
                            Name = movie.Name,
                            ImdbId = movie.Imdb
                        });
                    }

                    foreach (var series in db.Series)
                    {
                        videos.Add(new Tools.Scaffold.Video
                        {
                            Type = "series",
                            Name = series.Name,
                            ImdbId = series.Imdb
                        });
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }

            return videos;
        }
    }
}
