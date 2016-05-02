using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PlayOn.Emby.Rest
{
    public class Movie : Base
    {
        public async Task<List<Scaffold.Movie>> All(string letter, CancellationToken cancellationToken)
        {
            var movies = new List<Scaffold.Movie>();

            try
            {
                movies = await Request<List<Scaffold.Movie>>("/movie/all/letter/" + letter, "GET", cancellationToken);
            }
            catch (Exception exception)
            {
                Logger.ErrorException("Loading movies error, API call failed", exception);
            }

            return movies;
        }
    }
}
