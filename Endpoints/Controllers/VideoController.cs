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
        [Route("total")]
        [HttpGet]
        public Tools.Scaffold.Video Total()
        {
            return Model.Logic.Video.Total;
        }

        [Route("all")]
        [HttpGet]
        public List<Tools.Scaffold.Video> All()
        {
            return Model.Logic.Video.All;
        }
    }
}
