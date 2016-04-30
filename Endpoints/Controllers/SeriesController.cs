using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PlayOn.Endpoints.Controllers
{
    [RoutePrefix("series")]
    public class SeriesController : ApiController
    {
        [Route("all")]
        [HttpGet]
        public List<Tools.Scaffold.Series> All()
        {
            return Model.Logic.Series.All();
        }

        [Route("category/{category}")]
        [HttpGet]
        public List<Tools.Scaffold.Series> ByCategory(string category)
        {
            return Model.Logic.Series.ByCategory(category);
        }

        [Route("name/{name}")]
        [HttpGet]
        public List<Tools.Scaffold.Season> ByName(string name)
        {
            return Model.Logic.Series.ByName(name);
        }

        [Route("name/{name}/s/{season:int}")]
        [HttpGet]
        public List<Tools.Scaffold.Episode> BySeason(string name, int season)
        {
            return Model.Logic.Series.BySeason(name, season);
        }

        [Route("name/{name}/s/{season:int}/e/{episode:int}")]
        [HttpGet]
        public List<Tools.Scaffold.Video> ByEpisode(string name, int season, int episode)
        {
            return Model.Logic.Series.ByEpisode(name, season, episode);
        }
    }
}
