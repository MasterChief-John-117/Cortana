using System.Net;
using System.Threading.Tasks;
using Discord.Commands;

namespace Cortana.Modules
{
    public class WebModule : ModuleBase
    {
        [Command("curl")]
        [Summary("Get a string from a URL")]
        public async Task Curl(string url)
        {
            using (var client = new WebClient())
            {
                await ReplyAsync($"{client.DownloadString(url)}");
            }
        }
    }
}