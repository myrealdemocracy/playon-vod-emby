using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PlayOn.Endpoints.Controllers
{
    [RoutePrefix("movie")]
    public class MovieController : ApiController
    {
        [Route("all/{start}/{end}")]
        [HttpGet]
        public Tools.Scaffold.Result.Movie All(int start, int end)
        {
            return Model.Logic.Movie.All(start, end);
        }

        [Route("{imdbId}/video")]
        [HttpGet]
        public List<Tools.Scaffold.Video> Video(string imdbId)
        {
            return new List<Tools.Scaffold.Video>
            {
                new Tools.Scaffold.Video
                {
                    Path = Model.Logic.Movie.VideoByImdbId(imdbId)
                }
            };
        }
    }
}
