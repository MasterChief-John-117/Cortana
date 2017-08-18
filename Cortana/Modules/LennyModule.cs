using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Newtonsoft.Json;

namespace Cortana.Modules
{
    public class LennyModule : ModuleBase
    {
        [Command("randomLenny")]
        [Summary("Returns a random lenny face")]
        public async Task RandomLenny()
        {
            await Context.Message.DeleteAsync();
            var client = new WebClient {Encoding = Encoding.UTF8};
            string response = client.DownloadString(new Uri("http://lenny.today/api/v1/random")).Trim('[', ']');
            await ReplyAsync(JsonConvert.DeserializeObject<Dictionary<string, string>>(response)["face"]);
        }

        [Command("lenny")]
        [Summary("Append the lenny face to a message")]
        public async Task Lenny([Optional, Remainder] string input)
        {
            await Context.Message.DeleteAsync();
            await ReplyAsync($"{input} ( ͡° ͜ʖ ͡°)");
        }
    }
}