using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ray.Quartz.Ikuuu.Configs;
using Serilog;
using Volo.Abp;

namespace Ray.Quartz.Ikuuu;

public class HostlocHostedService : IHostedService
{
    private IAbpApplicationWithInternalServiceProvider _abpApplication;

    private readonly IConfiguration _configuration;
    private readonly IHostEnvironment _hostEnvironment;
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly ILogger<HostlocHostedService> _logger;

    public HostlocHostedService(
        IConfiguration configuration,
        IHostEnvironment hostEnvironment,
        IHostApplicationLifetime hostApplicationLifetime,
        ILogger<HostlocHostedService> logger
    )
    {
        _configuration = configuration;
        _hostEnvironment = hostEnvironment;
        _hostApplicationLifetime = hostApplicationLifetime;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _abpApplication = await AbpApplicationFactory.CreateAsync<HostlocModule>(options =>
        {
            options.Services.ReplaceConfiguration(_configuration);
            options.Services.AddSingleton(_hostEnvironment);

            options.UseAutofac();
            options.Services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog());
        });

        await _abpApplication.InitializeAsync();

        var accountOptionsList = _abpApplication.ServiceProvider.GetRequiredService<IOptions<List<AccountOptions>>>().Value;
        var ckManager = _abpApplication.ServiceProvider.GetRequiredService<TargetAccountManager<TargetAccountInfo>>();
        ckManager.Init(accountOptionsList.Select(x => new TargetAccountInfo(x.Email, x.Pwd)).ToList());

        if (ckManager.Count <= 0)
        {
            _logger.LogWarning("一个账号没配你运行个卵");
            return;
        }

        for (int i = 0; i < accountOptionsList.Count; i++)
        {
            ckManager.Index = i;
            var currentAccount = ckManager.CurrentTargetAccount;
            _logger.LogInformation("========账号{count}========", i + 1);
            _logger.LogInformation("用户名：{userName}", currentAccount.UserName);

            using var scope = _abpApplication.ServiceProvider.CreateScope();
            var helloWorldService = scope.ServiceProvider.GetRequiredService<HelloWorldService>();
            await helloWorldService.SayHelloAsync(cancellationToken);

            _logger.LogInformation("========账号{count}结束========{newLine}", i + 1, Environment.NewLine);

            _logger.LogInformation("·开始推送·{task}·{user}", $"{_configuration["Run"]}任务", currentAccount.UserName);
        }
        _hostApplicationLifetime.StopApplication();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _abpApplication.ShutdownAsync();
    }
}
