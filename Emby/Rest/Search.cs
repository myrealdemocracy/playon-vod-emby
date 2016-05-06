using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PlayOn.Emby.Rest
{
    public class Search : Base
    {
        public async Task<List<Scaffold.Video>> Name(string name, CancellationToken cancellationToken)
        {
            var videos = new List<Scaffold.Video>();

            try
            {
                videos = await Request<List<Scaffold.Video>>("/search/name", "POST", cancellationToken, "Name=" + name);
            }
            catch (Exception exception)
            {
                Logger.ErrorException("Loading search videos error, API call failed", exception);
            }

            return videos;
        }
    }
}
