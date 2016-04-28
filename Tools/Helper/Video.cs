using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PlayOn.Tools.Helper
{
    public class Video
    {
        public static List<Scaffold.Video> All(string path = null)
        {
            var url = Url.Generate(path);

            var videos = new List<Scaffold.Video>();

            var items = String.IsNullOrEmpty(path)
                ? Xml.Extractor.Items<Scaffold.Xml.Catalog>(url).Items
                : Xml.Extractor.Items<Scaffold.Xml.Group>(url).Items;

            foreach (var item in items)
            {
                videos.AddRange(Generate(item, items, path));
            }

            return videos;
        }

        public static List<Scaffold.Video> Generate(Scaffold.Xml.Item item, List<Scaffold.Xml.Item> items, string path)
        {
            var videos = new List<Scaffold.Video>();

            if (item.Href.Contains("playon") ||
                item.Href.Contains("playmark") ||
                item.Href.Contains("playlater") ||
                item.Name == "Clips" ||
                item.Name == "Video Clips" ||
                item.Name == "Clips & Extras" ||
                item.Name == "Episode Highlights" ||
                item.Name.Contains("This folder contains no content"))

                return videos;

            if (item.Type == "video")
                videos.Add(Mapper(item.Href, path));
            else
                videos.AddRange(All(path + item.Name.ToLower() + "|"));

            return videos;
        }

        public static Scaffold.Video Mapper(string url, string path)
        {
            var video = Xml.Extractor.Items<Scaffold.Xml.Video>(url);

            if (String.IsNullOrEmpty(video.Item?.Name)) return new Scaffold.Video();

            path = path + video.Item.Name.ToLower() + "|";

            if (String.IsNullOrEmpty(video.Series?.Name)) video.Series = new Scaffold.Xml.Info { Name = String.Empty };

            if (String.IsNullOrEmpty(video.Description?.Name)) video.Description = new Scaffold.Xml.Info { Name = String.Empty };

            var name = video.Item.Name;

            if (name.Contains(video.Series.Name)) name = name.Replace(video.Series.Name + " - ", "");

            return new Scaffold.Video
            {
                Path = path,
                Name = name,
                Overview = video.Description.Name,
                SeriesName = video.Series.Name
            };
        }
    }
}
