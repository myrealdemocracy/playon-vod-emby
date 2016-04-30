using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PlayOn.Emby.Rest
{
    public class Category : Base
    {
        public async Task<List<string>> All(CancellationToken cancellationToken)
        {
            var categories = new List<string>();

            try
            {
                var result = await Request<Scaffold.Category>("/category/all", "GET", cancellationToken);

                categories = result.Categories;
            }
            catch (Exception exception)
            {
                Logger.ErrorException("Loading categories error, API call failed", exception);
            }

            return categories;
        }
    }
}
