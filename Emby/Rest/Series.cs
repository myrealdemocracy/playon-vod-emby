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
                series = await Request<List<Scaffold.Series>>("/series/all", cancellationToken);
            }
            catch (Exception exception)
            {
                Logger.ErrorException("Loading series error, API call failed", exception);
            }

            return series;
        }
    }
}
