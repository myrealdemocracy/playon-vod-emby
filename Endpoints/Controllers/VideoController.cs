using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PlayOn.Endpoints.Controllers
{
    [RoutePrefix("video")]
    public class VideoController : ApiController
    {
        [Route("all")]
        [HttpGet]
        public List<Tools.Scaffold.Video> All()
        {
            return Tools.Helper.Video.All("nativefox|");
        }

        [Route("movies")]
        [HttpGet]
        public List<Tools.Scaffold.Movie> Movies()
        {
            return Model.Logic.Movie.All();
        }

        [Route("series")]
        [HttpGet]
        public List<Tools.Scaffold.Serie> Series()
        {
            return Model.Logic.Serie.All();
        }
    }
}
