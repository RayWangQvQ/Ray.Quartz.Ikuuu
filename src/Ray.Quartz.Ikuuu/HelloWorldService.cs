﻿using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Ray.Quartz.Ikuuu.Agents;
using Ray.Quartz.Ikuuu.Configs;
using Volo.Abp.DependencyInjection;

namespace Ray.Quartz.Ikuuu;

public class HelloWorldService : ITransientDependency
{
    private readonly IConfiguration _configuration;
    private readonly IIkuuuApi _hostlocApi;
    private readonly AccountOptions _accountConfig;

    public HelloWorldService(
        IConfiguration configuration,
        IIkuuuApi hostlocApi,
        IOptions<AccountOptions> accountOptions
        )
    {
        _configuration = configuration;
        _hostlocApi = hostlocApi;
        _accountConfig = accountOptions.Value;
        Logger = NullLogger<HelloWorldService>.Instance;
    }

    public ILogger<HelloWorldService> Logger { get; set; }


    public async Task SayHelloAsync(CancellationToken cancellationToken)
    {
        Logger.LogInformation("Hello World!{newLine}", Environment.NewLine);

        var taskName = _configuration["Run"];
        switch (taskName)
        {
            case "checkin":
                var re = await LoginAsync(cancellationToken);
                if (re)
                {
                    await CheckinAsync(cancellationToken);
                }
                break;
            default:
                Logger.LogWarning("任务不存在：{task}", taskName);
                break;
        }
    }

    /// <summary>
    /// 登录获取Cookie
    /// </summary>
    /// <returns></returns>
    public async Task<bool> LoginAsync(CancellationToken cancellationToken)
    {
        Logger.LogInformation("开始任务：登录");
        Logger.LogInformation(_accountConfig.Email);

        var req = new LoginRequest(_accountConfig.Email, _accountConfig.Pwd);
        var re = await _hostlocApi.LoginAsync(req);

        if (!re.IsSuccessStatusCode)
        {
            Logger.LogError(JsonSerializer.Serialize(re));
            return false;
        }

        Logger.LogDebug(JsonSerializer.Serialize(re.Content));

        if (re.Content?.ret != 1)
        {
            Logger.LogError("登录失败：{msg}", re.Content?.msg ?? "");
            return false;
        }
        Logger.LogInformation("{msg}", re.Content?.msg ?? "登录成功！");
        Logger.LogInformation("Success{newLine}", Environment.NewLine);

        return true;
    }

    /// <summary>
    /// 签到
    /// </summary>
    /// <returns></returns>
    public async Task CheckinAsync(CancellationToken cancellationToken)
    {
        Logger.LogInformation("开始签到");
        var re2 = await _hostlocApi.CheckinAsync();

        if (!re2.IsSuccessStatusCode)
        {
            Logger.LogError(JsonSerializer.Serialize(re2));
            return;
        }

        Logger.LogDebug(JsonSerializer.Serialize(re2.Content));

        if (re2.Content?.ret != 1)
        {
            Logger.LogError("签到失败：{msg}", re2.Content?.msg ?? "");
            return;
        }

        Logger.LogInformation("{msg}", re2.Content?.msg ?? "签到成功！");
        Logger.LogInformation("Success{newLine}", Environment.NewLine);
    }
}
