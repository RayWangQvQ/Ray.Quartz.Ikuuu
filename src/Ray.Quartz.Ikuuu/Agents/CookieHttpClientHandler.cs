using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

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

            this.UseCookies = true;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            this.CookieContainer.GetAllCookies().ToList().ForEach(x=>x.Expired=true);
            if (this.CookieContainer.Count>0)
            {
                var m = this.CookieContainer.GetType().GetMethod("AgeCookies", BindingFlags.NonPublic | BindingFlags.Instance);
                m.Invoke(this.CookieContainer, new object[]{null});
            }

            this.CookieContainer.Add(_ckManager.CurrentAccount.MyCookieContainer.GetAllCookies());

            HttpResponseMessage re = await base.SendAsync(request, cancellationToken);

            CookieContainer cc = new CookieContainer();
            cc.Add(this.CookieContainer.GetAllCookies());
            _ckManager.CurrentAccount.MyCookieContainer = cc;

            return re;
        }
    }
}
