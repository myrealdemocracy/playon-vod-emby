using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;

namespace PlayOn.Tools.Helper
{
    public class Omdb
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static Scaffold.Omdb Search(string title, string type)
        {
            var results = new Scaffold.Omdb();
            var url = Constant.Url.Omdb + "?type=" + type + "&s=" + WebUtility.UrlEncode(title.ToLower());

            Logger.Debug("url: " + url);

            try
            {
                var client = new WebClient();
                var data = client.DownloadString(url);
                var omdbResult = JsonConvert.DeserializeObject<Scaffold.Omdb>(data);

                results.Search = new List<Scaffold.Result.Omdb>();

                foreach (var result in omdbResult.Search)
                {
                    Logger.Debug("result.Title: " + result.Title);
                    Logger.Debug("result.ImdbId: " + result.ImdbId);

                    results.Search.Add(Imdb(result.ImdbId));
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }

            return results;
        }

        public static Scaffold.Result.Omdb Imdb(string imdbId)
        {
            var result = new Scaffold.Result.Omdb();
            var url = Constant.Url.Omdb + "?i=" + imdbId;

            Logger.Debug("url: " + url);

            try
            {
                var client = new WebClient();
                client.Encoding = Encoding.UTF8;

                var data = client.DownloadString(url);

                result = JsonConvert.DeserializeObject<Scaffold.Result.Omdb>(data);

                Logger.Debug("result.Year: " + result.Year);

                if (result.Year.Contains("–"))
                {
                    var years = result.Year.Split(Convert.ToChar("–"));

                    result.YearStarted = Convert.ToInt32(years[0]);
                    result.YearEnded = Convert.ToInt32(String.IsNullOrWhiteSpace(years[1]) ? "0" : years[1]);
                }
                else
                {
                    result.YearStarted = Convert.ToInt32(result.Year);
                }

                Logger.Debug("result.YearStarted: " + result.YearStarted);
                Logger.Debug("result.YearEnded: " + result.YearEnded);

                Logger.Debug("result.Runtime: " + result.Runtime);

                var runtime = result.Runtime.Contains("min") ? result.Runtime.Replace(" min", "") : "0";

                Logger.Debug("runtime: " + runtime);

                result.Minutes = Convert.ToInt32(runtime);

                Logger.Debug("result.Minutes: " + result.Minutes);
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }

            return result;
        }
    }
}
