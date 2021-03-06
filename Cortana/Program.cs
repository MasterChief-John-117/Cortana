﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Timers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.Webhook;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Timer = System.Timers.Timer;
using xkcd;

namespace Cortana
{
    public class Program
    {
        public static DiscordSocketClient Client;
        public static DiscordWebhookClient WHClient;
        private Configuration _config;
        private int _loadedGuilds = 0;
        private int _totalGuilds;
        private  Stopwatch _stopwatch = new Stopwatch();
        private WebClient sWClient = new WebClient();

        private string currentVer = "0.1.155";
        private string newestVer;
        private string upToDate;

        private static Timer getMemoryAndLog;
        private static Timer cleanMemory;
        public static void Main(string[] args)
        {
            cleanMemory = new Timer();
            cleanMemory.AutoReset = true;
            cleanMemory.Interval = 1000 * 60;
            cleanMemory.Elapsed += CleanMemory;
            cleanMemory.Start();

            if (!File.Exists("files/xkcd.json"))
            {
                Directory.CreateDirectory("files");
                File.WriteAllText("files/xkcd.json", JsonConvert.SerializeObject(new xkcdComicStore().create(), Formatting.Indented));
            }
            else
            {
                File.WriteAllText("files/xkcd.json", JsonConvert.SerializeObject(new xkcdComicStore().update(), Formatting.Indented));
            }

            new Program().Start().GetAwaiter().GetResult();
        }

        public async Task Start()
        {
            try
            {
                newestVer = sWClient.DownloadString("http://api.mcjohn117.duckdns.org/cortana/latest").Trim();
            }
            catch (Exception e){}
            upToDate = currentVer == newestVer ? $"Cortana is up to date! {currentVer}" : $"Cortana is out of date ({currentVer} < {newestVer})";
            //Make sure we have valid files
            if (!Directory.Exists("files")) Directory.CreateDirectory("files");

            if (!File.Exists("files/config.json"))
            {
                _config = new Configuration(new ConfigurationBuilder().createConfiguration());
                File.WriteAllText("files/config.json", JsonConvert.SerializeObject(_config, Formatting.Indented));
            }
            else
            {
                _config = new Configuration();
                File.WriteAllText("files/config.json", JsonConvert.SerializeObject(_config, Formatting.Indented));
            }

            if (!File.Exists("files/customCommands.json"))
            {
                var tempCommands = new List<CustomCommand>();
                tempCommands.Add(new CustomCommand("cortana", "This is the Cortana self-bot!"));
                File.WriteAllText("files/customCommands.json", JsonConvert.SerializeObject(tempCommands, Formatting.Indented));
            }

            if (!string.IsNullOrEmpty(_config.LoggingWebhookUrl))
            {
                WHClient = new DiscordWebhookClient(Convert.ToUInt64(_config.LoggingWebhookUrl.Substring(36, 18)), _config.LoggingWebhookUrl.Substring(55));
            }


            Client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose, //for most debug, Verbose. For normal use, Crit is fine
                AlwaysDownloadUsers = false,
                MessageCacheSize = 100
            });
            Client.Log += Log;

            try
            {
                await Client.LoginAsync(_config.TokenType, _config.Token);
                _stopwatch.Start();
                await Client.StartAsync();
            }
            catch (HttpException httpException)
            {
                if (httpException.HttpCode == HttpStatusCode.Unauthorized)
                {
                    _config.UpdateToken();
                    Start().GetAwaiter().GetResult();
                    return;
                }
            }

            Client.Ready += _onReady;

            var serviceProvider = ConfigureServices();

            var _handler = new CommandHandler();
            await _handler.Install(serviceProvider);

            // Block this program until it is closed.
            await Task.Delay(-1);
        }

        private IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection()
                .AddSingleton(Client)
                .AddSingleton(new CommandService(new CommandServiceConfig
                {
                    CaseSensitiveCommands = false,
                    ThrowOnError = false,
                    DefaultRunMode = RunMode.Async
                }));
            var provider = new DefaultServiceProviderFactory().CreateServiceProvider(services);
            return provider;
        }

        private async Task _onReady()
        {
            _totalGuilds = Client.Guilds.Count;
            Console.WriteLine($"Loading {_totalGuilds} guilds");
        }

        private Task Log(LogMessage msg)
        {
            if (msg.ToString().Contains("Connected to") && !_config.DebugLogMode)
            {
                _loadedGuilds++;
                var sb = new StringBuilder();
                sb.AppendLine(upToDate);
                sb.AppendLine($"You prefix is \"{_config.Prefix}\"");
                sb.AppendLine("Guild loading progress: ");
                sb.Append("[");
                sb.Append('|', (50 *_loadedGuilds) / _totalGuilds);
                sb.Append(' ', 50 - (50 * _loadedGuilds ) / _totalGuilds);
                sb.Append(']');
                sb.Append($" ({_loadedGuilds}/{_totalGuilds})");
                sb.Append($" [{_stopwatch.ElapsedMilliseconds/1000}s]");
                sb.Append($" {{{Client.Guilds.Sum(x => x.Users.Count())} users}}");
                Console.Clear();
                Console.WriteLine(sb.ToString());
            }
            Console.WriteLine(msg);
            return Task.FromResult(0);
        }
        private static async void CleanMemory(object source, ElapsedEventArgs e)
        {
            GC.Collect();
            if (Math.Round(GC.GetTotalMemory(true) / (1024.0 * 1024.0), 2) > 1024)
            {
                await Client.LogoutAsync();
                Client.Dispose();
                Main(null);
            }
        }
    }
}
