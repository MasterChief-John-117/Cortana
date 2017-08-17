using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Linq;
using System.Net;
using System;

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
                await (Context.Client as DiscordSocketClient).SetGameAsync(null);
                await ReplyAsync("Game has been cleared");
            }
        }

        [Command("streaming")]
        public async Task SetStreaming([Remainder] string streamInput)
        {
            var link = (streamInput.Split(' ').ToList().First(s => System.Uri.IsWellFormedUriString(s, System.UriKind.RelativeOrAbsolute)));
            var game = streamInput.Split(' ').ToList().SkipWhile(s => s.Equals(link)).Aggregate((a, b) => a + " " + b);
            await (Context.Client as DiscordSocketClient).SetGameAsync(game, link, StreamType.Twitch);

            await ReplyAsync($"Streaming {game} at <{link}>! *(do {new Configuration().Prefix}game to stop streaming)*");
        }

        [Command("avatar")]
        public async Task User_Avatar([Remainder] string url)
        {
            await Context.Message.DeleteAsync();
            using (var client = new WebClient())
            {
                client.DownloadFile(new Uri(url), "files/tempAvatar.png");

                await Task.Delay(1000);
                await Context.Client.CurrentUser.ModifyAsync(u => u.Avatar = new Image("files/tempAvatar.png"));
            }
        }
    }
}