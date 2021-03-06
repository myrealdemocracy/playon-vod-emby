﻿using System;
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

        [Route("{imdbId}/seasons")]
        [HttpGet]
        public List<Tools.Scaffold.Season> ByName(string imdbId)
        {
            return Model.Logic.Series.ByName(imdbId);
        }

        [Route("{imdbId}/episodes/{season}")]
        [HttpGet]
        public List<Tools.Scaffold.Episode> BySeason(string imdbId, int? season)
        {
            return Model.Logic.Series.BySeason(imdbId, season);
        }

        [Route("{imdbId}/video/{season}/{episode}")]
        [HttpGet]
        public List<Tools.Scaffold.Video> Video(string imdbId, int? season, int? episode)
        {
            return new List<Tools.Scaffold.Video>
            {
                new Tools.Scaffold.Video
                {
                    Path = Model.Logic.Series.VideoByImdbIdSeasonEpisode(imdbId, season, episode)
                }
            };
        }
    }
}
