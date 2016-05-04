using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NLog;

namespace PlayOn.Tools.Helper
{
    public class Video
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static Scaffold.Video Mapper(string url, string path)
        {
            var video = Xml.Extractor.Items<Scaffold.Xml.Video>(url);

            if (String.IsNullOrEmpty(video.Item?.Name)) return new Scaffold.Video();

            path = path + video.Item.Name.ToLower() + "|";

            Logger.Debug("path: " + path);

            if (String.IsNullOrEmpty(video.Series?.Name)) video.Series = new Scaffold.Xml.Info { Name = String.Empty };

            if (String.IsNullOrEmpty(video.Description?.Name)) video.Description = new Scaffold.Xml.Info { Name = String.Empty };

            if (String.IsNullOrEmpty(video.Time?.Name)) video.Time = new Scaffold.Xml.Info { Name = String.Empty };

            var name = video.Item.Name;

            var minutes = 0;

            if (video.Time.Name.Contains(":"))
            {
                var terms = video.Time.Name.Split(Convert.ToChar(":"));

                var count = 0;

                foreach (var term in terms)
                {
                    Logger.Debug("term[" + count + "]: " + term);

                    count++;
                }

                minutes = (Convert.ToInt32(terms[0]) * 60) + Convert.ToInt32(terms[1]) + (Convert.ToInt32(terms[2]) / 60);

                Logger.Debug("minutes: " + minutes);
            }

            if (name.Contains(video.Series.Name)) name = name.Replace(video.Series.Name + " - ", "");

            return new Scaffold.Video
            {
                Path = path,
                Name = name,
                Overview = video.Description.Name,
                IsLive = path.Contains("|live|") || path.Contains("|live tv|") || path.Contains("|live stream|"),
                Minutes = minutes,
                SeriesName = video.Series.Name
            };
        }
    }
}
