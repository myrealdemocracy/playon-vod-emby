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

        public static void SaveAll(string path = null)
        {
            var url = Tools.Helper.Url.Generate(path);

            var items = String.IsNullOrEmpty(path)
                ? Tools.Helper.Xml.Extractor.Items<Tools.Scaffold.Xml.Catalog>(url).Items
                : Tools.Helper.Xml.Extractor.Items<Tools.Scaffold.Xml.Group>(url).Items;

            foreach (var item in items)
            {
                Generate(item, items, path);
            }
        }

        public static void Generate(Tools.Scaffold.Xml.Item item, List<Tools.Scaffold.Xml.Item> items, string path)
        {
            if (item.Href.Contains("playon") ||
                item.Href.Contains("playmark") ||
                item.Href.Contains("playlater") ||
                item.Name == "Clips" ||
                item.Name == "Video Clips" ||
                item.Name == "Clips & Extras" ||
                item.Name == "Episode Highlights" ||
                item.Name == "Your History" ||
                item.Name == "Your Queue" ||
                item.Name == "Your Subscriptions" ||
                item.Name == "Playback Options" ||
                item.Name.Contains("This folder contains no content"))

                return;

            if (item.Type == "video")
            {
                var videoItem = Tools.Helper.Video.Mapper(item.Href, path);

                var video = Save(videoItem);

                if (video.IsLive == 1) return;

                if (String.IsNullOrEmpty(videoItem.SeriesName))
                {
                    Movie.Save(video);
                }
                else
                {
                    var serie = Serie.Save(videoItem.SeriesName);
                    Serie.Save(video, serie);
                }
            }
            else
                SaveAll(path + item.Name.ToLower() + "|");
        }

        public static Ado.Video Save(Tools.Scaffold.Video video)
        {
            var adoVideo = new Ado.Video();

            try
            {
                using (var db = new Ado.PlayOnEntities())
                {
                    adoVideo = db.Videos.FirstOrDefault(q => q.Path == video.Path);

                    if (adoVideo == null) adoVideo = new Ado.Video();

                    adoVideo.UpdatedAt = DateTime.UtcNow;

                    if (adoVideo.Id == 0)
                    {
                        adoVideo.Name = video.Name;
                        adoVideo.Overview = video.Overview;
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
                }
            }
            catch (Exception exception)
            {
            }

            return adoVideo;
        }
    }
}
