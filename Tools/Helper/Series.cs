using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NLog;

namespace PlayOn.Tools.Helper
{
    public class Series
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static bool Detected(Scaffold.Video video)
        {
            return !String.IsNullOrWhiteSpace(video.SeriesName) ||
                   video.Path.Contains("shows") ||
                   video.Path.Contains("|season") ||
                   video.Path.Contains("episodes") ||
                   video.Name.ToLower().StartsWith("season") ||
                   video.Name.ToLower().StartsWith("episode") ||
                   video.Name.ToLower().StartsWith("ep.");
        }

        public static Scaffold.SeasonEpisode SeasonEpisodeExtract(string name, string path)
        {
            var seasonEpisode = new Scaffold.SeasonEpisode();

            name = name.ToLower();

            try
            {
                foreach (var match in Constant.Regex.SeasonEpisode.Select(rule => new Regex(rule)).Select(regex => regex.Match(name)).Where(match => match.Success))
                {
                    seasonEpisode.Season = Convert.ToInt32(match.Groups["season"].Value);
                    seasonEpisode.Episode = Convert.ToInt32(match.Groups["episode"].Value);
                }
            }
            catch (Exception exception)
            {
                //Logger.Error(exception);
            }

            try
            {
                if (seasonEpisode.Season == null || seasonEpisode.Season == 0)
                {
                    foreach (var match in Constant.Regex.Season.Select(rule => new Regex(rule)).Select(regex => regex.Match(name)).Where(match => match.Success))
                    {
                        seasonEpisode.Season = Convert.ToInt32(match.Groups["season"].Value);
                    }
                }
            }
            catch (Exception exception)
            {
                //Logger.Error(exception);
            }

            try
            {
                if (seasonEpisode.Episode == null || seasonEpisode.Episode == 0)
                {
                    foreach (var match in Constant.Regex.Episode.Select(rule => new Regex(rule)).Select(regex => regex.Match(name)).Where(match => match.Success))
                    {
                        seasonEpisode.Episode = Convert.ToInt32(match.Groups["episode"].Value);
                    }

                    if ((seasonEpisode.Season == null || seasonEpisode.Season == 0) &&
                        (seasonEpisode.Episode != null && seasonEpisode.Episode > 0))

                        seasonEpisode.Season = 1;
                }
            }
            catch (Exception exception)
            {
                //Logger.Error(exception);
            }

            return seasonEpisode;
        }
    }
}
