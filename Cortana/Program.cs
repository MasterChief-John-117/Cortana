using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace Cortana
{
    public class Program
    {
        public static DiscordSocketClient Client;
        private Configuration _config;

        public static void Main(string[] args) =>
             new Program().Start().GetAwaiter().GetResult();
        


        public async Task Start()
        {
            //Make sure we have valid files
            if (!Directory.Exists("files")) Directory.CreateDirectory("files");
            if (!File.Exists("files/config.json"))
            {
                _config = new Configuration(new ConfigurationBuilder().createConfiguration());
                File.WriteAllText("files/config.json", JsonConvert.SerializeObject(_config, Formatting.Indented));
            }

            else _config = new Configuration();
            Client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose, //for most debug, Verbose. For normal use, Crit is fine
                AlwaysDownloadUsers = true
            });
            Client.Log += Log;

            try
            {
                await Client.LoginAsync(_config.TokenType, _config.Token);
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
                .AddSingleton(new CommandService(new CommandServiceConfig { CaseSensitiveCommands = false, ThrowOnError = false}));
            var provider = new DefaultServiceProviderFactory().CreateServiceProvider(services);
            return provider;
        }

        private async Task _onReady()
        {
            Console.WriteLine("Almost ready to go! Your prefix is \"{0}\"", _config.Prefix);
            Console.WriteLine($"Connexting to {Client.Guilds.Count} guilds");

        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(DateTime.Now + ": " + msg.ToString());
            return Task.FromResult(0);
        }
    }
}