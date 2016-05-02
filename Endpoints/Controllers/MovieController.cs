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
        [Route("all")]
        [HttpGet]
        public List<Tools.Scaffold.Movie> All()
        {
            return Model.Logic.Movie.All();
        }

        [Route("all/letter/{letter}")]
        [HttpGet]
        public List<Tools.Scaffold.Movie> ByLetter(string letter)
        {
            return Model.Logic.Movie.All(letter);
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
