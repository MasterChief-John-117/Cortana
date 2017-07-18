using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Newtonsoft.Json;

namespace Cortana.Modules
{
    public class EmoteModule : ModuleBase
    {
        [Command("findEmote")]
        [Summary("Help locate an emote with the name")]
        public async Task FindEmote(string input)
        {
            var regex = new Regex(input, RegexOptions.IgnoreCase);

            var em = new EmbedBuilder();

            foreach (var guild in Context.Client.GetGuildsAsync().Result.Where(g => 
                g.Emotes.Any(e => e.IsManaged && regex.IsMatch(e.Name.ToLower()))).Take(10))
            {
                string guildEmotes = "";
                foreach (var emote in guild.Emotes.Where(e => regex.IsMatch(e.Name.ToLower()) && e.IsManaged).Take(28))
                {
                    guildEmotes += $"<:{emote.Name}:{emote.Id}>";
                }
                em.AddField(new EmbedFieldBuilder().WithName(guild.Name).WithValue(guildEmotes).WithIsInline(guild.Name.Length < 27));
            }
            await ReplyAsync("", embed: em.Build());
        }

        [Command("postEmote")][Alias("postEmoji")]
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
    }
}