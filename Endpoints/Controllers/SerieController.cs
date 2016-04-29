using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PlayOn.Endpoints.Controllers
{
    [RoutePrefix("serie")]
    public class SerieController : ApiController
    {
        [Route("all")]
        [HttpGet]
        public List<Tools.Scaffold.Serie> All()
        {
            return Model.Logic.Serie.All();
        }

        [Route("category/{category}")]
        [HttpGet]
        public List<Tools.Scaffold.Serie> ByCategory(string category)
        {
            return Model.Logic.Serie.ByCategory(category);
        }

        [Route("{name}/s/{season:int}")]
        [HttpGet]
        public List<Tools.Scaffold.Season> BySeason(string name, int season)
        {
            return Model.Logic.Serie.BySeason(name, season);
        }

        [Route("{name}/s/{season:int}/e/{episode:int}")]
        [HttpGet]
        public List<Tools.Scaffold.Video> ByEpisode(string name, int season, int episode)
        {
            return Model.Logic.Serie.ByEpisode(name, season, episode);
        }
    }
}
