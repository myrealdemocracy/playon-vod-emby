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
        public async Task<Scaffold.Category> All(CancellationToken cancellationToken)
        {
            var categories = new Scaffold.Category();

            try
            {
                if (Channel.Categories != null) return Channel.Categories;

                Channel.Categories = await Request<Scaffold.Category>("/category/all", cancellationToken);
            }
            catch (Exception exception)
            {
                Logger.ErrorException("Loading categories error, API call failed", exception);
            }

            return categories;
        }
    }
}
