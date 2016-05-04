using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PlayOn.Endpoints.Controllers
{
    [RoutePrefix("url")]
    public class UrlController : ApiController
    {
        [Route("image")]
        [HttpGet]
        public HttpResponseMessage UrlImage([FromUri] string path)
        {
            return new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Found,
                Headers =
                {
                    Location = new Uri(Tools.Helper.Url.Generate(WebUtility.UrlDecode(path) + "image|"))
                }
            };
        }

        [Route("video")]
        [HttpGet]
        public HttpResponseMessage UrlVideo([FromUri] string path)
        {
            return new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Found,
                Headers =
                {
                    Location = new Uri(Tools.Helper.Url.Generate(WebUtility.UrlDecode(path) + "video|"))
                }
            };
        }
    }
}
