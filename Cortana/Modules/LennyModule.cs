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
        [Command("lenny")]
        [Summary("Append the lenny face to a message")]
        public async Task Lenny([Optional, Remainder] string input)
        {
            await Context.Message.DeleteAsync();
            await ReplyAsync($"{input} ( ͡° ͜ʖ ͡°)");
        }
    }
}