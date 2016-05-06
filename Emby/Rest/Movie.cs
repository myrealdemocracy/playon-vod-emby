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
        public async Task<Scaffold.Result.Movie> All(int? start, int? end, CancellationToken cancellationToken)
        {
            var movies = new Scaffold.Result.Movie();

            try
            {
                movies = await Request<Scaffold.Result.Movie>("/movie/all/" + Convert.ToInt32(start) + "/" + Convert.ToInt32(end), "GET", cancellationToken);
            }
            catch (Exception exception)
            {
                Logger.ErrorException("Loading movies error, API call failed", exception);
            }

            return movies;
        }
    }
}
