using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PlayOn.Endpoints.Controllers
{
    [RoutePrefix("search")]
    public class SearchController : ApiController
    {
        [Route("name")]
        [HttpPost]
        public List<Tools.Scaffold.Video> ByName(Tools.Scaffold.Form.Search search)
        {
            return Model.Logic.Search.ByName(search);
        }
    }
}
