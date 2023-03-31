using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Ray.Infrastructure.AutoTask;

namespace Ray.Quartz.Ikuuu.Agents
{
    public class CookieHttpClientHandler : HttpClientHandler
    {
        private readonly ILogger<CookieHttpClientHandler> _logger;
        private readonly TargetAccountManager<TargetAccountInfo> _ckManager;

        public CookieHttpClientHandler(
            ILogger<CookieHttpClientHandler> logger, 
            TargetAccountManager<TargetAccountInfo> ckManager
            )
        {
            _logger = logger;
            _ckManager = ckManager;

            this.UseCookies = true;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            _ckManager.ReplaceCookieContainerWithCurrentAccount(this.CookieContainer);

            HttpResponseMessage re = await base.SendAsync(request, cancellationToken);

            _ckManager.UpdateCurrentCookieContainer(this.CookieContainer);

            return re;
        }
    }
}
