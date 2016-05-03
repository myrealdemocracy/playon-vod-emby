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
        public static Scaffold.Video Mapper(string url, string path)
        {
            var video = Xml.Extractor.Items<Scaffold.Xml.Video>(url);

            if (String.IsNullOrEmpty(video.Item?.Name)) return new Scaffold.Video();

            path = path + video.Item.Name.ToLower() + "|";

            if (String.IsNullOrEmpty(video.Series?.Name)) video.Series = new Scaffold.Xml.Info { Name = String.Empty };

            if (String.IsNullOrEmpty(video.Description?.Name)) video.Description = new Scaffold.Xml.Info { Name = String.Empty };

            if (String.IsNullOrEmpty(video.Time?.Name)) video.Time = new Scaffold.Xml.Info { Name = String.Empty };

            var name = video.Item.Name;

            var minutes = 0;

            if (video.Time.Name.Contains(":"))
            {
                var terms = video.Time.Name.Split(Convert.ToChar(":"));

                minutes = (Convert.ToInt32(terms[0]) * 60) + Convert.ToInt32(terms[1]);
            }

            if (name.Contains(video.Series.Name)) name = name.Replace(video.Series.Name + " - ", "");

            return new Scaffold.Video
            {
                Path = path,
                Name = name,
                Overview = video.Description.Name,
                Minutes = minutes,
                SeriesName = video.Series.Name
            };
        }
    }
}
