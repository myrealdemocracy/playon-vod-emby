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
        [Route("all/{start}/{end}")]
        [HttpGet]
        public Tools.Scaffold.Result.Series All(int start, int end)
        {
            return Model.Logic.Series.All(start, end);
        }

        [Route("seasons/{imdbId}")]
        [HttpGet]
        public List<Tools.Scaffold.Season> ByName(string imdbId)
        {
            return Model.Logic.Series.ByName(imdbId);
        }

        [Route("episodes/{imdbId}/s/{season}")]
        [HttpGet]
        public List<Tools.Scaffold.Episode> BySeason(string imdbId, int? season)
        {
            return Model.Logic.Series.BySeason(imdbId, season);
        }

        [Route("video/{imdbId}/s/{season}/e/{episode}")]
        [HttpGet]
        public HttpResponseMessage Video(string imdbId, int? season, int? episode)
        {
            var url = Model.Logic.Series.VideoByImdbIdSeasonEpisode(imdbId, season, episode);
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
