using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PlayOn.Emby.Rest
{
    public class Series : Base
    {
        public async Task<Scaffold.Result.Series> All(int? start, int? end, CancellationToken cancellationToken)
        {
            var series = new Scaffold.Result.Series();

            try
            {
                series = await Request<Scaffold.Result.Series>("/series/all/" + Convert.ToInt32(start) + "/" + Convert.ToInt32(end), "GET", cancellationToken);
            }
            catch (Exception exception)
            {
                Logger.ErrorException("Loading series error, API call failed", exception);
            }

            return series;
        }

        public async Task<List<Scaffold.Season>> Seasons(string imdbId, CancellationToken cancellationToken)
        {
            var seasons = new List<Scaffold.Season>();

            try
            {
                seasons = await Request<List<Scaffold.Season>>("/series/seasons/" + imdbId, "GET", cancellationToken);
            }
            catch (Exception exception)
            {
                Logger.ErrorException("Loading seasons error, API call failed", exception);
            }

            return seasons;
        }

        public async Task<List<Scaffold.Episode>> Episodes(string imdbId, int season, CancellationToken cancellationToken)
        {
            var episodes = new List<Scaffold.Episode>();

            try
            {
                episodes = await Request<List<Scaffold.Episode>>("/series/episodes/" + imdbId + "/s/" + season, "GET", cancellationToken);
            }
            catch (Exception exception)
            {
                Logger.ErrorException("Loading episodes error, API call failed", exception);
            }

            return episodes;
        }
    }
}
