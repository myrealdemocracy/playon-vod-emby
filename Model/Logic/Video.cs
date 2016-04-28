using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PlayOn.Model.Logic
{
    public class Video
    {
        public static void SaveAll()
        {
            foreach (var video in Tools.Helper.Video.All())
            {
                var isSeries = !String.IsNullOrEmpty(video.Series);
                var rule = new Regex(@"s(?<season>[0-9]+)e(?<episode>[0-9]+)\s*");
                var regex = rule.Match(video.Name);
                var season = 0;
                var episode = 0;

                if (regex.Success)
                {
                    season = Convert.ToInt32(regex.Groups["season"].Value);
                    episode = Convert.ToInt32(regex.Groups["episode"].Value);
                }
            }
        }
    }
}
