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
    public class Video
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

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
            const string url = Tools.Helper.Url.Xml;
            var items = Tools.Helper.Xml.Extractor.Items<Tools.Scaffold.Xml.Catalog>(url).Items;

            foreach (var item in items)
            {
                Logger.Debug("item.Name: " + item.Name);
                Logger.Debug("item.Href: " + item.Href);

                if (Tools.Helper.Ignore.Item(item))
                {
                    Logger.Debug("ignoring");

                    continue;
                }

                Logger.Debug("calling SaveLoop");

                SaveLoop(item.Href.Split(Convert.ToChar("="))[1] + "|");
            }
        }

        public static void SaveLoop(string path)
        {
            var url = Tools.Helper.Url.Generate(path);

            Logger.Debug("path: " + path);
            Logger.Debug("url: " + url);
            Logger.Debug("calling SaveForEach");

            SaveForEach(url, path);
        }

        public static void SaveForEach(string url, string path)
        {
            foreach (var item in Tools.Helper.Xml.Extractor.Items<Tools.Scaffold.Xml.Group>(url).Items)
            {
                var nextPath = path + item.Name.ToLower() + "|";

                Logger.Debug("path: " + path);
                Logger.Debug("nextPath: " + nextPath);
                Logger.Debug("item.Name: " + item.Name);
                Logger.Debug("item.Type: " + item.Type);
                Logger.Debug("item.Href: " + item.Href);


                if (Tools.Helper.Ignore.Item(item) ||
                    String.IsNullOrEmpty(nextPath) ||
                    path == nextPath)
                {
                    Logger.Debug("ignoring");
                    continue;
                }

                if (item.Type != "video")
                {
                    Logger.Debug("next loop");

                    SaveLoop(nextPath);
                }
                else
                {
                    Logger.Debug("saving video");

                    var videoItem = Tools.Helper.Video.Mapper(item.Href, path);

                    var video = Save(videoItem);

                    if (video.IsLive == 1) return;

                    if (String.IsNullOrEmpty(videoItem.SeriesName))
                    {
                        var movie = Movie.Save(videoItem.Name);
                        Movie.Save(video, movie);
                    }
                    else
                    {
                        var serie = Serie.Save(videoItem.SeriesName);
                        Serie.Save(video, serie);
                    }
                }
            }
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

                    var pathTerms = video.Path.Split(Convert.ToChar("|"));

                    if (adoVideo.Id == 0)
                    {
                        adoVideo.Name = video.Name;
                        adoVideo.Overview = video.Overview;
                        adoVideo.Path = video.Path;
                        adoVideo.Provider = pathTerms[0];
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
                Logger.Error(exception);
            }

            return adoVideo;
        }
    }
}
