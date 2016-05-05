using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PlayOn.Emby.Rest
{
    public class Video : Base
    {
        public async Task<Scaffold.Video> Total(CancellationToken cancellationToken)
        {
            var video = new Scaffold.Video();

            try
            {
                video = await Request<Scaffold.Video>("/video/total", "GET", cancellationToken);
            }
            catch (Exception exception)
            {
                Logger.ErrorException("Loading movies error, API call failed", exception);
            }

            return video;
        }
    }
}
