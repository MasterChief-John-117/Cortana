using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Cortana.Modules
{
    public class SelfModule : ModuleBase
    {
        [Command("game")]
        public async Task SetGame([Optional, Remainder] string gameToSet)
        {
            await Context.Message.DeleteAsync();
            if (!string.IsNullOrEmpty(gameToSet))
            {
                await (Context.Client as DiscordSocketClient).SetGameAsync(gameToSet);
                await ReplyAsync($"Game has been set to **{gameToSet}**");
            }
            else
            {
                await (Context.Client as DiscordSocketClient).SetGameAsync("");
                await ReplyAsync("Game has been cleared");
            }
    }
    }
}