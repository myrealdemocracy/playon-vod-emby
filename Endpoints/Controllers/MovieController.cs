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

        [Route("category/{category}")]
        [HttpGet]
        public List<Tools.Scaffold.Movie> ByCategory(string category)
        {
            return Model.Logic.Movie.ByCategory(category);
        }

        [Route("name")]
        [HttpPost]
        public List<Tools.Scaffold.Video> ByName(Tools.Scaffold.Form.Movie movie)
        {
            return Model.Logic.Movie.ByName(movie.Name);
        }
    }
}
