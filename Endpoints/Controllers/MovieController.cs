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
        public Tools.Scaffold.MovieList All(int start, int end)
        {
            return Model.Logic.Movie.All(start, end);
        }

        [Route("video")]
        [HttpGet]
        public HttpResponseMessage Video([FromUri] string name)
        {
            var url = Model.Logic.Movie.VideoByName(WebUtility.UrlDecode(name));
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
