using System;
using System.Collections;
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

            try
            {
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
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }

            return videos;
        }

        public static string Url(IEnumerable<Ado.Video> videos)
        {
            var url = String.Empty;

            try
            {
                foreach (var video in videos)
                {
                    url = Tools.Helper.Url.Generate(video.Path + "video|");

                    Logger.Debug("url: " + url);

                    if (url.Contains("m3u8") || url.Contains("flv")) break;
                }

                url = url.Contains("xml") ? "" : url;
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }

            return url;
        }

        public static void SaveAll(string url = null, string path = null)
        {
            url = String.IsNullOrEmpty(url) ? Tools.Constant.Url.Xml : url;

            var items = String.IsNullOrEmpty(path) ? 
                Tools.Helper.Xml.Extractor.Items<Tools.Scaffold.Xml.Catalog>(url).Items :
                Tools.Helper.Xml.Extractor.Items<Tools.Scaffold.Xml.Group>(url).Items;

            foreach (var item in items)
            {
                var nextPath = (String.IsNullOrWhiteSpace(path) ? 
                    item.Href.Split(Convert.ToChar("="))[1] : 
                    path + item.Name.ToLower()) + "|";

                Logger.Debug("path: " + path);
                Logger.Debug("nextPath: " + nextPath);
                Logger.Debug("item.Name: " + item.Name);
                Logger.Debug("item.Type: " + item.Type);
                Logger.Debug("item.Href: " + item.Href);

                if (Tools.Helper.Ignore.Item(items, item)) continue;

                if (String.IsNullOrWhiteSpace(path))
                {
                    Provider.Save(item);
                }

                if (item.Type != "video")
                {
                    Logger.Debug("Next loop");

                    SaveAll(item.Href, nextPath);
                }
                else
                {
                    Logger.Debug("Saving video");

                    var videoItem = Tools.Helper.Video.Mapper(item.Href, path);

                    if (Convert.ToInt32(videoItem.Minutes) <= 5)
                    {
                        Logger.Debug("Ignoring " + videoItem.Minutes + " minute(s) video");

                        return;
                    }

                    var video = Save(videoItem);

                    if (video.IsLive == 1)
                    {
                        Logger.Debug("Video is live feed");

                        return;
                    }

                    if (!Tools.Helper.Series.Detected(videoItem))
                    {
                        var movie = Movie.Save(videoItem.Name);
                        Movie.Save(video, movie);
                    }
                    else if (Tools.Helper.Series.Detected(videoItem) && !String.IsNullOrWhiteSpace(videoItem.SeriesName))
                    {
                        var serie = Series.Save(videoItem.SeriesName);
                        Series.Save(video, serie);
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
                    adoVideo.FailingCount = 0;

                    var providerCode = video.Path.Split(Convert.ToChar("|"))[0];

                    if (adoVideo.Id == 0)
                    {
                        adoVideo.IdProvider = db.Providers.FirstOrDefault(q => q.Code == providerCode).Id;
                        adoVideo.Name = video.Name;
                        adoVideo.Overview = video.Overview;
                        adoVideo.Minutes = video.Minutes;
                        adoVideo.Path = video.Path;
                        adoVideo.IsLive = video.IsLive ? 1 : 0;
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
