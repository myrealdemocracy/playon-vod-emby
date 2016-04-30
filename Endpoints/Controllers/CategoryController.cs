using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PlayOn.Endpoints.Controllers
{
    [RoutePrefix("category")]
    public class CategoryController : ApiController
    {
        [Route("all")]
        [HttpGet]
        public Tools.Scaffold.Category All()
        {
            return new Tools.Scaffold.Category
            {
                Categories = Tools.Constant.Category.Items.Select(s => s.Key).ToList()
            };
        }
    }
}
