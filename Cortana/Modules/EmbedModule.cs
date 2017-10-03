using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Cortana.Utilities;
using Discord;
using Discord.Commands;
using Color = Discord.Color;

namespace Cortana.Modules
{
    public class EmbedModule : ModuleBase
    {
        [Command("embedText")]
        [Alias("em")]
        [Summary("embed some text")]
        public async Task embedText([Remainder] string input)
        {
            await Context.Message.DeleteAsync();
            string title = Regex.Match(input, @"\[([^)]*)\]").Groups[1].Value;
            string text = Regex.Match(input, @"\(([^)]*)\)").Groups[1].Value;
            if (!input.Contains("(")) text = input;

            Random random = new Random();
            var em = new EmbedBuilder();
            em.WithDescription(text);
            em.WithTitle(title);
            em.WithColor(new Color(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255)));

            await ReplyAsync("", embed: em.Build());
        }

        [Command("embedImage")]
        [Alias("img")]
        [Summary("embed an image")]
        public async Task EmbedImage([Remainder] string input)
        {
            var watch = new Stopwatch();
            watch.Restart();
            await Context.Message.DeleteAsync();
            string title = Regex.Match(input, @"\[([^)]*)\]").Groups[1].Value;
            string link = Regex.Match(input, @"\(([^)]*)\)").Groups[1].Value;
            if (!input.Contains("(")) link = input;

            var cmd = new CommandHandler();

            new WebClient().DownloadFile(new Uri(link), "files/tempImg.png");
            using (Bitmap bitmap = new Bitmap("files/tempImg.png"))
            {
                var clr = new ImageUtils().GetAverageColor(bitmap, cmd, 10);

                var em = new EmbedBuilder()
                    .WithTitle(title)
                    .WithImageUrl(link)
                    .WithColor(new Discord.Color(clr.R, clr.G, clr.B))
                    .WithFooter(new EmbedFooterBuilder().WithText(
                        $"{cmd.Iterator} cycles | {watch.ElapsedMilliseconds}ms " +
                        $"| {new FileInfo(("files/tempImg.png")).Length / 1024}kb"))
                    .Build();
                await ReplyAsync("", embed: em);
            }
        }
    }
}
