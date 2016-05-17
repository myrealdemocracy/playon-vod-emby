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

        public static string Url(IEnumerable<Ado.Video> videos)
        {
            var url = String.Empty;

            try
            {
                foreach (var video in videos.Where(q => q.Provider.Searchable != 0))
                {
                    if (video.VideoSeries.Any())
                    {
                        var series = video.VideoSeries.FirstOrDefault().Serie.Name;

                        url = Tools.Helper.Url.Search(video.Provider.Code, video.Name, series);
                    }
                    else
                    {
                        url = Tools.Helper.Url.Search(video.Provider.Code, video.Name);
                    }

                    if (url.Contains("m3u8") || url.Contains("flv")) break;
                }

                Logger.Debug("after search - url: " + url);

                if (String.IsNullOrEmpty(url) || url == Tools.Constant.Url.Base)
                {
                    foreach (var video in videos)
                    {
                        url = Tools.Helper.Url.Generate(video.Path + "video|");

                        Logger.Debug("loop - url: " + url);

                        if (url.Contains("m3u8") || url.Contains("flv")) break;
                    }

                    Logger.Debug("after path - url: " + url);

                    url = url.Contains("xml") ? "" : url;
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }

            Logger.Debug("return - url: " + url);

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
                        var movie = Movie.Save(videoItem.Name, videoItem.Minutes);

                        if (movie != null) Movie.Save(video, movie);
                    }
                    else if (Tools.Helper.Series.Detected(videoItem) && !String.IsNullOrWhiteSpace(videoItem.SeriesName))
                    {
                        var series = Series.Save(videoItem.SeriesName, videoItem.Minutes);

                        if(series != null) Series.Save(video, series);
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

        public static void DeleteOld()
        {
            try
            {
                using (var db = new Ado.PlayOnEntities())
                {
                    var yesterday = DateTime.UtcNow.AddDays(-1);

                    foreach (var video in db.Videos.Where(q => q.UpdatedAt <= yesterday))
                    {
                        Logger.Debug("video.Name: " + video.Name);
                        Logger.Debug("video.Path: " + video.Path);
                        Logger.Debug("video.UpdatedAt: " + video.UpdatedAt);

                        db.Videos.Remove(video);
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
