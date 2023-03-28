using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp;

namespace Ray.Quartz.Ikuuu.Agents
{
    public class CookieHttpClientHandler : HttpClientHandler
    {
        private readonly ILogger<CookieHttpClientHandler> _logger;
        private readonly CookieManager _ckManager;
        private readonly CookieInfo _ckInfo;

        public CookieHttpClientHandler(
            ILogger<CookieHttpClientHandler> logger, 
            CookieManager ckManager, 
            CookieInfo ckInfo
            )
        {
            _logger = logger;
            _ckManager = ckManager;
            _ckInfo = ckInfo;

            this.UseCookies = false;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var uri = new Uri("https://ikuuu.eu");

            //_logger.LogDebug("CkContainer before：{ck}", this.CookieContainer.GetAllCookies().ToJsonStr());
            //_logger.LogDebug("CkStr:{ckStr}", _ckManager.GetCurrentCookieStr());

            //var uri = new Uri("https://ikuuu.eu");
            //Type type = this.CookieContainer.GetType();
            //BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;
            //var m = type.GetMethod("DomainTableCleanup", flags);
            //m.Invoke(this.CookieContainer, null);
            //this.CookieContainer.SetCookies(uri, _ckManager.GetCurrentCookieStr());//todo
            //_logger.LogDebug("CkContainer before：{ck}", this.CookieContainer.GetAllCookies().ToJsonStr());
            var ckHeaderStr = _ckInfo.ToString();
            _logger.LogDebug("Cookie:{cookie}",ckHeaderStr);
            request.Headers.Add("Cookie", ckHeaderStr);

            var re = await base.SendAsync(request, cancellationToken);

            //CookieCollection cookies = this.CookieContainer.GetAllCookies();
            //_logger.LogDebug("CkContainer after：{cookie}", cookies.ToJsonStr());
            //_ckManager.Add(_ckManager.Index, this.CookieContainer.GetCookieHeader(uri));
            //_logger.LogDebug(_ckManager.ToJsonStr());

            //add or update cookie from set-header
            var cookies = re.Headers.FirstOrDefault(header => header.Key == "Set-Cookie").Value?.ToList()??new List<string>();
            _ckInfo.MergeCurrentCookieBySetCookieHeaders(cookies);

            return re;
        }
    }
}
