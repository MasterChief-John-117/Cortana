using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Runtime.Remoting;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Cortana.Utilities;
using Discord;
using Discord.Commands;
using Newtonsoft.Json;

namespace Cortana.Modules
{
    public class EmoteModule : ModuleBase
    {
        [Command("postEmote")]
        [Alias("postEmoji")]
        [Summary("try to find an emote by name and post it as a file")]
        public async Task PostEmote(string name, [Optional, Remainder] string msg)
        {
            await Context.Message.DeleteAsync();
            var regex = new Regex(name, RegexOptions.IgnoreCase);
            var emote = Context.Client.GetGuildsAsync().Result.First(g =>
                g.Emotes.Any(e => !e.IsManaged && regex.IsMatch(e.Name.ToLower())))
                .Emotes.First(e => !e.IsManaged && regex.IsMatch(e.Name.ToLower()));
            new WebClient().DownloadFile($"https://cdn.discordapp.com/emojis/{emote.Id}.png", "files/tempEmote.png");
            await Context.Channel.SendFileAsync("files/tempEmote.png", msg);
        }

        [Command("saveAllEmotes")]
        [Remarks("no-help")]
        [Summary("nope")]
        public async Task SaveAllEmotes()
        {
            if (Context.User.Id != 169918990313848832)
            {
                await ReplyAsync(new string[]{"No", "Nah", "Not happening"}[new Random().Next(2)]);
                return;
            }
            int i = 0;
            var msg = await ReplyAsync($"Downloaded `{i}` emotes!");
            foreach (var guild in Context.Client.GetGuildsAsync().Result)
            {
                string path = "files";
                path += $"/{new HackyAfUtils().SanitizeFileName(guild.Name)}";
                Directory.CreateDirectory(path);
                foreach (var emote in guild.Emotes)
                {
                    new WebClient().DownloadFileAsync(new Uri(emote.Url), $"{path}/{emote.Name}.png");
                    i++;
                    if (i % 100 == 0) await msg.ModifyAsync(m => m.Content = $"Downloaded `{i}` emotes!");
                }
            }
            await msg.ModifyAsync(m => m.Content = $"Done! Downloaded {i} emotes!");
        }
    }
}
