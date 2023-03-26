using System;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Ray.Quartz.Ikuuu.Agents
{
    public class CookieHttpClientHandler : HttpClientHandler
    {
        private readonly ILogger<CookieHttpClientHandler> _logger;
        private readonly CookieManager _ckManager;

        public CookieHttpClientHandler(ILogger<CookieHttpClientHandler> logger, CookieManager ckManager)
        {
            _logger = logger;
            _ckManager = ckManager;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("CkContainer before：{ck}", this.CookieContainer.GetAllCookies().ToJsonStr());
            _logger.LogDebug("CkStr:{ckStr}", _ckManager.GetCurrentCookieStr());

            var uri = new Uri("https://ikuuu.eu");
            Type type = this.CookieContainer.GetType();
            BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;
            var m = type.GetMethod("DomainTableCleanup", flags);
            m.Invoke(this.CookieContainer, null);
            this.CookieContainer.SetCookies(uri, _ckManager.GetCurrentCookieStr());//todo
            //this.UseCookies = true;
            _logger.LogDebug("CkContainer before：{ck}", this.CookieContainer.GetAllCookies().ToJsonStr());

            var re = await base.SendAsync(request, cancellationToken);

            CookieCollection cookies = this.CookieContainer.GetAllCookies();
            _logger.LogDebug("CkContainer after：{cookie}", cookies.ToJsonStr());
            _ckManager.Add(_ckManager.Index, this.CookieContainer.GetCookieHeader(uri));
            _logger.LogDebug(_ckManager.ToJsonStr());

            return re;
        }
    }
}
