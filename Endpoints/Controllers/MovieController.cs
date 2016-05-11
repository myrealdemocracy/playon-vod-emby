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
        public HttpResponseMessage Video(string imdbId)
        {
            var url = Model.Logic.Movie.VideoByImdbId(imdbId);
            var message = new HttpResponseMessage();

            if (String.IsNullOrEmpty(url))
            {
                message.StatusCode = HttpStatusCode.Gone;
            }
            else
            {
                message.StatusCode = HttpStatusCode.Found;
                message.Headers.Location = new Uri(url);
            }

            return message;
        }
    }
}
