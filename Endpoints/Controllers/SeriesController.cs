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

        [Route("name")]
        [HttpPost]
        public List<Tools.Scaffold.Season> ByName(Tools.Scaffold.Form.Series series)
        {
            return Model.Logic.Series.ByName(series.Name);
        }

        [Route("season")]
        [HttpPost]
        public List<Tools.Scaffold.Episode> BySeason(Tools.Scaffold.Form.Series series)
        {
            return Model.Logic.Series.BySeason(series.Name, series.Season);
        }

        [Route("name/{name}/season/{season}")]
        [HttpGet]
        public List<Tools.Scaffold.Episode> BySeason(string name, int? season)
        {
            return Model.Logic.Series.BySeason(name, season);
        }

        [Route("episode")]
        [HttpPost]
        public List<Tools.Scaffold.Video> ByEpisode(Tools.Scaffold.Form.Series series)
        {
            return Model.Logic.Series.ByEpisode(series.Name, series.Season, series.Episode);
        }
    }
}
