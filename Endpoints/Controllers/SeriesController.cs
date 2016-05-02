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

        [Route("video/s/{season}/e/{episode}")]
        [HttpGet]
        public HttpResponseMessage Video([FromUri] string name, int? season, int? episode)
        {
            return new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Found,
                Headers =
                {
                    Location = new Uri(Model.Logic.Series.VideoByNameSeasonEpisode(WebUtility.UrlDecode(name), season, episode))
                }
            };
        }
    }
}
