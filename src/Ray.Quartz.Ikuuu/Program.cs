using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ray.Serilog.Sinks.PushPlusBatched;
using Ray.Serilog.Sinks.ServerChanBatched;
using Ray.Serilog.Sinks.TelegramBatched;
using Ray.Serilog.Sinks.WorkWeiXinBatched;
using Serilog;
using Serilog.Events;

namespace Ray.Quartz.Ikuuu;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        var hb = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostBuilderContext, configurationBuilder) =>
            {
                IList<IConfigurationSource> list = configurationBuilder.Sources;
                list.ReplaceWhile(
                    configurationSource => configurationSource is EnvironmentVariablesConfigurationSource,
                    new EnvironmentVariablesConfigurationSource()
                    {
                        Prefix = "Ray_Ikuuu_"
                    }
                );
            });
        var tempHost = hb.Build();
        var config = tempHost.Services.GetRequiredService<IConfiguration>();
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
#if DEBUG
            .MinimumLevel.Debug()
#endif
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Async(c =>
            {
                c.File($"Logs/{DateTime.Now.ToString("yyyy-MM-dd")}/{DateTime.Now.ToString("HH-mm-ss")}.txt",
                    restrictedToMinimumLevel: LogEventLevel.Debug);
            })
            .WriteTo.Console()
            .WriteTo.PushPlusBatched(
                config["Notify:PushPlus:Token"],
                config["Notify:PushPlus:Channel"],
                config["Notify:PushPlus:Topic"],
                config["Notify:PushPlus:Webhook"],
                restrictedToMinimumLevel: LogEventLevel.Information
            )
            .WriteTo.TelegramBatched(
                config["Notify:Telegram:BotToken"],
                config["Notify:Telegram:ChatId"],
                config["Notify:Telegram:Proxy"],
                restrictedToMinimumLevel: LogEventLevel.Information
            )
            .WriteTo.ServerChanBatched(
                "",
                turboScKey: config["Notify:ServerChan:TurboScKey"],
                restrictedToMinimumLevel: LogEventLevel.Information
            )
            .WriteTo.WorkWeiXinBatched(
                config["Notify:WorkWeiXin:WebHookUrl"],
                restrictedToMinimumLevel: LogEventLevel.Information
            )
            .CreateLogger();
        try
        {
            Console.WriteLine("Starting console host.");

            await Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostBuilderContext, configurationBuilder) =>
                {
                    IList<IConfigurationSource> list = configurationBuilder.Sources;
                    list.ReplaceWhile(
                        configurationSource => configurationSource is EnvironmentVariablesConfigurationSource,
                        new EnvironmentVariablesConfigurationSource()
                        {
                            Prefix = "Ray_Ikuuu_"
                        }
                        );
                })
                .ConfigureServices(services =>
                {
                    services.AddHostedService<HostlocHostedService>();
                })
                .UseSerilog()
                .RunConsoleAsync();

            return 0;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly!");
            Log.Logger.Fatal("·开始推送·{task}·{user}", "任务异常", "");
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
