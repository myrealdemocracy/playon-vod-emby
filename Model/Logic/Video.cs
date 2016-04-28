using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PlayOn.Model.Logic
{
    public class Video
    {
        public static List<Tools.Scaffold.Video> All()
        {
            var videos = new List<Tools.Scaffold.Video>();

            using (var db = new Ado.PlayOnEntities())
            {
                foreach (var video in db.Videos)
                {
                    videos.Add(new Tools.Scaffold.Video
                    {
                        Id = video.Id,
                        Name = video.Name,
                        Overview = video.Overview,
                        Path = video.Path
                    });
                }
            }

            return videos;
        }

        public static void SaveAll()
        {
            foreach (var item in Tools.Helper.Video.All("nativefox|"))
            {
                var video = Save(item);

                if(video.IsLive == 1) continue;

                if (String.IsNullOrEmpty(item.SeriesName))
                    Movie.Save(video);
                else
                {
                    var serie = Serie.Save(item.SeriesName);
                    Serie.Save(video, serie);
                }
            }
        }

        public static Ado.Video Save(Tools.Scaffold.Video video)
        {
            if(String.IsNullOrEmpty(video?.Path)) return new Ado.Video();

            using (var db = new Ado.PlayOnEntities())
            {
                var adoVideo = db.Videos.FirstOrDefault(q => q.Path == video.Path) ?? new Ado.Video();

                adoVideo.UpdatedAt = DateTime.UtcNow;

                if (adoVideo.Id == 0)
                {
                    adoVideo.Name = video.Name;
                    adoVideo.Path = video.Path;
                    adoVideo.Provider = video.Path.Split(Convert.ToChar("|"))[0];
                    adoVideo.IsLive = video.Path.Contains("|live|") || video.Path.Contains("|live tv|") ? 1 : 0;
                    adoVideo.CreatedAt = DateTime.UtcNow;

                    db.Videos.Add(adoVideo);
                }
                else
                {
                    db.Entry(adoVideo).State = EntityState.Modified;
                }

                db.SaveChanges();

                return adoVideo;
            }
        }
    }
}
