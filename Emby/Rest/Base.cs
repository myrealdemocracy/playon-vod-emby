using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Common.Net;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Serialization;

namespace PlayOn.Emby.Rest
{
    public class Base
    {
        protected static IHttpClient HttpClient = Channel.HttpClient;
        protected static ILogger Logger = Channel.Logger;
        protected static IJsonSerializer JsonSerializer = Channel.JsonSerializer;

        protected static Task<T> Request<T>(string route, string method, CancellationToken cancellationToken, string body = null) where T : new()
        {
            Logger.Debug("route: " + route);
            Logger.Debug("method: " + method);
            Logger.Debug("body: " + body);

            try
            {
                return Task.Run(() =>
                {
                    var options = new HttpRequestOptions
                    {
                        Url = "http://playon.local" + route,
                        EnableHttpCompression = true,
                        CacheMode = CacheMode.None,
                        CancellationToken = cancellationToken,
                        TimeoutMs = 1000 * 60 * 5
                    };

                    if(!String.IsNullOrEmpty(body)) options.RequestContent = body;

                    var response = HttpClient.SendAsync(options, method);

                    return JsonSerializer.DeserializeFromStream<T>(response.Result.Content);
                }, cancellationToken);
            }
            catch (Exception exception)
            {
                Logger.ErrorException("API request call failed", exception);
            }

            return Task.Run(() => new T(), cancellationToken);
        }
    }
}
