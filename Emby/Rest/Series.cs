using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PlayOn.Emby.Rest
{
    public class Series : Base
    {
        public async Task<List<Scaffold.Series>> All(CancellationToken cancellationToken)
        {
            var series = new List<Scaffold.Series>();

            try
            {
                series = await Request<List<Scaffold.Series>>("/series/all", "GET", cancellationToken);
            }
            catch (Exception exception)
            {
                Logger.ErrorException("Loading series error, API call failed", exception);
            }

            return series;
        }

        public async Task<List<Scaffold.Season>> Seasons(string name, CancellationToken cancellationToken)
        {
            var seasons = new List<Scaffold.Season>();

            try
            {
                seasons = await Request<List<Scaffold.Season>>("/series/name", "POST", cancellationToken, "Name=" + name);
            }
            catch (Exception exception)
            {
                Logger.ErrorException("Loading seasons error, API call failed", exception);
            }

            return seasons;
        }

        public async Task<List<Scaffold.Episode>> Episodes(string name, int season, CancellationToken cancellationToken)
        {
            var episodes = new List<Scaffold.Episode>();

            try
            {
                episodes = await Request<List<Scaffold.Episode>>("/series/season", "POST", cancellationToken, "Name=" + name + "&Season=" + season);
            }
            catch (Exception exception)
            {
                Logger.ErrorException("Loading episodes error, API call failed", exception);
            }

            return episodes;
        }
    }
}
